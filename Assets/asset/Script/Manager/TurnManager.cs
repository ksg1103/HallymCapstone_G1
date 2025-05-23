using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn,
    GameOver
}

public enum TurnPhase
{
    Preparation,
    Execution
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurn = TurnState.PlayerTurn;
    public TurnPhase CurrentPhase = TurnPhase.Preparation;

    public PlayerState player;
    public List<Enemy> enemies = new List<Enemy>();
    public BulletController bulletController;

    public GameObject executeButton;

    private bool isTurnInProgress = false;
    private bool playerPressedExecute = false;

    private List<ActionData> actionQueue = new List<ActionData>();

    // ���� ���� ����
    public GameObject gameOverPopup;
    public Text moneyText;
    public Text stageText;
    //

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (CurrentTurn != TurnState.GameOver)
        {
            yield return StartCoroutine(HandleTurn());
        }
    }

    IEnumerator HandleTurn()
    {
        isTurnInProgress = true;

        Debug.Log($"[�� ����] {CurrentTurn}");

        switch (CurrentTurn)
        {
            case TurnState.PlayerTurn:
                CurrentPhase = TurnPhase.Preparation;

                bulletController.SpawnBullets(player.bullet);
                bulletController.SetBulletButtonsInteractable(true);

                executeButton.SetActive(true);
                playerPressedExecute = false;

                yield return StartCoroutine(WaitForPlayerExecute());

                executeButton.SetActive(false);
                bulletController.SetBulletButtonsInteractable(false);

                CurrentPhase = TurnPhase.Execution;

                //  ����� �׻� ó�� - �÷��̾�
                player.ProcessDebuffsPerTurn();

                yield return StartCoroutine(ExecuteActions());

                CurrentTurn = TurnState.EnemyTurn;
                break;

            case TurnState.EnemyTurn:
                CurrentPhase = TurnPhase.Preparation;

                yield return StartCoroutine(EnemyActions());

                CurrentPhase = TurnPhase.Execution;

                //  ����� �׻� ó�� - ��� ��
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null && !enemy.IsDead)
                    {
                        enemy.ProcessDebuffsPerTurn();
                    }
                }

                //  ����� �׻� ó�� - ����
                FinalBoss boss = FindObjectOfType<FinalBoss>();
                if (boss != null && !boss.IsDead)
                {
                    boss.ProcessDebuffsPerTurn();
                }

                yield return StartCoroutine(ExecuteActions());

                CurrentTurn = TurnState.PlayerTurn;
                break;
        }

        isTurnInProgress = false;
    }

    IEnumerator WaitForPlayerExecute()
    {
        while (!playerPressedExecute)
        {
            yield return null;
        }
    }

    public void OnExecuteButtonPressed()
    {
        if (CurrentTurn == TurnState.PlayerTurn && CurrentPhase == TurnPhase.Preparation)
        {
            bulletController.RegisterSelectedBullets();
            playerPressedExecute = true;
        }
    }

    public void SubmitPlayerAction(GameObject target, BulletType type)
    {
        var action = new ActionData(player.gameObject, target, player.AttackSpeed, type);
        actionQueue.Add(action);
    }

    public void SubmitGroupedPlayerAction(GameObject target, BulletType type, int count)
    {
        var action = new GroupedActionData(player.gameObject, target, player.AttackSpeed, type, count);
        actionQueue.Add(action);
    }

    IEnumerator EnemyActions()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                Debug.Log($"[���] Enemy: {enemy.name}");
                actionQueue.Add(new ActionData(enemy.gameObject, player.gameObject, enemy.AttackSpeed, BulletType.Curse));
            }
        }

        FinalBoss boss = FindObjectOfType<FinalBoss>();
        if (boss != null && !boss.IsDead)
        {
            if (!actionQueue.Exists(a => a.caster == boss.gameObject))
            {
                Debug.Log($"[���] FinalBoss: {boss.name}");
                actionQueue.Add(new ActionData(boss.gameObject, player.gameObject, boss.AttackSpeed, BulletType.Curse));
            }
            else
            {
                Debug.LogWarning("[�ߺ� ����] ���� �̹� ��ϵ�!");
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ExecuteActions()
    {
        actionQueue.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        foreach (var action in actionQueue)
        {
            if (action.caster == null || action.target == null)
                continue;

            if (action is GroupedActionData groupedAction)
            {
                groupedAction.Execute();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                action.Execute();
                yield return new WaitForSeconds(1f);
            }

            //  ����� ó�� ���ŵ� (�ߺ� ���� �� ���� ����)
        }

        actionQueue.Clear();
        yield return new WaitForSeconds(0.3f);
    }

    public void OnEnemyDied(Enemy deadEnemy)
    {
        if (enemies.Contains(deadEnemy))
            enemies.Remove(deadEnemy);

        Debug.Log($"���� �� ��: {enemies.Count}");

        if (enemies.Count == 0)
        {
            GameOver();
        }
    }

    public void OnEnemyDied(FinalBoss boss)
    {
        Debug.Log("���� ��� ó����");
        ClearScene();
    }

    public void GameOver()
    {
        if (CurrentTurn == TurnState.GameOver) return;

        CurrentTurn = TurnState.GameOver;
        Debug.Log("���� ���� - �˾� ǥ��");

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);

            // GameManager���� ���� �����ͼ� ǥ��
            moneyText.text = $"���� �ݾ�: {GameManager.instance.playerMoney} G";
            stageText.text = $"��������: {GameManager.instance.StageLevel}";
        }

        //if (!Application.CanStreamedLevelBeLoaded("Store"))
        //{
        //    Debug.LogError("Store ���� �ε��� �� �����ϴ�. Build Settings�� ��ϵǾ����� Ȯ���ϼ���.");
        //    return;
        //}

        //SceneManager.LoadScene("Store");
    }

    //
    public void StoreScene()
    {
        if (CurrentTurn == TurnState.GameOver) return;

        CurrentTurn = TurnState.GameOver;
        Debug.Log("���� ���� - ���� ���� �̵�");

        if (!Application.CanStreamedLevelBeLoaded("Store"))
        {
            Debug.LogError("Store ���� �ε��� �� �����ϴ�. Build Settings�� ��ϵǾ����� Ȯ���ϼ���.");
            return;
        }

        SceneManager.LoadScene("Store");
    }
    //

    public void ClearScene()
    {
        if (CurrentTurn == TurnState.GameOver) return;

        CurrentTurn = TurnState.GameOver;
        Debug.Log("���� ���� - ���� ���� �̵�");

        if (!Application.CanStreamedLevelBeLoaded("Clear"))
        {
            Debug.LogError("Clear ���� �ε��� �� �����ϴ�. Build Settings�� ��ϵǾ����� Ȯ���ϼ���.");
            return;
        }

        SceneManager.LoadScene("Clear");
    }

    public void Titlescene()
    {
        GameManager.isReturningFromGame = true;
        SceneManager.LoadScene("Title");

    }

    // ----------------------------
    // ���� Ŭ����: �⺻ ���� ������
    // ----------------------------
    private class ActionData
    {
        public GameObject caster;
        public GameObject target;
        public float attackSpeed;
        public BulletType bulletType;

        public ActionData(GameObject caster, GameObject target, float speed, BulletType bullet)
        {
            this.caster = caster;
            this.target = target;
            this.attackSpeed = speed;
            this.bulletType = bullet;
        }

        public virtual void Execute()
        {
            if (caster.TryGetComponent<PlayerState>(out var player))
            {
                player.Attack(null, target, bulletType);
            }
            else if (caster.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.AttackPlayer(target, bulletType);
            }
            else if (caster.TryGetComponent<FinalBoss>(out var boss))
            {
                boss.AttackPlayer();
            }
        }
    }

    // ----------------------------
    // ���� Ŭ����: ���� �Ѿ� ó����
    // ----------------------------
    private class GroupedActionData : ActionData
    {
        private int count;

        public GroupedActionData(GameObject caster, GameObject target, float speed, BulletType bullet, int count)
            : base(caster, target, speed, bullet)
        {
            this.count = count;
        }

        public override void Execute()
        {
            Debug.Log($"[GroupedActionData] Execute ȣ���: {bulletType} x{count}");

            if (caster.TryGetComponent<PlayerState>(out var player))
            {
                player.AttackGrouped(target, bulletType, count);
            }
        }
    }
}
