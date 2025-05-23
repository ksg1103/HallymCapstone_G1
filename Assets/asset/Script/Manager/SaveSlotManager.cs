using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotManager : MonoBehaviour
{
    public GameObject slotButtonPrefab;
    public Transform slotParent;

    private int slotCount;

    void Start()
    {
        // GameManager���� ���� ���� ��������
        slotCount = GameManager.instance != null ? GameManager.instance.saveSlotCount : 3;

        for (int i = 1; i <= slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotButtonPrefab, slotParent);
            SlotButton btn = slotObj.GetComponent<SlotButton>();
            btn.slotNumber = i;
            btn.RefreshSlotDisplay();
        }
    }
    public void QuitGame()
    {
        Debug.Log("���� ���� ��ư Ŭ����!");
        SoundManager.Instance?.PlayGameQuitSound(); // null üũ ����
        SceneManager.LoadScene("Title");
    }
}
