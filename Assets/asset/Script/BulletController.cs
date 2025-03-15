using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public List<GameObject> bulletPrefabs = new List<GameObject>(); // 직접 추가할 프리팹 리스트
    public Transform spawnParent; // 생성 위치와 부모 오브젝트
    public PlayerState player; // 플레이어 (삭제 함수 호출용)

    [Header("Spawn Settings")]
    public float spawnInterval = 5f; // 몇 초마다 생성할지
    public int maxChildren = 10; // 최대 자식 수 (인스펙터에서 조정)

    private bool isSpawning = true; // 현재 생성 중인지 여부

    void Start()
    {
        // 자동으로 생성 코루틴 시작
        StartCoroutine(SpawnBulletRoutine());
    }

    // 일정 시간마다 총알(버튼) 생성 루틴
    IEnumerator SpawnBulletRoutine()
    {
        while (true)
        {
            int childCount = spawnParent.childCount;

            if (childCount >= maxChildren)
            {
                if (isSpawning)
                {
                    isSpawning = false;
                    Debug.Log("최대 개수 도달. 총알(버튼) 생성 멈춤.");
                }
            }
            else
            {
                if (!isSpawning)
                {
                    isSpawning = true;
                    Debug.Log("총알(버튼) 개수 줄어듦. 생성 재개.");
                }

                if (isSpawning)
                {
                    SpawnRandomBullet();
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // 랜덤으로 총알(버튼) 생성
    void SpawnRandomBullet()
    {
        if (bulletPrefabs.Count == 0)
        {
            Debug.LogWarning("추가된 총알(버튼) 프리팹이 없습니다!");
            return;
        }

        int randomIndex = Random.Range(0, bulletPrefabs.Count);
        GameObject selectedBullet = bulletPrefabs[randomIndex];

        // 버튼(총알) 생성
        GameObject spawnedBullet = Instantiate(selectedBullet, spawnParent.position, Quaternion.identity, spawnParent);

        // 버튼 컴포넌트 가져오기
        Button buttonComponent = spawnedBullet.GetComponent<Button>();
        if (buttonComponent != null)
        {
            // 버튼 클릭 시 PlayerState의 Attack 호출
            buttonComponent.onClick.AddListener(() =>
            {
                player.Attack(spawnedBullet); // 플레이어가 해당 버튼(총알) 삭제
            });
        }
        else
        {
            Debug.LogWarning($"생성된 오브젝트 {selectedBullet.name} 에 Button 컴포넌트가 없습니다.");
        }

        Debug.Log($"{selectedBullet.name} 생성 및 삭제 연결 (현재 {spawnParent.childCount}개)");
    }
}
