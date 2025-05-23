using UnityEngine;
using System.Collections;

public class ManagerCleaner : MonoBehaviour
{
    public GameObject gameManagerPrefab;
    public GameObject inventoryManagerPrefab;
    //public GameObject soundManagerPrefab;
    //public GameObject bgmManagerPrefab;
    //public GameObject saveLoadManagerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateAndMark(gameManagerPrefab);
        InstantiateAndMark(inventoryManagerPrefab);
        //InstantiateAndMark(soundManagerPrefab);
        //InstantiateAndMark(bgmManagerPrefab);
        //InstantiateAndMark(saveLoadManagerPrefab);

        // "���ӿ��� ���ƿ� ��쿡��" �Ŵ��� �ʱ�ȭ
        if (GameManager.isReturningFromGame)
        {
            StartCoroutine(CleanupAndReinitialize());
            GameManager.isReturningFromGame = false; // �ʱ�ȭ �� ����
        }
    }

    IEnumerator CleanupAndReinitialize()
    {
        yield return null; // �� �� ������Ʈ���� ������ �ε�� ������ 1������ ���

        // 1. ���� DontDestroyOnLoad �Ŵ��� ����
        DestroyIfExists("GameManager");
        DestroyIfExists("InventoryManager");
        //DestroyIfExists("SoundManager");
        //DestroyIfExists("BGMManager");
        //DestroyIfExists("Save_Load Manager");
        Debug.Log("�Ŵ��� ����");

        yield return null;
        yield return null;

        // 2. �� �Ŵ��� ������ ���� �� ���
        InstantiateAndMark(gameManagerPrefab);
        InstantiateAndMark(inventoryManagerPrefab);
        //InstantiateAndMark(soundManagerPrefab);
        //InstantiateAndMark(bgmManagerPrefab);
        //InstantiateAndMark(saveLoadManagerPrefab);
        Debug.Log("�Ŵ��� ����");

    }

    void DestroyIfExists(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null && obj.transform.parent == null)
        {
            Debug.Log($"{name} ���� ��û��");
            Destroy(obj);
        }
    }

    void InstantiateAndMark(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("�������� null��");
            return;
        }

        GameObject instance = Instantiate(prefab);
        Debug.Log($"{prefab.name} �ν��Ͻ� ������: {instance.GetInstanceID()}");
        instance.name = prefab.name;
        DontDestroyOnLoad(instance);
    }
}
