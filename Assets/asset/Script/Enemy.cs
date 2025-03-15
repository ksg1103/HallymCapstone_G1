using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int Enemy_num = 1;
    public int StageLevel = 1;
    public float AttackSpeed = 1f; // ������ ����
    public float DefaultDamage = 1f;
    public float ItemDamage = 0f;
    public float FinalDamage;
    public float Health = 100f; // ü�� �߰�


    void Start()
    {
        Debug.Log("�� ��ȣ: " + Enemy_num);

        // ���� ������ ���
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;

        // ���� ��ƾ ����
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (Health <= 0)
        {
            Death(); // ü���� 0 �����̸� Death() ����
        }
    }

    IEnumerator AttackRoutine()
    {
        while (Health > 0) // ü���� 0���� Ŭ ���� ����
        {
            Attack();
            yield return new WaitForSeconds(AttackSpeed); // AttackSpeed���� �ݺ�
        }
    }

    void Attack()
    {
        float minDamage = FinalDamage * 0.67f; // 67% (FinalDamage�� 3/2)
        float maxDamage = FinalDamage * 1.33f; // 133% (FinalDamage�� 3/4)
        float actualDamage = Random.Range(minDamage, maxDamage); // ���� �� ����

        Debug.Log($"���� {actualDamage:F2} ��ŭ ����! (�⺻: {FinalDamage})");
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log($"���� {damage} �������� ����! ���� ü��: {Health}");

        if (Health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log("���� ����߽��ϴ�!");
        Destroy(gameObject); // ������Ʈ ����
    }
}
