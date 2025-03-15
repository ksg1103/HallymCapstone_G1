using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    public int StageLevel = 1;
    public float AttackSpeed = 1f;
    public float DefaultDamage = 1f;
    public float ItemDamage = 0f;
    public float FinalDamage;
    public float Health = 100f;

    void Start()
    {
        // ���� ������ ���
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;
    }

    void Update()
    {
        if (Health <= 0)
        {
            Death(); // ü���� 0 �����̸� Death() ����
        }
    }

    //��ư ������ ���� �� �Ѿ� ����
    public void Attack(GameObject bulletObject)
    {
        if (bulletObject != null)
        {
            Destroy(bulletObject); // �ش� ��ư�� ����
            Debug.Log($"��ư(�Ѿ�) {bulletObject.name} ������!");
        }
        float minDamage = FinalDamage * 0.67f;
        float maxDamage = FinalDamage * 1.33f;
        float actualDamage = Random.Range(minDamage, maxDamage);
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
        Destroy(gameObject);
    }

   
  
}
