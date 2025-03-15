using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int Enemy_num = 1;
    public int StageLevel = 1;
    public float AttackSpeed = 1f; // 변수명 통일
    public float DefaultDamage = 1f;
    public float ItemDamage = 0f;
    public float FinalDamage;
    public float Health = 100f; // 체력 추가


    void Start()
    {
        Debug.Log("적 번호: " + Enemy_num);

        // 최종 데미지 계산
        FinalDamage = (StageLevel * DefaultDamage) + ItemDamage;

        // 공격 루틴 시작
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (Health <= 0)
        {
            Death(); // 체력이 0 이하이면 Death() 실행
        }
    }

    IEnumerator AttackRoutine()
    {
        while (Health > 0) // 체력이 0보다 클 때만 공격
        {
            Attack();
            yield return new WaitForSeconds(AttackSpeed); // AttackSpeed마다 반복
        }
    }

    void Attack()
    {
        float minDamage = FinalDamage * 0.67f; // 67% (FinalDamage의 3/2)
        float maxDamage = FinalDamage * 1.33f; // 133% (FinalDamage의 3/4)
        float actualDamage = Random.Range(minDamage, maxDamage); // 랜덤 값 설정

        Debug.Log($"적이 {actualDamage:F2} 만큼 공격! (기본: {FinalDamage})");
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
        Destroy(gameObject); // 오브젝트 삭제
    }
}
