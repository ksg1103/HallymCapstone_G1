using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems;
    public Transform itemParent; // 아이템 UI가 생성될 부모 객체
    public GameObject itemPrefab; // 아이템 UI 프리팹
    private List<ShopItem> currentItems = new List<ShopItem>();

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
        }
    }
}
