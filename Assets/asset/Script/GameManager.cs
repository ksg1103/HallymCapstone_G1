using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;   // 리스트 사용하기 위해 작성

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameover = false;
    public int StageLevel=0;
    public int EnemyState=0;
    public int playerMoney = 1000;
    public bool isTest;
    //

    public List<ShopItem> globalItemList;       // 상점 중복 구매 관련

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �Ŵ��� ���� (�ʿ� ��)
        }
        else
        {
            Debug.LogWarning("이미 게임 매니져 존재");
            Destroy(gameObject);
        }
    }
    
}
