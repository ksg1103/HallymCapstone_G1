using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

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
    public int bullet = 2;

    public bool IsDead { get; private set; } = false;
    public GameObject enemy;

    public BuffUIHandler buffUI;
    public Slider HPslider;

    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;

    private Animator animator;

    //ȿ����
    public AudioSource audioSource;
    public AudioClip gunshotClip;
    public AudioClip hurtClip;
    public AudioClip deadClip;

    [Header(" �÷��̾� ������ UI")]
    public Text PlayerdamageLogText;
    [Header(" �÷��̾� �� UI")]
    public Text HealLogText;
    // ���� �� ���� �ִϸ��̼� ���� ����
    public enum WeaponType
    {
        Pistol = 0,
        Rifle = 1
    }

    public WeaponType testWeaponType = WeaponType.Pistol; // �ν����Ϳ��� �ٲ� �� ����

    //���� �� ���� �ִϸ��̼� ���� ����

    void Start()
    {
        Debug.Log("[PlayerState] Start() ȣ���");

        PlayerdamageLogText.text = "";
        HealLogText.text = "";
        MaxHP = Health;
        HPslider.maxValue = MaxHP;
        HPslider.value = MaxHP;

        FinalDamage = (StageLevel * DefaultDamage); // ItemDamage�� ���� �� ����

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
                Debug.LogWarning("[PlayerState] Animator�� �ڽĿ��� ã�ҽ��ϴ�.");
            else
                Debug.LogError("[PlayerState] Animator �� ã��!!");
        }

        if (animator != null)
        {
            foreach (var param in animator.parameters)
            {
                Debug.Log("Animator �Ķ���� �̸�: " + param.name);
            }

            //
            DetectEquippedWeaponType(); // ���� �� ������ ���� �ִϸ��̼� ����
            //
        }
        //  ������ ���� ����
        var stats = GetItemDebuffStats();
        bang = Mathf.Clamp(stats["bang"], 1, 12);     // �ּ� 1, �ִ� 12�� ����
        bullet = Mathf.Clamp(stats["bullet"], 1, 12); // UI���� ������ �ִ� �Ѿ� ��


        //���� �� ���� �ִϸ��̼� ���� ����
        //animator.SetInteger("weaponType", (int)testWeaponType);
    }

    void Update()
    {
        if (Health <= 0 && !IsDead)
        {
            Death();
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                ShowTooltip();
            }
        }
    }

    Dictionary<string, int> GetItemDebuffStats()
    {
        var result = new Dictionary<string, int>
    {
        { "burn", 0 },
        { "bleeding", 0 },
        { "curse", 0 },
        { "blind", 0 },
        { "holy", 0 },
        { "bang", 2 }, // ���� �߰�
        { "bullet", 2 }

    };

        var itemList = InventoryManager.InventoryInstance.GetEquippedItems();
        ItemDamage = 0;

        foreach (var item in itemList)
        {
            //ItemDamage += item.damage;
            result["burn"] += item.burn;
            result["bleeding"] += item.bleeding;
            result["curse"] += item.curse;
            result["blind"] += item.blind;
            result["holy"] += item.holy;
            result["bang"] += item.bang;  //  �����ۿ��� bang �� ����
            result["bullet"] += item.bullet;
        }

        return result;
    }


    public void Attack(GameObject bulletObject, GameObject enemy, BulletType bulletType)
    {
        if (IsDead) return;

        //var debuffs = GetItemDebuffStats();
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;

        //
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(ResetAttackFlag());
        }
        //

        // ������ ������ 
        StartCoroutine(DelayedDamage(enemy, bulletType));

        //float minDamage = FinalDamage * 0.67f;
        //float maxDamage = FinalDamage * 1.33f;
        //float actualDamage = Random.Range(minDamage, maxDamage);
        //Debug.Log($"���� ������: {actualDamage:F2} (�⺻: {FinalDamage})");

        //if (bulletType == BulletType.Normal)
        //{
        //    Debug.Log("�Ϲ�ź - ����� ���� ����");
        //}
        //else if (bulletType == BulletType.Holy)
        //{
        //    int value = 2 + debuffs["holy"];
        //    ApplyDebuff("holy", value);
        //}
        //else if (enemy != null)
        //{
        //    string key = bulletType.ToString().ToLower();
        //    int value = 2 + debuffs[key];

        //    if (enemy.TryGetComponent<Enemy>(out var enemyScript))
        //    {
        //        enemyScript.TakeDamage(actualDamage);
        //        enemyScript.ApplyDebuff(key, value);
        //    }
        //    else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
        //    {
        //        bossScript.TakeDamage(actualDamage);
        //        bossScript.ApplyDebuff(key, value);
        //    }
        //}
    }

    public void AttackGrouped(GameObject enemy, BulletType bulletType, int count)
    {
        if (IsDead) return;

        var debuffs = GetItemDebuffStats();
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;

        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(ResetAttackFlag());
        }

        //
        StartCoroutine(DelayedGroupedDamage(enemy, bulletType, count));
        //

        //float totalDamage = 0f;
        //for (int i = 0; i < count; i++)
        //{
        //    float min = FinalDamage * 0.67f;
        //    float max = FinalDamage * 1.33f;
        //    totalDamage += Random.Range(min, max);
        //}

        //if (bulletType == BulletType.Normal)
        //{
        //    Debug.Log("�Ϲ�ź - ����� ���� ����");
        //}
        //else if (bulletType == BulletType.Holy)
        //{
        //    int buffAmount = count * 2 + debuffs["holy"];
        //    ApplyDebuff("holy", buffAmount);
        //}
        //else if (enemy != null)
        //{
        //    string debuffKey = bulletType.ToString().ToLower();
        //    int debuffAmount = count * (count + 1) / 2 + debuffs[debuffKey];

        //    if (enemy.TryGetComponent<Enemy>(out var enemyScript))
        //    {
        //        enemyScript.TakeDamage(totalDamage);
        //        enemyScript.ApplyDebuff(debuffKey, debuffAmount);
        //    }
        //    else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
        //    {
        //        bossScript.TakeDamage(totalDamage);
        //        bossScript.ApplyDebuff(debuffKey, debuffAmount);
        //    }
        //}
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

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        Health -= damage;
        Debug.Log($"[PlayerState] {damage} ���ظ� ����. ���� ü��: {Health}");
        DamageText(damage);
        if (PlayerdamageLogText != null)
        {
            PlayerdamageLogText.text = damage.ToString("F1"); //�Ҽ��� ù°�ڸ� ������
            StartCoroutine(HideDamageTextAfterDelay(0.5f));
        }
        if (animator != null)
            animator.SetTrigger("isHurt");

        HPslider.value = Mathf.Clamp(Health, 0, MaxHP);

        if (Health <= 0)
        {
            Death();
        }
    }

    public void ProcessDebuffsPerTurn()
    {
        // 1. ����� ���� �� ȸ��
        float debuffDamage = burn + bleeding * 2 + curse;
        float holyHeal = holy * 2f;

        if (debuffDamage > 0)
        {
            Health -= debuffDamage;
            Debug.Log($"[����� ����] {debuffDamage}, ���� ü��: {Health}");

            if (animator != null)
                animator.SetTrigger("isHurt");
            if (PlayerdamageLogText != null)
            {
                PlayerdamageLogText.text = debuffDamage.ToString("F1"); //�Ҽ��� ù°�ڸ� ������
                StartCoroutine(HideDamageTextAfterDelay(0.5f));
            }
        }

        if (holyHeal > 0)
        {
            Health += holyHeal;
            Debug.Log($"[holy ȸ��] {holyHeal}, ���� ü��: {Health}");
            HealText(holyHeal);
        }

        // 2. Holy �⺻ �Ҹ�
        holy = Mathf.Max(0, holy - 1);

        // 3. Burn ��ȭ
        int originalBurn = burn;
        burn = Mathf.Max(0, burn - holy);
        int burnCleansed = originalBurn - burn;
        holy = Mathf.Max(0, holy - burnCleansed);

        // 4. Bleeding ��ȭ
        int originalBleeding = bleeding;
        bleeding = Mathf.Max(0, bleeding - holy);
        int bleedingCleansed = originalBleeding - bleeding;
        holy = Mathf.Max(0, holy - bleedingCleansed);

        // 5. Blind ��ȭ
        int originalBlind = blind;
        blind = Mathf.Max(0, blind - holy);
        int blindCleansed = originalBlind - blind;
        holy = Mathf.Max(0, holy - blindCleansed);

        // 6. Curse ��ȭ
        int originalCurse = curse;
        curse = Mathf.Max(0, curse - holy);
        int curseCleansed = originalCurse - curse;
        holy = Mathf.Max(0, holy - curseCleansed);

        // 7. ü�� Ŭ���� �� �����̴� �ݿ�
        HPslider.value = Mathf.Clamp(Health, 0, MaxHP);

        // 8. ���� UI ����
        if (buffUI != null)
        {
            buffUI.UpdateBuff("burn", burn);
            buffUI.UpdateBuff("bleeding", bleeding);
            buffUI.UpdateBuff("curse", curse);
            buffUI.UpdateBuff("blind", blind);
            buffUI.UpdateBuff("holy", holy);
        }

        // 9. ��� ó��
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
        //infoText.text = "�÷��̾��� �������� �� ���";
        var s = InventoryManager.InventoryInstance.GetTotalStats();
        infoText.text =
            $"�ż�: {s.holy}     ȭ��: {s.burn}     ����: {s.bleeding}\n" +
            $"�Ǹ�: {s.blind}     ����: {s.curse}\n" +
            $"�ϴ� �߻� ���� �߼�: {s.bang}\n" +
            $"�ϴ� ���� �Ѿ� ��: {s.bullet}";
    }

    void Death()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log("�÷��̾� ���!");

        if (animator != null)
            animator.SetBool("isDead", true);

        StartCoroutine(DeathAfterAnimation());
    }

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("isAttacking", false);
    }

    IEnumerator DeathAfterAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        TurnManager.Instance.GameOver();
        Destroy(gameObject);
    }

    public void OnDeathAnimationEnd()
    {
        TurnManager.Instance.GameOver();
        Destroy(gameObject);
    }

    void ShowTooltip()
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        nameText.text = "�÷��̾�";
        //infoText.text = "�÷��̾��� �������� �� ���";
        var s = InventoryManager.InventoryInstance.GetTotalStats();
        infoText.text =
            $"�ż�: {s.holy}     ȭ��: {s.burn}     ����: {s.bleeding}\n" +
            $"�Ǹ�: {s.blind}     ����: {s.curse}\n" +
            $"�ϴ� �߻� ���� �߼�: {s.bang}\n" +
            $"�ϴ� ���� �Ѿ� ��: {s.bullet}";
    }

    //
    IEnumerator DelayedDamage(GameObject enemy, BulletType bulletType)
    {
        yield return new WaitForSeconds(1.5f); // ���� �ִϸ��̼� Ÿ�̹�

        var debuffs = GetItemDebuffStats(); // ���� ���⼭ ����� ���!

        float minDamage = FinalDamage * 0.67f;
        float maxDamage = FinalDamage * 1.33f;
        float actualDamage = Random.Range(minDamage, maxDamage);
        Debug.Log($"���� ������: {actualDamage:F2} (�⺻: {FinalDamage})");

        if (bulletType == BulletType.Normal)
        {
            Debug.Log("�Ϲ�ź - ����� ���� ����");
        }
        else if (bulletType == BulletType.Holy)
        {
            int value = 2 + debuffs["holy"];
            ApplyDebuff("holy", value);
        }
        else if (enemy != null)
        {
            string key = bulletType.ToString().ToLower();
            int value = 2 + debuffs[key];

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(actualDamage);
                enemyScript.ApplyDebuff(key, value);
            }
            else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
            {
                bossScript.TakeDamage(actualDamage);
                bossScript.ApplyDebuff(key, value);
            }
        }
    }
    //

    //
    IEnumerator DelayedGroupedDamage(GameObject enemy, BulletType bulletType, int count)
    {
        yield return new WaitForSeconds(1.5f); // �ִϸ��̼ǰ� ���缭 Ÿ�̹� ����

        var debuffs = GetItemDebuffStats(); // ������� ���⼭ ���

        float totalDamage = 0f;
        for (int i = 0; i < count; i++)
        {
            float min = FinalDamage * 0.67f;
            float max = FinalDamage * 1.33f;
            totalDamage += Random.Range(min, max);
        }

        if (bulletType == BulletType.Normal)
        {
            Debug.Log("�Ϲ�ź - ����� ���� ����");
        }
        else if (bulletType == BulletType.Holy)
        {
            int buffAmount = count * 2 + debuffs["holy"];
            ApplyDebuff("holy", buffAmount);
        }
        else if (enemy != null)
        {
            string debuffKey = bulletType.ToString().ToLower();
            int debuffAmount = count * (count + 1) / 2 + debuffs[debuffKey];

            if (enemy.TryGetComponent<Enemy>(out var enemyScript))
            {
                enemyScript.TakeDamage(totalDamage);
                enemyScript.ApplyDebuff(debuffKey, debuffAmount);
            }
            else if (enemy.TryGetComponent<FinalBoss>(out var bossScript))
            {
                bossScript.TakeDamage(totalDamage);
                bossScript.ApplyDebuff(debuffKey, debuffAmount);
            }
        }
    }
    void DamageText(float damage)
    {
        if (PlayerdamageLogText != null)
        {
            PlayerdamageLogText.text = damage.ToString("F1"); //�Ҽ��� ù°�ڸ� ������
            StartCoroutine(HideDamageTextAfterDelay(0.5f));
            
        }
    }

    void HealText(float heal)
    {
        if (HealLogText != null)
        {
            HealLogText.text = heal.ToString("F1"); //�Ҽ��� ù°�ڸ� ������
            StartCoroutine(HideHealTextAfterDelay(0.5f));
        }
    }
    //

    // �� �߻���
    public void PlayGunshotSound()
    {
        if (audioSource != null && gunshotClip != null)
        {
            audioSource.PlayOneShot(gunshotClip);
        }
    }
    //

    // �� �¾����� ���� ����
    public void PlayHurtSound()
    {
        if (audioSource != null && hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }
    }
    //

    // �׾����� ����
    public void PlayDeadSound()
    {
        if (audioSource != null && deadClip != null)
        {
            audioSource.PlayOneShot(deadClip);
        }
    }
    //

    //������ ����id�� �ִϸ��̼� ����
    void DetectEquippedWeaponType()
    {
        var itemList = InventoryManager.InventoryInstance.GetEquippedItems();
        testWeaponType = WeaponType.Pistol; // �⺻��

        foreach (var item in itemList)
        {
            if (item.id == 6) // Pistol
            {
                testWeaponType = WeaponType.Pistol;
                break;
            }
            else if (item.id == 18) // Rifle
            {
                testWeaponType = WeaponType.Rifle;
                break;
            }
        }

        animator.SetInteger("weaponType", (int)testWeaponType);
    }
    //
    //�׳� �����׽�Ʈ

    IEnumerator HideDamageTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PlayerdamageLogText != null)
            PlayerdamageLogText.text = "";
    }
    IEnumerator HideHealTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (HealLogText != null)
            HealLogText.text = "";
    }
    //
}
