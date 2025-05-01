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
            Debug.Log($"[보스 공격] powerStage: {powerStage}, 데미지: {curseAmount}, 저주 {curseAmount}");
        }
        else
        {
            ApplyDebuff("curse", curseAmount);
            Debug.Log($"[보스 자해] powerStage: 7, 자기 자신에게 저주 {curseAmount}, 플레이어 피해 없음");
        }

        powerStage = Mathf.Clamp(powerStage + 1, 1, 7);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        HPslider.value = Mathf.Max(0, Health);
        Debug.Log($"[보스 피격] 데미지 {damage}, 현재 체력: {Health}");

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
            Debug.Log($"[보스 디버프 피해] {debuffDamage}, 남은 체력: {Health}");
        }

        if (healAmount > 0)
        {
            Health += healAmount;
            Debug.Log($"[보스 holy 회복] {healAmount}, 현재 체력: {Health}");
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
        nameText.text = "보스";
        infoText.text = "보스의 스텟정보 들어갈 장소";
    }
    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("보스 사망");

        TurnManager.Instance?.OnEnemyDied(this);
        Destroy(gameObject);
    }
}
