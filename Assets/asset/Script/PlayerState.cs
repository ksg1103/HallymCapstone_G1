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

    // �����/���� ����
    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;


    int totalBleeding, totalCurse, totalBurn, totalBlind, totalHoly;
    public List<ShopItem> ItemList; //�� ����Ʈ�� ���â�� �ִ� ������ �޾ƿ�. ItemList��� ����Ʈ�� ���� ����

    void Start()
    {   
        MaxHP = Health;
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;
        HPslider.maxValue = MaxHP;
        HPslider.value = MaxHP;



        ItemList = InventoryManager.InventoryInstance.GetEquippedItems(); //start �� ���� �Ѿ�� ����ɶ� �������� �޾ƿ�

        int totalBleeding = 1, totalCurse = 1, totalBurn = 1, totalBlind = 1, totalHoly = 1;//����Ʈ ��
        for (int i = 0; i < ItemList.Count; i++) { //�����ۿ� �ִ� �������� ���������� �޾ƿͼ� �ջ���
            totalBleeding += ItemList[i].bleeding;
            totalCurse += ItemList[i].curse;
            totalBurn += ItemList[i].burn;
            totalBlind += ItemList[i].blind;
            totalHoly += ItemList[i].holy;
        }
        // �ջ��� �������� ������
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
        Debug.Log($"���� {actualDamage:F2} ��ŭ ����! (�⺻: {FinalDamage})");

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
        Debug.Log($"�÷��̾ {damage} �������� ����! ���� ü��: {Health}");
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

        Debug.Log($"[��������] {bulletType} �Ѿ� {count}�� ��� �� �� ������: {totalDamage:F2}");

        if (bulletType == BulletType.Holy)
        {
            int buffAmount = count * 2;
            ApplyDebuff("holy", buffAmount);
            Debug.Log($"[holy] ���� {buffAmount} ����");
        }
        else if (enemy != null)
        {
            string debuffKey = bulletType.ToString().ToLower(); // "burn", "bleeding" ��
            int debuffAmount = count * (count + 1) / 2; // ��: 1+2+3 = 6 (���� ����)

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(totalDamage);
                enemyScript.ApplyDebuff(debuffKey, debuffAmount);
                Debug.Log($"[�����] {debuffKey} {debuffAmount} ����");
            }
            else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
            {
                bossScript.TakeDamage(totalDamage);
                bossScript.ApplyDebuff(debuffKey, debuffAmount);
                Debug.Log($"[�����] {debuffKey} {debuffAmount} ����");
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
            Debug.Log($"[����� ����] {debuffDamage}, ���� ü��: {Health}");
        }

        if (holyHeal > 0)
        {
            Health += holyHeal;
            Debug.Log($"[���� ȸ��] holy {holyHeal}, ���� ü��: {Health}");
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
        nameText.text = "�÷��̾�";
        infoText.text = "�÷��̾��� �������� �� ���";
    }
    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("�÷��̾� ���!");
        Destroy(gameObject);
    }
}
