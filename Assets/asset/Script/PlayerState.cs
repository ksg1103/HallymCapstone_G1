using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum BulletType
{
    Normal,
    Burn,
    Bleeding,
    Curse,
    Blind,
    Holy
}

public class PlayerState : MonoBehaviour, IPointerEnterHandler
{
    public int StageLevel = 1;
    public float AttackSpeed = 1f;
    public float DefaultDamage = 1f;
    public float ItemDamage = 0f;
    public float FinalDamage;
    public float Health = 100f;
    public float MaxHP;
    public int bang = 2;

    public bool IsDead { get; private set; } = false;
    public GameObject enemy;

    public BuffUIHandler buffUI;
    public Slider HPslider;

    // 디버프/버프 상태
    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;


    int totalBleeding, totalCurse, totalBurn, totalBlind, totalHoly;
    public List<ShopItem> ItemList; //이 리스트로 장비창에 있는 정보를 받아옴. ItemList라는 리스트를 따로 만듬

    void Start()
    {   
        MaxHP = Health;
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;
        HPslider.maxValue = MaxHP;
        HPslider.value = MaxHP;



        ItemList = InventoryManager.InventoryInstance.GetEquippedItems(); //start 즉 씬이 넘어가서 실행될때 아이템을 받아옴

        int totalBleeding = 1, totalCurse = 1, totalBurn = 1, totalBlind = 1, totalHoly = 1;//디폴트 값
        for (int i = 0; i < ItemList.Count; i++) { //아이템에 있는 정보들을 종합적으로 받아와서 합산함
            totalBleeding += ItemList[i].bleeding;
            totalCurse += ItemList[i].curse;
            totalBurn += ItemList[i].burn;
            totalBlind += ItemList[i].blind;
            totalHoly += ItemList[i].holy;
        }
        // 합산한 정보들을 저장함
        this.bleeding = totalBleeding;
        this.curse = totalCurse;
        this.burn = totalBurn;
        this.blind = totalBlind;
        this.holy = totalHoly;
        

        
    }

    void Update()
    {
        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }

    public void Attack(GameObject bulletObject, GameObject enemy, BulletType bulletType)
    {
        if (IsDead) return;

        float minDamage = FinalDamage * 0.67f;
        float maxDamage = FinalDamage * 1.33f;
        float actualDamage = Random.Range(minDamage, maxDamage);
        Debug.Log($"내가 {actualDamage:F2} 만큼 공격! (기본: {FinalDamage})");

        if (bulletType == BulletType.Holy)
        {
            ApplyDebuff("holy", 2);
        }
        else if (enemy != null)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(actualDamage);
                switch (bulletType)
                {
                    case BulletType.Burn: enemyScript.ApplyDebuff("burn", 2); break;
                    case BulletType.Bleeding: enemyScript.ApplyDebuff("bleeding", 2); break;
                    case BulletType.Curse: enemyScript.ApplyDebuff("curse", 2); break;
                    case BulletType.Blind: enemyScript.ApplyDebuff("blind", 2); break;
                }
            }
            else
            {
                FinalBoss bossScript = enemy.GetComponent<FinalBoss>();
                if (bossScript != null)
                {
                    bossScript.TakeDamage(actualDamage);
                    switch (bulletType)
                    {
                        case BulletType.Burn: bossScript.ApplyDebuff("burn", 2); break;
                        case BulletType.Bleeding: bossScript.ApplyDebuff("bleeding", 2); break;
                        case BulletType.Curse: bossScript.ApplyDebuff("curse", 2); break;
                        case BulletType.Blind: bossScript.ApplyDebuff("blind", 2); break;
                    }
                }
            }
        }
    }


    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        Health -= damage;
        Debug.Log($"플레이어가 {damage} 데미지를 받음! 현재 체력: {Health}");
        HPslider.value = Mathf.Clamp(Health, 0, MaxHP);

        if (Health <= 0)
        {
            Death();
        }
    }
    public void AttackGrouped(GameObject enemy, BulletType bulletType, int count)
    {
        if (IsDead) return;

        float totalDamage = 0f;
        for (int i = 0; i < count; i++)
        {
            float min = FinalDamage * 0.67f;
            float max = FinalDamage * 1.33f;
            totalDamage += Random.Range(min, max);
        }

        Debug.Log($"[누적공격] {bulletType} 총알 {count}개 사용 → 총 데미지: {totalDamage:F2}");

        if (bulletType == BulletType.Holy)
        {
            int buffAmount = count * 2;
            ApplyDebuff("holy", buffAmount);
            Debug.Log($"[holy] 버프 {buffAmount} 적용");
        }
        else if (enemy != null)
        {
            string debuffKey = bulletType.ToString().ToLower(); // "burn", "bleeding" 등
            int debuffAmount = count * (count + 1) / 2; // 예: 1+2+3 = 6 (누진 적용)

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(totalDamage);
                enemyScript.ApplyDebuff(debuffKey, debuffAmount);
                Debug.Log($"[디버프] {debuffKey} {debuffAmount} 적용");
            }
            else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
            {
                bossScript.TakeDamage(totalDamage);
                bossScript.ApplyDebuff(debuffKey, debuffAmount);
                Debug.Log($"[디버프] {debuffKey} {debuffAmount} 적용");
            }
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

        if (buffUI != null)
        {
            int value = type switch
            {
                "burn" => burn,
                "bleeding" => bleeding,
                "curse" => curse,
                "blind" => blind,
                "holy" => holy,
                _ => 0
            };

            buffUI.UpdateBuff(type, value);
        }
    }

    public void ProcessDebuffsPerTurn()
    {
        float debuffDamage = burn + bleeding * 2 + curse;
        float holyHeal = holy * 2f;

        if (debuffDamage > 0)
        {
            Health -= debuffDamage;
            Debug.Log($"[디버프 피해] {debuffDamage}, 현재 체력: {Health}");
        }

        if (holyHeal > 0)
        {
            Health += holyHeal;
            Debug.Log($"[버프 회복] holy {holyHeal}, 현재 체력: {Health}");
        }

        if (burn > 0) burn--;
        if (bleeding > 0) bleeding--;
        if (curse > 0) curse--;
        if (blind > 0) blind--;
        if (holy > 0) holy--;

        HPslider.value = Mathf.Clamp(Health, 0, MaxHP);

        if (buffUI != null)
        {
            buffUI.UpdateBuff("burn", burn);
            buffUI.UpdateBuff("bleeding", bleeding);
            buffUI.UpdateBuff("curse", curse);
            buffUI.UpdateBuff("blind", blind);
            buffUI.UpdateBuff("holy", holy);
        }

        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        nameText.text = "플레이어";
        infoText.text = "플레이어의 스텟정보 들어갈 장소";
    }
    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("플레이어 사망!");
        Destroy(gameObject);
    }
}
