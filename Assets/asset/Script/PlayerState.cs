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
        // 최종 데미지 계산
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;
    }

    void Update()
    {
        if (Health <= 0)
        {
            Death(); // 체력이 0 이하이면 Death() 실행
        }
    }

    //버튼 누르면 공격 및 총알 삭제
    public void Attack(GameObject bulletObject)
    {
        if (bulletObject != null)
        {
            Destroy(bulletObject); // 해당 버튼만 삭제
            Debug.Log($"버튼(총알) {bulletObject.name} 삭제됨!");
        }
        float minDamage = FinalDamage * 0.67f;
        float maxDamage = FinalDamage * 1.33f;
        float actualDamage = Random.Range(minDamage, maxDamage);
        Debug.Log($"내가 {actualDamage:F2} 만큼 공격! (기본: {FinalDamage})");
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log($"적이 {damage} 데미지를 받음! 현재 체력: {Health}");

        if (Health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log("적이 사망했습니다!");
        Destroy(gameObject);
    }

   
  
}
