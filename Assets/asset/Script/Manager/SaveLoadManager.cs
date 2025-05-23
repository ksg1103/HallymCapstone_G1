using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // ����
    public void Save(GameData data, int slot)
    {
        string path = GetSlotPath(slot);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"[���� �Ϸ�] ���� {slot} �����: {path}");
    }

    // �ҷ�����
    public GameData Load(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);

            Debug.Log($"[�ҷ����� �Ϸ�] ���� {slot}");
            return data;
        }

        Debug.LogWarning($"[�ε� ����] ���� {slot}�� ������ ����");
        return null;
    }

    // ����
    public void Delete(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[���� �Ϸ�] ���� {slot} ������ ������");
        }
        else
        {
            Debug.LogWarning($"[���� ����] ���� {slot}�� ������ ���� ����");
        }
    }

    // ���� ��� ����
    private string GetSlotPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot{slot}.json");
    }

    // ���� ���� �� Ÿ��Ʋ ȭ�� �̵�
    public void QuitGame()
    {
        Debug.Log("���� ���� ��ư Ŭ����!");
        SoundManager.Instance?.PlayGameQuitSound(); // null üũ ����
        SceneManager.LoadScene("Title");
    }
}
