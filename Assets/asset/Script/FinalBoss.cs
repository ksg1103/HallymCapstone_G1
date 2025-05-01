using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalBoss : MonoBehaviour,IPointerEnterHandler
{
    public int StageLevel = 1;
    public float Health = 100f;
    public float MaxHP;
    public float AttackSpeed = 1f;
    public float DefaultDamage = 10f;
    public float FinalDamage;
    public int Accuracy;
    public int Evasion;
    public bool IsDead { get; private set; }

    public BuffUIHandler buffUI;
    public Slider HPslider;

    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;

    private GameObject player;
    private int powerStage = 1;

    void Start()
    {
        float SM = 1 + ((float)StageLevel / 10);

        MaxHP = Health;
        HPslider.maxValue = MaxHP;
        HPslider.value = MaxHP;

        FinalDamage = DefaultDamage;

        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }

    public void AttackPlayer()
    {
        if (player == null) return;

        PlayerState ps = player.GetComponent<PlayerState>();
        if (ps == null) return;

        int[] curseTable = { 0, 2, 4, 8, 16, 32, 64, 128 };
        int curseAmount = curseTable[powerStage];

        if (powerStage < 7)
        {
            ps.ApplyDebuff("curse", curseAmount);
            Debug.Log($"[���� ����] powerStage: {powerStage}, ������: {curseAmount}, ���� {curseAmount}");
        }
        else
        {
            ApplyDebuff("curse", curseAmount);
            Debug.Log($"[���� ����] powerStage: 7, �ڱ� �ڽſ��� ���� {curseAmount}, �÷��̾� ���� ����");
        }

        powerStage = Mathf.Clamp(powerStage + 1, 1, 7);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        HPslider.value = Mathf.Max(0, Health);
        Debug.Log($"[���� �ǰ�] ������ {damage}, ���� ü��: {Health}");

        if (Health <= 0)
        {
            Death();
        }
    }

    public void ApplyDebuff(string type, int amount)
    {
        switch (type)
        {
            case "burn": burn += amount; break;
            case "bleeding": bleeding += amount; break;
            case "curse": curse += amount; break;
            case "blind": blind += amount; break;
            case "holy": holy += amount; break;
        }

        UpdateBuffUI();
    }

    public void ProcessDebuffsPerTurn()
    {
        float debuffDamage = burn + bleeding * 2 + curse;
        float healAmount = holy * 2;

        if (debuffDamage > 0)
        {
            Health -= debuffDamage;
            Debug.Log($"[���� ����� ����] {debuffDamage}, ���� ü��: {Health}");
        }

        if (healAmount > 0)
        {
            Health += healAmount;
            Debug.Log($"[���� holy ȸ��] {healAmount}, ���� ü��: {Health}");
        }

        if (burn > 0) burn--;
        if (bleeding > 0) bleeding--;
        if (curse > 0) curse--;
        if (blind > 0) blind--;
        if (holy > 0) holy--;

        Health = Mathf.Clamp(Health, 0, MaxHP);
        HPslider.value = Health;

        UpdateBuffUI();

        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }

    void UpdateBuffUI()
    {
        if (buffUI != null)
        {
            buffUI.UpdateBuff("burn", burn);
            buffUI.UpdateBuff("bleeding", bleeding);
            buffUI.UpdateBuff("curse", curse);
            buffUI.UpdateBuff("blind", blind);
            buffUI.UpdateBuff("holy", holy);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        nameText.text = "����";
        infoText.text = "������ �������� �� ���";
    }
    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("���� ���");

        TurnManager.Instance?.OnEnemyDied(this);
        Destroy(gameObject);
    }
}
