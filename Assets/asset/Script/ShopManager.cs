using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems;
    public Transform itemParent; // ������ UI�� ������ �θ� ��ü
    public GameObject itemPrefab; // ������ UI ������
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
        // ���� ������ ����
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        // 10�� �� 6�� ���� ����
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

            // UI ����
            GameObject newItem = Instantiate(itemPrefab, itemParent);
            newItem.transform.Find("ItemName").GetComponent<Text>().text = shuffledItems[i].itemName;
            newItem.transform.Find("ItemPrice").GetComponent<Text>().text = shuffledItems[i].price.ToString();
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = shuffledItems[i].itemIcon;
        }
    }
}
