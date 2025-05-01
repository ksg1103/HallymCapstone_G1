
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour 
{
    public static InventoryManager InventoryInstance;

    public Inventory playerInventory;

    
    public GameObject PopupForSellPrefab;

    public int itemManyLimit; // 개수 제한용 이걸 따로 만들지 shopmanager의 slotcount를 받아올지
    public int howMany; //지금 인벤토리에 아이템 몇개인지
    public bool ItemFull; //bool 변수를 따로 만들어서 다른 클래스에도 전달하기 위해
    //왜냐하면 이 변수가 true 일경우 UI쪽 변경없이 그냥 return 하기 위해서
    //private ShopManager shopManager;

    public Dictionary<string, ShopItem> equippedItems = new Dictionary<string, ShopItem>(); //string으로 장비 슬롯의 유형 판단하려고
    
    void Awake()
    {   
        if (InventoryInstance == null)
        {
            InventoryInstance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("인벤토리 매니저");
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
            playerInventory.playerItems.Clear(); //게임 시작할때 인벤토리 초기화
            Debug.Log("[InventoryManager] playerItems 초기화 완료");
        }
        if (InventoryInstance == null)
        {
            Debug.LogError("InventoryManager instance가 null입니다!");
        }
    }
    public List<ShopItem> GetEquippedItems() //이 메서드로 장비리스트를 불러올 수 있다.
    {
        return playerInventory.currentPlayerItems;
    }
    public void AddItemToInventory(ShopItem item) //인벤토리 창에 아이템 추가
    {   
        if (playerInventory == null)
        {
            Debug.LogError("playerInventory가 null입니다!");
            return;
        }

        Debug.Log("[InventoryManager] 연결된 playerInventory 오브젝트 이름: " + playerInventory.gameObject.name);
        howMany++;
        if(howMany >= itemManyLimit)
        {
            Debug.LogWarning("인벤토리 창이 가득 찼읍니다."); //이렇게만 하면 인벤토리 매니저 상에만 업데이트 안됨. UI쪽은 똑같이 사라짐
            ItemFull = true; 
            return;
        }
        playerInventory.playerItems.Add(item);
        item.IfEquip = 1;
        

        Debug.Log($"[InventoryManager] {item.itemName} 추가됨. 총 개수: {playerInventory.playerItems.Count}");
    }
   
    public ShopItem GetEquippedItem(string slotType)
    {
        return equippedItems.ContainsKey(slotType) ? equippedItems[slotType] : null;
    }

    public void EquipItem(ShopItem item) //인벤토리 창에서 장비창으로
    {
        if (!playerInventory.currentPlayerItems.Contains(item))
        {
            item.IfEquip = 2;
            playerInventory.currentPlayerItems.Add(item); //장비 리스트에 넣고
            playerInventory.playerItems.Remove(item); //인벤토리 리스트에서 지우고 
            Debug.Log($"장착됨: {item.itemName}");
            howMany--;
            if (item.equipType == EquipType.Special)
            {
                Debug.Log("[Special] 슬롯들을 다시 활성화합니다.");
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
            Debug.Log($"해제됨: {item.itemName}");
            howMany++;
            if (item.equipType == EquipType.Special)
            {
                Debug.Log("[Special] 슬롯들을 다시 비활성화합니다.");
                ShopManager shop = ShopManager.instance;

                Transform weapon2Slot = shop.Weapone2Eqip.transform;

                if (weapon2Slot.childCount > 0)
                {
                    GameObject weaponItemObj = weapon2Slot.GetChild(0).gameObject;
                    DragItem weaponDragItem = weaponItemObj.GetComponent<DragItem>();

                    if (weaponDragItem != null)
                    {
                        ShopItem weaponItem = weaponDragItem.item;

                        // 데이터 이동
                        playerInventory.currentPlayerItems.Remove(weaponItem);
                        playerInventory.playerItems.Add(weaponItem);
                        weaponItem.IfEquip = 1;
                        howMany++;

                        Debug.Log($"[Special 연쇄 해제] Weapon2에 장착된 {weaponItem.itemName} 인벤토리로 이동");

                        // 빈 슬롯 찾아서 UI 복원
                        //  인벤토리 UI 슬롯 중 빈 곳 찾아서 아이템 배치
                        for (int i = 0; i < shop.slotCount; i++)
                        {
                            Transform invSlot = shop.slots[i].transform;

                            if (invSlot.childCount == 0)
                            {
                                GameObject itemUI = GameObject.Instantiate(shop.inventoryItemPrefab, invSlot);
                                itemUI.name = weaponItem.itemName;

                                //  이미지 할당(이미지가 자식인 itemicon에 있기 때문에)
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

                                //  드래그 정보 할당
                                DragItem newDrag = itemUI.GetComponent<DragItem>();
                                if (newDrag != null)
                                {
                                    newDrag.item = weaponItem;
                                    newDrag.slot = i;

                                    //  꼭 해줘야 드래그됨
                                    CanvasGroup cg = itemUI.GetComponent<CanvasGroup>();
                                    if (cg != null)
                                        cg.blocksRaycasts = true;
                                }

                                //  툴팁 정보 연결
                                InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                                if (uiScript != null)
                                    uiScript.itemData = weaponItem;

                                //  ShopManager.items도 동기화 (중요!!)
                                shop.items[i] = weaponItem;

                                break;
                            }
                        }


                        Destroy(weaponItemObj); // 기존 장비창의 UI 삭제
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
            Debug.Log($"[InventoryManager] 슬롯 {slotIndex} 아이템 제거됨");
        }
    }

    public void RequestRemoveItem(int slotIndex, GameObject targetItemObject)
    {   
        DragItem dragItem = targetItemObject.GetComponent<DragItem>();
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject popupInstance = Instantiate(PopupForSellPrefab, canvas.transform);
        Debug.Log("삭제 확인 팝업 생성됨");

        ShopItem item = dragItem.item;

        // 텍스트 설정
        Text messageText = popupInstance.transform.Find("Message").GetComponent<Text>();
        messageText.text = $"판매하시겠습니까?\n({(int)(item.price * 0.7)} Gold)";

        // 버튼 연결
        Button yesBtn = popupInstance.transform.Find("YesButton").GetComponent<Button>();
        Button noBtn = popupInstance.transform.Find("NOButton").GetComponent<Button>();

        yesBtn.onClick.AddListener(() =>
        {
            if (dragItem != null)
            {
                ShopItem item = dragItem.item;

                if (item.IfEquip == 1)
                {
                    Debug.Log($"{item.itemName}을 인벤토리에서 판매 확인");

                    
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
                        Debug.LogError("shopManager가 null입니다! 연결 확인 요망.");
                    }*/
                    if (slotIndex >= 0 && slotIndex < playerInventory.playerItems.Count)
                    {   
                        playerInventory.playerItems[slotIndex] = new ShopItem();
                        playerInventory.playerItems[slotIndex].id = -1;
                    }
                }
                else if(item.IfEquip == 2) 
                {
                    Debug.Log($"{item.itemName}을 장비창에서 판매 확인");
                    playerInventory.currentPlayerItems.Remove(item);

                    GameManager.instance.playerMoney += (int)(item.price * 0.7);
                    /*
                    if (shopManager != null)
                    {
                        shopManager.UpdatePlayerMoneyUI();
                    }
                    else
                    {
                        Debug.LogError("shopManager가 null입니다! 연결 확인 요망.");
                    }*/
                    if (slotIndex >= 0 && slotIndex < playerInventory.currentPlayerItems.Count)
                    {
                        playerInventory.currentPlayerItems[slotIndex] = new ShopItem();
                        playerInventory.currentPlayerItems[slotIndex].id = -1;
                    }
                }

                // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x
                item.MarkAsSold();      // 상점 재등장 ?? 오류 테스트중
                // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x

                Destroy(targetItemObject);
                Destroy(popupInstance);
                ShopManager.instance.UpdatePlayerMoneyUI();
            }
            else
            {
                Debug.LogError("DragItem 컴포넌트를 찾을 수 없습니다.");
            }
           
            
        });

        noBtn.onClick.AddListener(() =>
        {
            Destroy(popupInstance);
            Debug.Log("삭제 취소됨");
        });
    }

    public void RebuildInventoryUI() //씬 넘어와도 인벤토리 ui가 다시 보이게

    {
        // 1. 기존 슬롯 초기화
        for (int i = 0; i < ShopManager.instance.slots.Count; i++)
        {
            Transform slot = ShopManager.instance.slots[i].transform;
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        // 2. 인벤토리 아이템 복원 (일반 아이템: playerItems)
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

        // 3. 장착 아이템 복원 (장비창: currentPlayerItems)
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

            // 슬롯이 비어있지 않다면 삭제
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }

        // 인벤토리 아이템 복원
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

        // 장착 아이템 복원
        for (int i = 0; i < playerInventory.currentPlayerItems.Count; i++)
        {
            ShopItem item = playerInventory.currentPlayerItems[i];
            if (item != null && item.id != -1)
            {
                // 장비 슬롯 index 찾기
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
        Debug.Log($"[InventoryManager] {item.itemName} 인벤토리에 추가됨. 현재 개수: {playerInventory.playerItems.Count}");*/

    // 슬롯 UI에 아이템도 추가하려면 여기서 Instantiate(itemUI) 해주기
}
/*
public static InventoryManager InventoryInstance;
public ScrollRect scrollRect; // 스크롤뷰
//public GameObject itemPrefab; // 아이템 프리팹
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
        Debug.LogWarning("이미 인벤토리 매니져 존재");
        Destroy(gameObject);
    }
}
void Start()
{
    // 스크롤뷰 설정
    if (scrollRect == null)
    {
        scrollRect = GameObject.Find("ScrollRect").GetComponent<ScrollRect>();
        //처음 타이틀 화면부터 있을것이니까 스크롤뷰를 넣는것도 
    }
    dragLayer = GameObject.Find("Equipment").transform;
}
public void AddItem(ShopItem item)
{
    int j = 0;
    inventory.Add(item);
    /*
    Debug.Log($"{item.itemName}을(를) 인벤토리에 추가했습니다.");
    Debug.Log("현재 인벤토리:");
    foreach (ShopItem i in inventory)
    {
        Debug.Log(i.itemName);
        j++;
    }
    Debug.Log(j);
    // 인벤토리 UI 업데이트 코드 추가
    AddItemToScrollRect(item);
}
private void AddItemToScrollRect(ShopItem item)
{
    // 스크롤뷰의 Content 객체

    GameObject contentObject = scrollRect.content.gameObject;


    // 아이템 이름 텍스트 생성
    GameObject itemNameObject = new GameObject("ItemName");
    itemNameObject.transform.SetParent(contentObject.transform);
    Text itemNameText = itemNameObject.AddComponent<Text>();
    itemNameText.text = item.itemName;
    itemNameText.fontSize = 24;
    itemNameText.alignment = TextAnchor.MiddleCenter;

    // 아이템 아이콘 이미지 생성
    GameObject itemIconObject = new GameObject("ItemIcon");
    itemIconObject.transform.SetParent(contentObject.transform);
    Image itemIconImage = itemIconObject.AddComponent<Image>();
    itemIconImage.sprite = item.itemIcon;
    itemIconImage.preserveAspect = true;

    //아이템 가격
    GameObject itemPriceObject = new GameObject("Price");
    itemPriceObject.transform.SetParent(contentObject.transform);
    Text itemPriceText = itemPriceObject.AddComponent<Text>();
    itemPriceText.text = $"Price: {item.price}";
    itemPriceText.fontSize = 24;
    itemPriceText.alignment = TextAnchor.MiddleCenter;

    // 레이아웃 설정 (예: 수평 레이아웃)
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
    itemButton.onClick.AddListener(() => OnItemButtonClicked(item)); // 인벤토리의 아이템에 버튼 구현



    Image imageComponent = itemLayoutObject.AddComponent<Image>();
    imageComponent.color = Color.white;

    DragItem dragItem = itemLayoutObject.AddComponent<DragItem>();
    dragItem.onDragParent = dragLayer; //하이어라키에 equipment에 slot역할 부여

    CanvasGroup canvasGroup = itemLayoutObject.AddComponent<CanvasGroup>();

}


public void RemoveItem(ShopItem item)
{
    inventory.Remove(item);
    Debug.Log($"{item.itemName}을(를) 인벤토리에서 제거했습니다.");
    // 인벤토리 UI 업데이트 코드 추가해야 됨
}

private void OnItemButtonClicked(ShopItem item)
{
    // 아이템 버튼 클릭 시 수행할 행동
    Debug.Log($"{item.itemName} 버튼 클릭!");

}*/




