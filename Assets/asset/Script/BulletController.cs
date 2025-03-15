using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public List<GameObject> bulletPrefabs = new List<GameObject>(); // ���� �߰��� ������ ����Ʈ
    public Transform spawnParent; // ���� ��ġ�� �θ� ������Ʈ
    public PlayerState player; // �÷��̾� (���� �Լ� ȣ���)

    [Header("Spawn Settings")]
    public float spawnInterval = 5f; // �� �ʸ��� ��������
    public int maxChildren = 10; // �ִ� �ڽ� �� (�ν����Ϳ��� ����)

    private bool isSpawning = true; // ���� ���� ������ ����

    void Start()
    {
        // �ڵ����� ���� �ڷ�ƾ ����
        StartCoroutine(SpawnBulletRoutine());
    }

    // ���� �ð����� �Ѿ�(��ư) ���� ��ƾ
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
                    Debug.Log("�ִ� ���� ����. �Ѿ�(��ư) ���� ����.");
                }
            }
            else
            {
                if (!isSpawning)
                {
                    isSpawning = true;
                    Debug.Log("�Ѿ�(��ư) ���� �پ��. ���� �簳.");
                }

                if (isSpawning)
                {
                    SpawnRandomBullet();
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // �������� �Ѿ�(��ư) ����
    void SpawnRandomBullet()
    {
        if (bulletPrefabs.Count == 0)
        {
            Debug.LogWarning("�߰��� �Ѿ�(��ư) �������� �����ϴ�!");
            return;
        }

        int randomIndex = Random.Range(0, bulletPrefabs.Count);
        GameObject selectedBullet = bulletPrefabs[randomIndex];

        // ��ư(�Ѿ�) ����
        GameObject spawnedBullet = Instantiate(selectedBullet, spawnParent.position, Quaternion.identity, spawnParent);

        // ��ư ������Ʈ ��������
        Button buttonComponent = spawnedBullet.GetComponent<Button>();
        if (buttonComponent != null)
        {
            // ��ư Ŭ�� �� PlayerState�� Attack ȣ��
            buttonComponent.onClick.AddListener(() =>
            {
                player.Attack(spawnedBullet); // �÷��̾ �ش� ��ư(�Ѿ�) ����
            });
        }
        else
        {
            Debug.LogWarning($"������ ������Ʈ {selectedBullet.name} �� Button ������Ʈ�� �����ϴ�.");
        }

        Debug.Log($"{selectedBullet.name} ���� �� ���� ���� (���� {spawnParent.childCount}��)");
    }
}
