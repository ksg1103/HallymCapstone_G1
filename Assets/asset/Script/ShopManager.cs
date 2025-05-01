using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public bool isTest= GameManager.instance.isTest; 
    
    public List<ShopItem> allItems;
    public Transform itemParent; // ������ UI�� ������ �θ� ��ü
    public GameObject itemPrefab; // ������ UI ������
    private List<ShopItem> currentItems = new List<ShopItem>();

    //
    public static ShopManager instance;
    public GameObject tooltipPanel;
    public Text tooltipText;
    //
    public Button buyButton; //�̰����� �������� �ν��Ͻ�ȭ �ǹǷ� ��ư�� �̰����� �����ؾ� �ҵ�

    //
    public Text playerMoneyText;        //�÷��̾� ��� ui
    //

    //
    public Button reloadButton; // ���ΰ�ħ ��ư �߰�
    private Text reloadButtonText;  // ���ΰ�ħ ��ư �ؽ�Ʈ
    private int refreshCost = 30;        // ó�� ���ΰ�ħ ���

    public GameObject purchaseMessagePanel;     // ���� ���� �޽��� �˾�
    //
    //
    [Header("�κ� ����á���� �˾� ������")]
    public GameObject ItemFullPopUp;
    public Transform popupParent; //�˾� ���� ��ġ
    //
    //
    //public Inventory playerInventory;

    [Header("�κ��丮 ���� ������")]
    public GameObject inventoryItemPrefab;
    [Header("���� ����")]
    public GameObject slotPrefab;   // ���� ������
    public int slotCount;      // �� �� ��������
    GameObject inventoryPanel;
    GameObject slotPanel;
    [Header("��� ����")]
    public GameObject HeadEqip;
    public GameObject TopEqip;
    public GameObject GlovesEqip;
    public GameObject WeaponEqip;
    public GameObject BottomEqip;
    public GameObject ShoesEqip;
    public GameObject Accessory1Eqip;
    public GameObject Accessory2Eqip;
    public GameObject Accessory3Eqip;
    //public GameObject Accessory4Eqip;
    public GameObject SpecialEqip;
    public GameObject Weapone2Eqip;


    //public GameObject inventorySlot; //slot �������� �ֱ� ���Ͽ�
    public GameObject inventoryItem;

    int slotAmount;
    public List<ShopItem> items = new List<ShopItem>(); //�̰� �� ���� �˾Ƴ��� ����..? inventory�� �ִ� ����Ʈ���� �ٸ� ����

    public List<GameObject> slots = new List<GameObject>();


    

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
        //

        if (purchaseMessagePanel != null)
        {
            purchaseMessagePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("purchaseMessagePanel�� ������� �ʾҽ��ϴ�!");
        }

        slotCount = 32;
    }

    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ���� �ߺ� ���� ����
        if (GameManager.instance.globalItemList == null || GameManager.instance.globalItemList.Count == 0)
        {
            GameManager.instance.globalItemList = allItems; // ���ʿ��� ����
        }
        // ���� �ߺ� ���� ����

        allItems = GameManager.instance.globalItemList;
        //
        //Debug.Log(GameManager.instance.StageLevel);
        GenerateShopItems();

        //
        UpdatePlayerMoneyUI();      // ��� ui������Ʈ
        //
        inventoryPanel = GameObject.Find("InventoryUI");
        Debug.Log(inventoryPanel);
        slotPanel = inventoryPanel.transform.Find("Right Inventory/Scroll View/Viewport/SlotContent").gameObject; //slotpanel�� �������� �Ҵ�
        CreateSlots();
        CreateEquipSlot();
        //
        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(RefreshShop); // ���ΰ�ħ ��ư �̺�Ʈ ���
            Debug.Log("���ΰ�ħ ��ư�� ���������� �����.");

            // ���ε� ��ư �ڽ� �ؽ�Ʈ ��������
            reloadButtonText = reloadButton.GetComponentInChildren<Text>();

            if (reloadButtonText != null)
            {
                UpdateReloadButtonText(); // �ʱ� ��� ǥ��
            }
        }
        else
        {
            Debug.LogError("���ΰ�ħ ��ư�� ã�� �� �����ϴ�! Hierarchy���� ��Ȯ�� �̸��� Ȯ���ϼ���.");
        }
        //

        if (InventoryManager.InventoryInstance != null)
        {
            InventoryManager.InventoryInstance.RebuildInventoryUI();
            Debug.Log("[ShopManager] �κ��丮 UI ���� �Ϸ�");
        }
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

        currentItems.Clear();

        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x
        // count�� 1�� �����۸� ������ ǥ��
        List<ShopItem> availableItems = allItems.FindAll(item => item.IsAvailableInShop());
        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x

        // 10�� �� 6�� ���� ����
        //List<ShopItem> shuffledItems = new List<ShopItem>(allItems);
        //for (int i = 0; i < shuffledItems.Count - 1; i++)
        //{
        //    int randIndex = Random.Range(i, shuffledItems.Count);
        //    (shuffledItems[i], shuffledItems[randIndex]) = (shuffledItems[randIndex], shuffledItems[i]);
        //}

        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x
        for (int i = 0; i < availableItems.Count - 1; i++)
        {
            int randIndex = Random.Range(i, availableItems.Count);
            (availableItems[i], availableItems[randIndex]) = (availableItems[randIndex], availableItems[i]);
        }
        // �� �ڵ� ����ҰŸ� ���� 10���� 6�� ���� �ּ�ó��
        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x

        //for (int i = 0; i < 6; i++)
        //{
        //    currentItems.Add(shuffledItems[i]);

        //    // UI ����
        //    GameObject newItem = Instantiate(itemPrefab, itemParent);
        //    Button button = newItem.GetComponent<Button>();

        //    newItem.transform.Find("ItemName").GetComponent<Text>().text = shuffledItems[i].itemName;
        //    newItem.transform.Find("ItemPrice").GetComponent<Text>().text = shuffledItems[i].price.ToString();
        //    newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = shuffledItems[i].itemIcon;

        //    //Debug.Log(currentItems[i].itemName);


        //    // ShopItemUI ����
        //    ShopItemUI itemUI = newItem.AddComponent<ShopItemUI>();
        //    itemUI.itemData = shuffledItems[i];

        //    ShopItem currentItem = shuffledItems[i];
        //    button.onClick.AddListener(() => OnBuyButtonClicked(currentItem, newItem)); //������ ������ �������� ������ ��ư�� �����ǵ���

        //    //button.onClick.AddListener(() => OnBuyButtonClicked(shuffledItems[i]));

        //}


        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x
        int itemCount = Mathf.Min(6, availableItems.Count);
        for (int i = 0; i < itemCount; i++)
        {
            ShopItem item = availableItems[i];
            currentItems.Add(item);

            GameObject newItem = Instantiate(itemPrefab, itemParent);
            Button button = newItem.GetComponent<Button>();

            newItem.transform.Find("ItemName").GetComponent<Text>().text = item.itemName;
            newItem.transform.Find("ItemPrice").GetComponent<Text>().text = item.price.ToString();
            newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon;

            ShopItemUI itemUI = newItem.AddComponent<ShopItemUI>();
            itemUI.itemData = item;

            button.onClick.AddListener(() => OnBuyButtonClicked(item, newItem));
        }
        // �� �ڵ� ����ҰŸ� ���� for�� �ּ� ó��
        // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x
    }

    //
    // �÷��̾� ��� UI ������Ʈ �Լ�
    public void UpdatePlayerMoneyUI()
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = "Gold: " + GameManager.instance.playerMoney.ToString();
        }
    }
    //




    public bool CanAffordItem(int price)
    {
        //return playerMoney >= price;
        return GameManager.instance.playerMoney >= price;
    }


    void CreateSlots()
    {
        for(int i = 0; i < slotCount; i++)
        {
            items.Add(new ShopItem { id = -1 }); // �� ������ ShopItem id �� -1�̴�.
            slots.Add(Instantiate(slotPrefab));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(slotPanel.transform, false);


            //slotPanel = inventoryPanel.transform.Find("SlotContent").gameObject;
        }
    }
    void CreateEquipSlot() //
    {

        items.Add(new ShopItem { id = -1 });
        slots.Add(HeadEqip);
        slots[slotCount].GetComponent<Slot>().id = slotCount;
        slots[slotCount].tag = "Head_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(TopEqip);
        slots[slotCount+1].GetComponent<Slot>().id = slotCount+1;
        slots[slotCount+1].tag = "Top_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(GlovesEqip);
        slots[slotCount+2].GetComponent<Slot>().id = slotCount+2;
        slots[slotCount+2].tag = "Gloves_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(WeaponEqip);
        slots[slotCount+3].GetComponent<Slot>().id = slotCount+3;
        slots[slotCount+3].tag = "Weapon_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(BottomEqip);
        slots[slotCount + 4].GetComponent<Slot>().id = slotCount + 4;
        slots[slotCount + 4].tag = "Bottom_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(ShoesEqip);
        slots[slotCount+5].GetComponent<Slot>().id = slotCount+5;
        slots[slotCount+5].tag = "Shoes_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(Accessory1Eqip);
        slots[slotCount + 6].GetComponent<Slot>().id = slotCount + 6;
        slots[slotCount + 6].tag = "Accessory_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(Accessory2Eqip);
        slots[slotCount + 7].GetComponent<Slot>().id = slotCount + 7;
        slots[slotCount + 7].tag = "Accessory_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(Accessory3Eqip);
        slots[slotCount + 8].GetComponent<Slot>().id = slotCount + 8;
        slots[slotCount + 8].tag = "Accessory_Equip";

        /*items.Add(new ShopItem { id = -1 });
        slots.Add(Accessory4Eqip);
        slots[slotCount + 9].GetComponent<Slot>().id = slotCount + 9;
        slots[slotCount + 9].tag = "Accessory_Equip";*/

        items.Add(new ShopItem { id = -1 });
        slots.Add(SpecialEqip);
        slots[slotCount + 9].GetComponent<Slot>().id = slotCount + 9;
        slots[slotCount + 9].tag = "Special_Equip";

        items.Add(new ShopItem { id = -1 });
        slots.Add(Weapone2Eqip);
        slots[slotCount + 10].GetComponent<Slot>().id = slotCount + 10;
        slots[slotCount + 10].tag = "Weapon_Equip";
    }

    public void OnBuyButtonClicked(ShopItem item, GameObject itemObject) //���� ������ ������ �Ͼ�� �޼���
    {
        if (item != null)
        {
            if (!InventoryManager.InventoryInstance.ItemFull) //�� if ���� �κ��丮â ���� á���� ���� ���ǹ�
            {
                // ������ ���� ����
                if (CanAffordItem(item.price))
                {
                    //playerMoney -= item.price;
                    //
                    GameManager.instance.playerMoney -= item.price;
                    //
                    //InventoryManager.InventoryInstance.AddItem(item); // �κ��丮�� ������ �߰�

                    item.MarkAsBought(); //  count = 0���� ����

                    Debug.Log($"{item.itemName}��(��) Ŭ���ϼ����ϴ�.");
                    Destroy(itemObject); //ȭ�� �ؿ�  ������ �������� �ν��Ͻ��� �����Ѵ�
                                         //ShopItem �� horizontal layout�̶� �����ϸ� ��� ������ ���� ��ġ �����ؼ� ���δ°� ��������?
                                         //InventoryManager.InventoryInstance.playerInventory.AddItem(item);

                    //

                    UpdatePlayerMoneyUI();
                    addItem(item);

                    StartCoroutine(ShowPurchaseMessage());
                    //���� �κ��� �ִ� ������ ���� �޾ƿͼ� ui�� �Ѱ��ָ� �ɵ�?
                    //playerInventory.AddItem(item);
                    //
                }
                else
                {
                    Debug.Log("���� �����մϴ�!");
                }
            }
            else
            {
                Debug.LogWarning("�κ��丮 �ڸ��� �����ؼ� ������ ���Ÿ� ���մϴ�.");
                //����ٰ� �˾�â�� ��������
                if (ItemFullPopUp != null && popupParent != null)
                {
                    GameObject popupInstance = Instantiate(ItemFullPopUp, popupParent);

                    
                    Text text = popupInstance.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.text = "�κ��丮�� ���� á���ϴ�!";
                    }

                    
                    StartCoroutine(DestroyPopupAfterDelay(popupInstance, 0.5f)); //�˾� ������� �ð� ���⼭ ���� ����
                }

            }
        }
        else
        {
            Debug.LogError("item is null!");
        }
    }

    private IEnumerator DestroyPopupAfterDelay(GameObject popup, float delay) //�˾� �ڵ������� ������� �ڷ�ƾ���� �ۼ�
    {
        yield return new WaitForSeconds(delay);
        if (popup != null)
        {
            Destroy(popup);
        }
    }

    public void addItem(ShopItem item) //�������� �κ��丮�� �߰��Ǵ� ����, inventory�� ������ ����Ǿ��ִ�. ���� �κ��丮�� �������� ���� �ȴ�.
    {   //3.26 ������ ����, �̸�, ��� �Ӽ� ���ؾߵ�
        if (InventoryManager.InventoryInstance.playerInventory != null && item != null)
        {
           
            InventoryManager.InventoryInstance.AddItemToInventory(item); //�� �������� inventory �� ������ ������ ����.
            Debug.Log($"[ShopManager] {item.itemName} �κ��丮�� �߰���?");
            for (int i = 0; i < slots.Count; i++)
            {
                
                    if (slots[i].transform.childCount == 0)
                    {
                    GameObject itemUI = Instantiate(inventoryItemPrefab, slots[i].transform, false); //itemUI�� inventoryprefab�� �ν��Ͻ� ��Ų���̴�.

                    /*Image icon = itemUI.GetComponent<Image>();
                    if (icon != null)
                    {
                        icon.sprite = item.itemIcon;
                    }*/
                    Image icon = itemUI.transform.Find("ItemIcon")?.GetComponent<Image>(); //���� itemicon �̸� ã�� ����.
                    if (icon != null && item.itemIcon != null) //����ó��
                    {
                        icon.sprite = item.itemIcon;
                    }
                    else
                    {
                        Debug.LogWarning($"[ShopManager] ������ ���� ����. icon: {icon}, itemIcon: {item.itemIcon}");
                    }

                    itemUI.name = item.itemName;

                    DragItem dragItem = itemUI.GetComponent<DragItem>();
                    if (dragItem != null)
                    {
                        dragItem.item= item;  //�κ��� ������ ���� ����..?
                        
                    }

                    //
                    InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                    if (uiScript != null)
                    {
                        uiScript.itemData = item;
                    }
                    //

                    Debug.Log($"[ShopManager] {item.itemName} �κ��丮 ���� {i}�� UI�� �߰���");
                    Debug.Log($"[ShopManager] {item.equipType} �� ��� �Ӽ�");
                    return;
                }
            }

            Debug.LogWarning("[ShopManager] �� ���� ����!");

        }
        /*else
        {
            Debug.LogError("playerInventory�� ����ְų� �������� null�Դϴ�!");
        }*/
    }
   /*
    public void BuyItem(ShopItem item)
    {
        if (CanAffordItem(item.price))
        {
            //playerMoney -= item.price;
            //
            GameManager.instance.playerMoney -= item.price;
            //
            //InventoryManager.InventoryInstance.AddItem(item); // �κ��丮�� ������ �߰�
            


            Debug.Log($"{item.itemName}��(��) �����߽��ϴ�!");
        }
        else
        {
            Debug.Log("���� �����մϴ�!");
        }
    }
    */

    // ���ΰ�ħ ��ư Ŭ�� �� ����� �Լ�
    void RefreshShop()
    {
        //GenerateShopItems(); // ���� ������ �ٽ� ����
        if (GameManager.instance.playerMoney >= refreshCost)
        {
            // �� ����
            GameManager.instance.playerMoney -= refreshCost;
            UpdatePlayerMoneyUI();

            // ��� ���� (30 -> 300 -> 3000 -> 30000)
            refreshCost *= 10;

            // ��ư �ؽ�Ʈ ������Ʈ
            UpdateReloadButtonText();

            // ���� ������ �ٽ� ����
            GenerateShopItems();
        }
        else
        {
            Debug.Log("���� �����Ͽ� ���ΰ�ħ�� �� �����ϴ�!");
        }
    }

    // ���ΰ�ħ ��ư�� �ؽ�Ʈ ������Ʈ �Լ�
    void UpdateReloadButtonText()
    {
        if (reloadButtonText != null)
        {
            reloadButtonText.text = $"Reload ({refreshCost})";
        }
    }

    // ���� ���� ���� �޽��� �˾� �Լ�
    IEnumerator ShowPurchaseMessage()
    {
        if (purchaseMessagePanel != null)
        {
            purchaseMessagePanel.transform.SetAsLastSibling();      // ui���� ���ʿ� ��ġ
            purchaseMessagePanel.SetActive(true);
            yield return new WaitForSeconds(1f); // 1�� ���
            purchaseMessagePanel.SetActive(false);
        }
    }


    public void nextscene()
    {
        int stage = GameManager.instance.StageLevel;
        if (isTest == true) {
           

       
          
                SceneManager.LoadScene("BossScene");
            
           
        }
        else { 
           

        if (stage == 3 || stage == 6 || stage == 9)
        {
            SceneManager.LoadScene("BossScene");
        }
        else
        {
            SceneManager.LoadScene("Choice");
        }

    }
    }

    public int GetEquipSlotIndex(EquipType type, int accessoryOrder = 0)
    {
        int baseIndex = slotCount; // ��� ������ slotCount ���ĺ��� ����

        switch (type)
        {
            case EquipType.Head: return baseIndex;
            case EquipType.Top: return baseIndex + 1;
            case EquipType.Gloves: return baseIndex + 2;
            case EquipType.Weapon: return baseIndex + 3;
            case EquipType.Bottom: return baseIndex + 4;
            case EquipType.Shoes: return baseIndex + 5;
            case EquipType.Accessory: return baseIndex + 6 + accessoryOrder; // �׼������� 4��
            // case ����� �߰��ؾ���
            case EquipType.Special: return baseIndex + 7;
            default: return -1;
        }
    }

}
