using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour, IPointerEnterHandler
{
    public int Enemy_num = 1;
    public int StageLevel = 1;
    public bool IsDead { get; private set; }

    public float AttackSpeed = 1f;
    public float DefaultDamage = 1f;
    public float FinalDamage;
    public float Health = 10f;
    public float MaxHP;

    public int Accuracy;
    public int Evasion;

    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;

    private GameObject player;

    public BuffUIHandler buffUI;
    public UnityEngine.UI.Slider HPslider;

    void Start()
    {
        StageLevel = GameManager.instance != null ? GameManager.instance.StageLevel : 1;

        int[] parts = SplitPowerWithRange(GameManager.instance.EnemyState);
        float SM = 1 + ((float)StageLevel / 10);

        Health = SM * parts[0];
        MaxHP = Health;
        HPslider.maxValue = MaxHP;
        HPslider.value = MaxHP;
        AttackSpeed = Mathf.Clamp((100f - parts[1]) / 10f, 0.2f, 10f);

        Accuracy = parts[2];
        DefaultDamage = SM * parts[3];
        FinalDamage = DefaultDamage;

        Evasion = parts[4];

        Debug.Log($"[스탯] 체력: {Health}, 공격속도: {AttackSpeed}, 공격력: {DefaultDamage}, 명중률: {Accuracy}, 회피율: {Evasion}");

        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }

    public void AttackPlayer(BulletType type)
    {
        if (player != null)
        {
            float minDamage = FinalDamage * 0.67f;
            float maxDamage = FinalDamage * 1.33f;
            float actualDamage = Random.Range(minDamage, maxDamage);

            Debug.Log($"적이 {actualDamage:F2} 만큼 플레이어를 공격! (타입: {type})");

            PlayerState playerState = player.GetComponent<PlayerState>();
            if (playerState != null)
            {
                playerState.TakeDamage(actualDamage);

                switch (type)
                {
                    case BulletType.Burn: playerState.ApplyDebuff("burn", 1); break;
                    case BulletType.Bleeding: playerState.ApplyDebuff("bleeding", 1); break;
                    case BulletType.Curse: playerState.ApplyDebuff("curse", 1); break;
                    case BulletType.Blind: playerState.ApplyDebuff("blind", 1); break;
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        HPslider.value = Health;
        Debug.Log($"적이 {damage} 데미지를 받음! 현재 체력: {Health}");

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
        float heal = holy * 2;

        if (debuffDamage > 0)
        {
            Health -= debuffDamage;
            Debug.Log($"[디버프] {debuffDamage} 데미지, 현재 체력: {Health}");
        }

        if (heal > 0)
        {
            Health += heal;
            Debug.Log($"[holy 회복] {heal} 회복, 현재 체력: {Health}");
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

    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("보스 사망!");

        TurnManager.Instance?.GameOver();
        Destroy(gameObject);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        nameText.text = "적";
        infoText.text = "적의 스텟정보 들어갈 장소";
    }
    public int[] SplitPowerWithRange(int totalPower)
    {
        int[] parts = new int[5];

        parts[1] = Random.Range(50, 71);
        int remaining = totalPower - parts[1];

        for (int i = 0; i < 5; i++)
        {
            if (i == 1) continue;

            if (i < 4)
            {
                int maxAllowed = remaining;
                int minAllowed = 0;

                if (i == 0)
                {
                    maxAllowed = Mathf.Min(remaining, totalPower / 3);
                    minAllowed = Mathf.Min(100, maxAllowed);
                }

                if (i == 2) maxAllowed = Mathf.Min(100, remaining);
                if (i == 3)
                {
                    maxAllowed = Mathf.Min(50, remaining);
                    minAllowed = Mathf.Min(10, maxAllowed);
                }

                parts[i] = Random.Range(minAllowed, maxAllowed + 1);
                remaining -= parts[i];
            }
            else
            {
                parts[i] = Mathf.Min(100, remaining);
            }
        }

        return parts;
    }
}
