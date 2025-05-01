
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour 
{
    public static InventoryManager InventoryInstance;

    public Inventory playerInventory;

    
    public GameObject PopupForSellPrefab;

    public int itemManyLimit; // ���� ���ѿ� �̰� ���� ������ shopmanager�� slotcount�� �޾ƿ���
    public int howMany; //���� �κ��丮�� ������ �����
    public bool ItemFull; //bool ������ ���� ���� �ٸ� Ŭ�������� �����ϱ� ����
    //�ֳ��ϸ� �� ������ true �ϰ�� UI�� ������� �׳� return �ϱ� ���ؼ�
    //private ShopManager shopManager;

    public Dictionary<string, ShopItem> equippedItems = new Dictionary<string, ShopItem>(); //string���� ��� ������ ���� �Ǵ��Ϸ���
    
    void Awake()
    {   
        if (InventoryInstance == null)
        {
            InventoryInstance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("�κ��丮 �Ŵ���");
        }

        else
        {
            Destroy(gameObject);
        }
        itemManyLimit = 6;
        howMany = 0;
        ItemFull = false;
        
    }

    void Start()
    {
        
        if (playerInventory != null)
        {
            playerInventory.playerItems.Clear(); //���� �����Ҷ� �κ��丮 �ʱ�ȭ
            Debug.Log("[InventoryManager] playerItems �ʱ�ȭ �Ϸ�");
        }
        if (InventoryInstance == null)
        {
            Debug.LogError("InventoryManager instance�� null�Դϴ�!");
        }
    }
    public List<ShopItem> GetEquippedItems() //�� �޼���� ��񸮽�Ʈ�� �ҷ��� �� �ִ�.
    {
        return playerInventory.currentPlayerItems;
    }
    public void AddItemToInventory(ShopItem item) //�κ��丮 â�� ������ �߰�
    {   
        if (playerInventory == null)
        {
            Debug.LogError("playerInventory�� null�Դϴ�!");
            return;
        }

        Debug.Log("[InventoryManager] ����� playerInventory ������Ʈ �̸�: " + playerInventory.gameObject.name);
        howMany++;
        if(howMany >= itemManyLimit)
        {
            Debug.LogWarning("�κ��丮 â�� ���� á���ϴ�."); //�̷��Ը� �ϸ� �κ��丮 �Ŵ��� �󿡸� ������Ʈ �ȵ�. UI���� �Ȱ��� �����
            ItemFull = true; 
            return;
        }
        playerInventory.playerItems.Add(item);
        item.IfEquip = 1;
        

        Debug.Log($"[InventoryManager] {item.itemName} �߰���. �� ����: {playerInventory.playerItems.Count}");
    }
   
    public ShopItem GetEquippedItem(string slotType)
    {
        return equippedItems.ContainsKey(slotType) ? equippedItems[slotType] : null;
    }

    public void EquipItem(ShopItem item) //�κ��丮 â���� ���â����
    {
        if (!playerInventory.currentPlayerItems.Contains(item))
        {
            item.IfEquip = 2;
            playerInventory.currentPlayerItems.Add(item); //��� ����Ʈ�� �ְ�
            playerInventory.playerItems.Remove(item); //�κ��丮 ����Ʈ���� ����� 
            Debug.Log($"������: {item.itemName}");
            howMany--;
            if (item.equipType == EquipType.Special)
            {
                Debug.Log("[Special] ���Ե��� �ٽ� Ȱ��ȭ�մϴ�.");
                ShopManager shop = ShopManager.instance;
                shop.Weapone2Eqip.SetActive(true);


            }
        }
    }

    public void UnequipItem(ShopItem item)
    {
        if (playerInventory.currentPlayerItems.Contains(item))
        {
            item.IfEquip = 1;
            playerInventory.currentPlayerItems.Remove(item);
            playerInventory.playerItems.Add(item);
            Debug.Log($"������: {item.itemName}");
            howMany++;
            if (item.equipType == EquipType.Special)
            {
                Debug.Log("[Special] ���Ե��� �ٽ� ��Ȱ��ȭ�մϴ�.");
                ShopManager shop = ShopManager.instance;

                Transform weapon2Slot = shop.Weapone2Eqip.transform;

                if (weapon2Slot.childCount > 0)
                {
                    GameObject weaponItemObj = weapon2Slot.GetChild(0).gameObject;
                    DragItem weaponDragItem = weaponItemObj.GetComponent<DragItem>();

                    if (weaponDragItem != null)
                    {
                        ShopItem weaponItem = weaponDragItem.item;

                        // ������ �̵�
                        playerInventory.currentPlayerItems.Remove(weaponItem);
                        playerInventory.playerItems.Add(weaponItem);
                        weaponItem.IfEquip = 1;
                        howMany++;

                        Debug.Log($"[Special ���� ����] Weapon2�� ������ {weaponItem.itemName} �κ��丮�� �̵�");

                        // �� ���� ã�Ƽ� UI ����
                        //  �κ��丮 UI ���� �� �� �� ã�Ƽ� ������ ��ġ
                        for (int i = 0; i < shop.slotCount; i++)
                        {
                            Transform invSlot = shop.slots[i].transform;

                            if (invSlot.childCount == 0)
                            {
                                GameObject itemUI = GameObject.Instantiate(shop.inventoryItemPrefab, invSlot);
                                itemUI.name = weaponItem.itemName;

                                //  �̹��� �Ҵ�(�̹����� �ڽ��� itemicon�� �ֱ� ������)
                                Transform iconTransform = itemUI.transform.Find("ItemIcon");
                                if (iconTransform != null)
                                {
                                    Image iconImage = iconTransform.GetComponent<Image>();
                                    if (iconImage != null && weaponItem.itemIcon != null)
                                    {
                                        iconImage.sprite = weaponItem.itemIcon;
                                        iconImage.enabled = true;
                                    }
                                }

                                //  �巡�� ���� �Ҵ�
                                DragItem newDrag = itemUI.GetComponent<DragItem>();
                                if (newDrag != null)
                                {
                                    newDrag.item = weaponItem;
                                    newDrag.slot = i;

                                    //  �� ����� �巡�׵�
                                    CanvasGroup cg = itemUI.GetComponent<CanvasGroup>();
                                    if (cg != null)
                                        cg.blocksRaycasts = true;
                                }

                                //  ���� ���� ����
                                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                                if (uiScript != null)
                                    uiScript.itemData = weaponItem;

                                //  ShopManager.items�� ����ȭ (�߿�!!)
                                shop.items[i] = weaponItem;

                                break;
                            }
                        }


                        Destroy(weaponItemObj); // ���� ���â�� UI ����
                    }
                }

                shop.Weapone2Eqip.SetActive(false);
            }

        }
    }
    
    public void RemoveItemFromInventory(int slotIndex)
    {
        GameObject popupInstance = Instantiate(PopupForSellPrefab, transform.root);
        if (slotIndex >= 0 && slotIndex < playerInventory.playerItems.Count)
        {
            playerInventory.playerItems[slotIndex] = new ShopItem(); 
            playerInventory.playerItems[slotIndex].id = -1; 
            Debug.Log($"[InventoryManager] ���� {slotIndex} ������ ���ŵ�");
        }
    }

    public void RequestRemoveItem(int slotIndex, GameObject targetItemObject)
    {   
        DragItem dragItem = targetItemObject.GetComponent<DragItem>();
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject popupInstance = Instantiate(PopupForSellPrefab, canvas.transform);
        Debug.Log("���� Ȯ�� �˾� ������");

        ShopItem item = dragItem.item;

        // �ؽ�Ʈ ����
        Text messageText = popupInstance.transform.Find("Message").GetComponent<Text>();
        messageText.text = $"�Ǹ��Ͻðڽ��ϱ�?\n({(int)(item.price * 0.7)} Gold)";

        // ��ư ����
        Button yesBtn = popupInstance.transform.Find("YesButton").GetComponent<Button>();
        Button noBtn = popupInstance.transform.Find("NOButton").GetComponent<Button>();

        yesBtn.onClick.AddListener(() =>
        {
            if (dragItem != null)
            {
                ShopItem item = dragItem.item;

                if (item.IfEquip == 1)
                {
                    Debug.Log($"{item.itemName}�� �κ��丮���� �Ǹ� Ȯ��");

                    
                    playerInventory.playerItems.Remove(item);

                    GameManager.instance.playerMoney += (int)(item.price * 0.7);
                    howMany--;
                    /*
                    if (shopManager != null)
                    {
                        shopManager.UpdatePlayerMoneyUI();
                    }
                    else
                    {
                        Debug.LogError("shopManager�� null�Դϴ�! ���� Ȯ�� ���.");
                    }*/
                    if (slotIndex >= 0 && slotIndex < playerInventory.playerItems.Count)
                    {   
                        playerInventory.playerItems[slotIndex] = new ShopItem();
                        playerInventory.playerItems[slotIndex].id = -1;
                    }
                }
                else if(item.IfEquip == 2) 
                {
                    Debug.Log($"{item.itemName}�� ���â���� �Ǹ� Ȯ��");
                    playerInventory.currentPlayerItems.Remove(item);

                    GameManager.instance.playerMoney += (int)(item.price * 0.7);
                    /*
                    if (shopManager != null)
                    {
                        shopManager.UpdatePlayerMoneyUI();
                    }
                    else
                    {
                        Debug.LogError("shopManager�� null�Դϴ�! ���� Ȯ�� ���.");
                    }*/
                    if (slotIndex >= 0 && slotIndex < playerInventory.currentPlayerItems.Count)
                    {
                        playerInventory.currentPlayerItems[slotIndex] = new ShopItem();
                        playerInventory.currentPlayerItems[slotIndex].id = -1;
                    }
                }

                // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x
                item.MarkAsSold();      // ���� ����� ?? ���� �׽�Ʈ��
                // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x

                Destroy(targetItemObject);
                Destroy(popupInstance);
                ShopManager.instance.UpdatePlayerMoneyUI();
            }
            else
            {
                Debug.LogError("DragItem ������Ʈ�� ã�� �� �����ϴ�.");
            }
           
            
        });

        noBtn.onClick.AddListener(() =>
        {
            Destroy(popupInstance);
            Debug.Log("���� ��ҵ�");
        });
    }

    public void RebuildInventoryUI() //�� �Ѿ�͵� �κ��丮 ui�� �ٽ� ���̰�

    {
        // 1. ���� ���� �ʱ�ȭ
        for (int i = 0; i < ShopManager.instance.slots.Count; i++)
        {
            Transform slot = ShopManager.instance.slots[i].transform;
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        // 2. �κ��丮 ������ ���� (�Ϲ� ������: playerItems)
        for (int i = 0; i < playerInventory.playerItems.Count; i++)
        {
            ShopItem item = playerInventory.playerItems[i];
            if (item != null && item.id != -1)
            {
                GameObject itemUI = GameObject.Instantiate(ShopManager.instance.inventoryItemPrefab, ShopManager.instance.slots[i].transform);
                itemUI.name = item.itemName;

                Image icon = itemUI.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                DragItem dragItem = itemUI.GetComponent<DragItem>();
                if (dragItem != null)
                {
                    dragItem.item = item;
                    dragItem.slot = i;
                }

                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.itemData = item;
                }
            }
        }

        // 3. ���� ������ ���� (���â: currentPlayerItems)
        int accessoryCount = 0;
        for (int i = 0; i < playerInventory.currentPlayerItems.Count; i++)
        {
            ShopItem item = playerInventory.currentPlayerItems[i];
            if (item != null && item.id != -1)
            {
                int slotIndex;

                if (item.equipType == EquipType.Accessory)
                {
                    slotIndex = ShopManager.instance.GetEquipSlotIndex(item.equipType, accessoryCount);
                    accessoryCount++;
                }
                else
                {
                    slotIndex = ShopManager.instance.GetEquipSlotIndex(item.equipType);
                }

                if (slotIndex == -1) continue;

                GameObject itemUI = GameObject.Instantiate(ShopManager.instance.inventoryItemPrefab, ShopManager.instance.slots[slotIndex].transform);
                itemUI.name = item.itemName;

                Image icon = itemUI.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                DragItem dragItem = itemUI.GetComponent<DragItem>();
                if (dragItem != null)
                {
                    dragItem.item = item;
                    dragItem.slot = slotIndex;
                }

                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.itemData = item;
                }
            }
        }
        /*for (int i = 0; i < ShopManager.instance.slots.Count; i++)
        {
            Transform slot = ShopManager.instance.slots[i].transform;

            // ������ ������� �ʴٸ� ����
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        // �κ��丮 ������ ����
        for (int i = 0; i < playerInventory.playerItems.Count; i++)
        {
            ShopItem item = playerInventory.playerItems[i];
            if (item != null && item.id != -1)
            {
                GameObject itemUI = GameObject.Instantiate(ShopManager.instance.inventoryItemPrefab, ShopManager.instance.slots[i].transform);
                itemUI.name = item.itemName;

                Image icon = itemUI.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                DragItem dragItem = itemUI.GetComponent<DragItem>();
                if (dragItem != null)
                {
                    dragItem.item = item;
                    dragItem.slot = i;
                }

                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.itemData = item;
                }
            }
        }

        // ���� ������ ����
        for (int i = 0; i < playerInventory.currentPlayerItems.Count; i++)
        {
            ShopItem item = playerInventory.currentPlayerItems[i];
            if (item != null && item.id != -1)
            {
                // ��� ���� index ã��
                int slotIndex = ShopManager.instance.GetEquipSlotIndex(item.equipType, i);
                if (slotIndex == -1) continue;

                GameObject itemUI = GameObject.Instantiate(ShopManager.instance.inventoryItemPrefab, ShopManager.instance.slots[slotIndex].transform);
                itemUI.name = item.itemName;

                Image icon = itemUI.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                DragItem dragItem = itemUI.GetComponent<DragItem>();
                if (dragItem != null)
                {
                    dragItem.item = item;
                    dragItem.slot = slotIndex;
                }

                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.itemData = item;
                }
            }
        }*/
    }


    /*
    playerInventory.playerItems.Add(item);
        Debug.Log($"[InventoryManager] {item.itemName} �κ��丮�� �߰���. ���� ����: {playerInventory.playerItems.Count}");*/

    // ���� UI�� �����۵� �߰��Ϸ��� ���⼭ Instantiate(itemUI) ���ֱ�
}
/*
public static InventoryManager InventoryInstance;
public ScrollRect scrollRect; // ��ũ�Ѻ�
//public GameObject itemPrefab; // ������ ������
private List<ShopItem> inventory = new List<ShopItem>();

public Transform onDragParent;
public Transform dragLayer;


void Awake()
{
    if (InventoryInstance == null)
    {
        InventoryInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Debug.LogWarning("�̹� �κ��丮 �Ŵ��� ����");
        Destroy(gameObject);
    }
}
void Start()
{
    // ��ũ�Ѻ� ����
    if (scrollRect == null)
    {
        scrollRect = GameObject.Find("ScrollRect").GetComponent<ScrollRect>();
        //ó�� Ÿ��Ʋ ȭ����� �������̴ϱ� ��ũ�Ѻ並 �ִ°͵� 
    }
    dragLayer = GameObject.Find("Equipment").transform;
}
public void AddItem(ShopItem item)
{
    int j = 0;
    inventory.Add(item);
    /*
    Debug.Log($"{item.itemName}��(��) �κ��丮�� �߰��߽��ϴ�.");
    Debug.Log("���� �κ��丮:");
    foreach (ShopItem i in inventory)
    {
        Debug.Log(i.itemName);
        j++;
    }
    Debug.Log(j);
    // �κ��丮 UI ������Ʈ �ڵ� �߰�
    AddItemToScrollRect(item);
}
private void AddItemToScrollRect(ShopItem item)
{
    // ��ũ�Ѻ��� Content ��ü

    GameObject contentObject = scrollRect.content.gameObject;


    // ������ �̸� �ؽ�Ʈ ����
    GameObject itemNameObject = new GameObject("ItemName");
    itemNameObject.transform.SetParent(contentObject.transform);
    Text itemNameText = itemNameObject.AddComponent<Text>();
    itemNameText.text = item.itemName;
    itemNameText.fontSize = 24;
    itemNameText.alignment = TextAnchor.MiddleCenter;

    // ������ ������ �̹��� ����
    GameObject itemIconObject = new GameObject("ItemIcon");
    itemIconObject.transform.SetParent(contentObject.transform);
    Image itemIconImage = itemIconObject.AddComponent<Image>();
    itemIconImage.sprite = item.itemIcon;
    itemIconImage.preserveAspect = true;

    //������ ����
    GameObject itemPriceObject = new GameObject("Price");
    itemPriceObject.transform.SetParent(contentObject.transform);
    Text itemPriceText = itemPriceObject.AddComponent<Text>();
    itemPriceText.text = $"Price: {item.price}";
    itemPriceText.fontSize = 24;
    itemPriceText.alignment = TextAnchor.MiddleCenter;

    // ���̾ƿ� ���� (��: ���� ���̾ƿ�)
    GameObject itemLayoutObject = new GameObject("Item");
    itemLayoutObject.transform.SetParent(contentObject.transform);
    itemLayoutObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.5f);

    VerticalLayoutGroup VerticalLayoutGroup = itemLayoutObject.AddComponent<VerticalLayoutGroup>();
    itemNameObject.transform.SetParent(itemLayoutObject.transform);
    itemIconObject.transform.SetParent(itemLayoutObject.transform);
    itemPriceObject.transform.SetParent(itemLayoutObject.transform);

    GameObject itemButtonObject = new GameObject("ItemButton");
    itemButtonObject.transform.SetParent(itemLayoutObject.transform); ;
    Button itemButton = itemButtonObject.AddComponent<Button>();
    itemButton.onClick.AddListener(() => OnItemButtonClicked(item)); // �κ��丮�� �����ۿ� ��ư ����



    Image imageComponent = itemLayoutObject.AddComponent<Image>();
    imageComponent.color = Color.white;

    DragItem dragItem = itemLayoutObject.AddComponent<DragItem>();
    dragItem.onDragParent = dragLayer; //���̾��Ű�� equipment�� slot���� �ο�

    CanvasGroup canvasGroup = itemLayoutObject.AddComponent<CanvasGroup>();

}


public void RemoveItem(ShopItem item)
{
    inventory.Remove(item);
    Debug.Log($"{item.itemName}��(��) �κ��丮���� �����߽��ϴ�.");
    // �κ��丮 UI ������Ʈ �ڵ� �߰��ؾ� ��
}

private void OnItemButtonClicked(ShopItem item)
{
    // ������ ��ư Ŭ�� �� ������ �ൿ
    Debug.Log($"{item.itemName} ��ư Ŭ��!");

}*/




