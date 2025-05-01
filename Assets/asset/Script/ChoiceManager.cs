using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public bool isTest = GameManager.instance.isTest;
    public int apoint;  
    public int bpoint;
    public Text uiA;
    public Text uiB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
       apoint = UnityEngine.Random.Range(350, 451);
        Debug.Log(GameManager.instance.StageLevel);


        do
        {
            bpoint = UnityEngine.Random.Range(350, 451);
        } while (apoint == bpoint); // �ߺ� ����

        uiA.text = "�����:" + (apoint * 10).ToString();
        uiB.text = "�����:" + (bpoint * 10).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void buttonleft()
    {
        if (isTest == true)
        { SceneManager.LoadScene("Finalboss"); }
        else
        {
            int stage = GameManager.instance.StageLevel;

            if (stage == 3 || stage == 6 || stage == 9)
            {
                SceneManager.LoadScene("Finalboss");
            }
            else
            {

                GameManager.instance.EnemyState += apoint;
                GameManager.instance.playerMoney += (apoint * 10);
                SceneManager.LoadScene("Fighting");
                Debug.Log(GameManager.instance.playerMoney);
            }
        }
    }
    public void buttonright()
    {

        GameManager.instance.EnemyState += bpoint;
        GameManager.instance.playerMoney += (bpoint * 10);
        Debug.Log(GameManager.instance.playerMoney);
        SceneManager.LoadScene("Fighting");
    }
}
