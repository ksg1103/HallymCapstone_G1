using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems;
    public Transform itemParent; // ������ UI�� ������ �θ� ��ü
    public GameObject itemPrefab; // ������ UI ������
    private List<ShopItem> currentItems = new List<ShopItem>();

    //
    public static ShopManager instance;
    public GameObject tooltipPanel;
    public Text tooltipText;

    void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        tooltipPanel = GameObject.Find("Canvas/ItemTooltip"); // �θ� ������Ʈ�� Canvas��� ��θ� ��Ȯ�� ����
        if (tooltipPanel == null)
        {
            tooltipPanel = FindObjectOfType<Canvas>().transform.Find("ItemTooltip")?.gameObject;
        }

        if (tooltipPanel == null)
        {
            Debug.LogError(" tooltipPanel�� ã�� �� �����ϴ�! Hierarchy���� ��Ȯ�� �̸��� Ȯ���ϼ���.");
        }

        else
        {
            tooltipPanel.SetActive(false); // ���� �� ��Ȱ��ȭ
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

            // ShopItemUI ����
            ShopItemUI itemUI = newItem.AddComponent<ShopItemUI>();
            itemUI.itemData = shuffledItems[i];
        }
    }
}
