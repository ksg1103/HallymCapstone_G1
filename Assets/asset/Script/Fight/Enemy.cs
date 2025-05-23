using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

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

    public BuffUIHandler buffUI;
    public Slider HPslider;

    //�ִϸ����� ���� 
    private Animator animator;
    //

    //ȿ����
    public AudioSource audioSource;
    public AudioClip gunshotClip;
    public AudioClip hurtClip;
    public AudioClip deadClip;
    //

    [Header("������ UI")]
    public Text damageLogText;


    void Start()
    {
        damageLogText.text = "";
        //
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
                Debug.LogWarning("[Enemy] Animator�� �ڽĿ��� ã�ҽ��ϴ�.");
            else
                Debug.LogError("[Enemy] Animator �� ã��!!");
        }
        //

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

        Debug.Log($"[����] ü��: {Health}, ���ݼӵ�: {AttackSpeed}, ���ݷ�: {DefaultDamage}, ���߷�: {Accuracy}, ȸ����: {Evasion}");

        //  TurnManager�� �ڵ� ���
        if (TurnManager.Instance != null && !TurnManager.Instance.enemies.Contains(this))
        {
            TurnManager.Instance.enemies.Add(this);
        }
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

    public void AttackPlayer(GameObject target, BulletType type)
    {
        //
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
            StartCoroutine(ResetAttackFlag());
        }
        //

        Debug.Log($"[Enemy ���� ����] {name} �� {target?.name}");

        if (target == null || IsDead)
        {
            Debug.LogWarning("[Enemy ���� ���] Ÿ���� null�̰ų� �̹� ���");
            return;
        }

        int finalAccuracy = Mathf.Clamp(Accuracy - blind, 0, 100);
        int hitChance = Random.Range(1, 101);

        if (hitChance > finalAccuracy)
        {
            Debug.Log($"[Miss] ������ ������! (���� ���߷�: {finalAccuracy}%, �Ǹ� ���: {blind})");
            return;
        }

        float minDamage = FinalDamage * 0.67f;
        float maxDamage = FinalDamage * 1.33f;
        float actualDamage = Random.Range(minDamage, maxDamage);

        Debug.Log($"���� {actualDamage:F2} ��ŭ �÷��̾ ����! (Ÿ��: {type})");

        PlayerState playerState = target.GetComponent<PlayerState>();
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

    public void TakeDamage(float damage)
    {
        //
        if (animator != null)
        {
            animator.SetTrigger("isHurt");
        }
        //

        Health -= damage;
        HPslider.value = Health;
        Debug.Log($"���� {damage} �������� ����! ���� ü��: {Health}");
        DamageText(damage);
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
            Debug.Log($"[�����] {debuffDamage} ������, ���� ü��: {Health}");
            DamageText(debuffDamage);
        }

        if (heal > 0)
        {
            Health += heal;
            Debug.Log($"[holy ȸ��] {heal} ȸ��, ���� ü��: {Health}");
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
        Debug.Log("�� ���!");

        //TurnManager.Instance?.GameOver();
        //Destroy(gameObject);

        //
        if (animator != null)
        {
            animator.SetBool("isDead", true); // ��� �ִϸ��̼� ����
            StartCoroutine(DeathAfterAnimation()); // �ִϸ��̼� ���� ������ ��� �� ó��
        }
        else
        {
            TurnManager.Instance?.GameOver();
            Destroy(gameObject);
        }
        //
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        if (nameText != null) nameText.text = "��";
        if (infoText != null) infoText.text = "[����] ü��: {Health}, ���ݼӵ�: {AttackSpeed}, ���ݷ�: {DefaultDamage}, ���߷�: {Accuracy}, ȸ����: {Evasion}";
    }

    void ShowTooltip()
    {
        Text nameText = GameObject.Find("NameTxt")?.GetComponent<Text>();
        Text infoText = GameObject.Find("infoText")?.GetComponent<Text>();
        if (nameText != null) nameText.text = "��";
        if (infoText != null) infoText.text = "[����] ü��: {Health}, ���ݼӵ�: {AttackSpeed}, ���ݷ�: {DefaultDamage}, ���߷�: {Accuracy}, ȸ����: {Evasion}";
    }

    public int[] SplitPowerWithRange(int totalPower)
    {
        int[] parts = new int[5];

        parts[1] = Random.Range(50, 71); // ���ݼӵ�
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

                if (i == 2)
                {
                    maxAllowed = Mathf.Min(100, remaining);
                    minAllowed = Mathf.Min(60, maxAllowed); //  ���߷� �ּ� 60%
                }

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

    IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(1f);
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }

    //
    IEnumerator DeathAfterAnimation()
    {
        // "Dead" ���°� ���۵� ������ ���
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead_n"))
        {
            yield return null;
        }

        // �ش� �ִϸ��̼��� ���� ������ ���
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Debug.Log("��� �ִϸ��̼� ����!");

        TurnManager.Instance?.StoreScene();
        Destroy(gameObject);
    }
    //
    void DamageText(float damage)
    {
        if (damageLogText != null)
        {
            damageLogText.text = damage.ToString("F1"); //�Ҽ��� ù°�ڸ� ������
            StartCoroutine(HideDamageTextAfterDelay(0.5f));
        }
    }//

    IEnumerator HideDamageTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (damageLogText != null)
            damageLogText.text = "";
    }
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

}
