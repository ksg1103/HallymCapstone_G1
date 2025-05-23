using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndText : MonoBehaviour
{
    // ���� ���� ����
    public GameObject gameOverPopup;
    public Text moneyText;
    public Text stageText;
    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // GameManager���� ���� �����ͼ� ǥ��
        moneyText.text = $"���� �ݾ�: {GameManager.instance.playerMoney} G";
        stageText.text = $"Ŭ������ ��������: {GameManager.instance.StageLevel}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Titlescene()
    {
        GameManager.isReturningFromGame = true;
        SceneManager.LoadScene("Title");

    }
}
