using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems;
    public Transform itemParent; // 아이템 UI가 생성될 부모 객체
    public GameObject itemPrefab; // 아이템 UI 프리팹
    private List<ShopItem> currentItems = new List<ShopItem>();

    //
    public static ShopManager instance;
    public GameObject tooltipPanel;
    public Text tooltipText;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        tooltipPanel = GameObject.Find("Canvas/ItemTooltip"); // 부모 오브젝트가 Canvas라면 경로를 명확히 지정
        if (tooltipPanel == null)
        {
            tooltipPanel = FindObjectOfType<Canvas>().transform.Find("ItemTooltip")?.gameObject;
        }

        if (tooltipPanel == null)
        {
            Debug.LogError(" tooltipPanel을 찾을 수 없습니다! Hierarchy에서 정확한 이름을 확인하세요.");
        }

        else
        {
            tooltipPanel.SetActive(false); // 시작 시 비활성화
        }
    }

    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateShopItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateShopItems()
    {
        // 기존 아이템 제거
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        // 10개 중 6개 랜덤 선택
        currentItems.Clear();
        List<ShopItem> shuffledItems = new List<ShopItem>(allItems);
        for (int i = 0; i < shuffledItems.Count - 1; i++)
        {
            int randIndex = Random.Range(i, shuffledItems.Count);
            (shuffledItems[i], shuffledItems[randIndex]) = (shuffledItems[randIndex], shuffledItems[i]);
        }

        for (int i = 0; i < 6; i++)
        {
            currentItems.Add(shuffledItems[i]);

            // UI 생성
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.transform.Find("ItemName").GetComponent<Text>().text = shuffledItems[i].itemName;
            newItem.transform.Find("ItemPrice").GetComponent<Text>().text = shuffledItems[i].price.ToString();
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = shuffledItems[i].itemIcon;

            // ShopItemUI 연결
            ShopItemUI itemUI = newItem.AddComponent<ShopItemUI>();
            itemUI.itemData = shuffledItems[i];
        }
    }
}
