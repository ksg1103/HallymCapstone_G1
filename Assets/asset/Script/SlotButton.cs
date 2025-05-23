using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlotButton : MonoBehaviour
{
    public int slotNumber;
    public Text slotText;
    public Button loadBtn;
    public Button deleteBtn;

    void Start()
    {
        RefreshSlotDisplay();

        loadBtn.onClick.AddListener(() =>
        {
            Debug.Log($"[SlotButton] ���� {slotNumber} �ε� �õ�");

            if (GameManager.instance != null)
            {
                GameManager.instance.LoadGame(slotNumber); //  ��ü ������ �ε� �� ����
            }
            else
            {
                Debug.LogError("[SlotButton] GameManager �ν��Ͻ��� null�Դϴ�! GameManager�� ���� �����ϴ��� Ȯ���ϼ���.");
            }
        });

        deleteBtn.onClick.AddListener(() =>
        {
            SaveLoadManager.Instance.Delete(slotNumber);
            RefreshSlotDisplay();
        });
    }

    public void RefreshSlotDisplay()
    {
        string path = Path.Combine(Application.persistentDataPath, $"save_slot{slotNumber}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            slotText.text = $"������: {data.playerMoney} / ��������: {data.StageLevel + 1}";
        }
        else
        {
            slotText.text = "�� ����";
        }
    }
}
