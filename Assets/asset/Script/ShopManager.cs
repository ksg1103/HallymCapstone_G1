using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public bool isTest= GameManager.instance.isTest; 
    
    public List<ShopItem> allItems;
    public Transform itemParent; // 아이템 UI가 생성될 부모 객체
    public GameObject itemPrefab; // 아이템 UI 프리팹
    private List<ShopItem> currentItems = new List<ShopItem>();

    //
    public static ShopManager instance;
    public GameObject tooltipPanel;
    public Text tooltipText;
    //
    public Button buyButton; //이곳에서 아이템이 인스턴스화 되므로 버튼도 이곳에서 구현해야 할듯

    //
    public Text playerMoneyText;        //플레이어 골드 ui
    //

    //
    public Button reloadButton; // 새로고침 버튼 추가
    private Text reloadButtonText;  // 새로고침 버튼 텍스트
    private int refreshCost = 30;        // 처음 새로고침 비용

    public GameObject purchaseMessagePanel;     // 구매 성공 메시지 팝업
    //
    //
    [Header("인벤 가득찼을때 팝업 프리팹")]
    public GameObject ItemFullPopUp;
    public Transform popupParent; //팝업 나올 위치
    //
    //
    //public Inventory playerInventory;

    [Header("인벤토리 전용 프리팹")]
    public GameObject inventoryItemPrefab;
    [Header("슬롯 설정")]
    public GameObject slotPrefab;   // 슬롯 프리팹
    public int slotCount;      // 몇 개 생성할지
    GameObject inventoryPanel;
    GameObject slotPanel;
    [Header("장비 설정")]
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


    //public GameObject inventorySlot; //slot 프리팹을 넣기 위하여
    public GameObject inventoryItem;

    int slotAmount;
    public List<ShopItem> items = new List<ShopItem>(); //이건 빈 슬롯 알아내기 위해..? inventory에 있는 리스트랑은 다른 역할

    public List<GameObject> slots = new List<GameObject>();


    

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
        //

        if (purchaseMessagePanel != null)
        {
            purchaseMessagePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("purchaseMessagePanel이 연결되지 않았습니다!");
        }

        slotCount = 32;
    }

    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 상점 중복 구매 관련
        if (GameManager.instance.globalItemList == null || GameManager.instance.globalItemList.Count == 0)
        {
            GameManager.instance.globalItemList = allItems; // 최초에만 연결
        }
        // 상점 중복 구매 관련

        allItems = GameManager.instance.globalItemList;
        //
        //Debug.Log(GameManager.instance.StageLevel);
        GenerateShopItems();

        //
        UpdatePlayerMoneyUI();      // 골드 ui업데이트
        //
        inventoryPanel = GameObject.Find("InventoryUI");
        Debug.Log(inventoryPanel);
        slotPanel = inventoryPanel.transform.Find("Right Inventory/Scroll View/Viewport/SlotContent").gameObject; //slotpanel은 동적으로 할당
        CreateSlots();
        CreateEquipSlot();
        //
        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(RefreshShop); // 새로고침 버튼 이벤트 등록
            Debug.Log("새로고침 버튼이 정상적으로 연결됨.");

            // 리로드 버튼 자식 텍스트 가져오기
            reloadButtonText = reloadButton.GetComponentInChildren<Text>();

            if (reloadButtonText != null)
            {
                UpdateReloadButtonText(); // 초기 비용 표시
            }
        }
        else
        {
            Debug.LogError("새로고침 버튼을 찾을 수 없습니다! Hierarchy에서 정확한 이름을 확인하세요.");
        }
        //

        if (InventoryManager.InventoryInstance != null)
        {
            InventoryManager.InventoryInstance.RebuildInventoryUI();
            Debug.Log("[ShopManager] 인벤토리 UI 복원 완료");
        }
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

        currentItems.Clear();

        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x
        // count가 1인 아이템만 상점에 표시
        List<ShopItem> availableItems = allItems.FindAll(item => item.IsAvailableInShop());
        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x

        // 10개 중 6개 랜덤 선택
        //List<ShopItem> shuffledItems = new List<ShopItem>(allItems);
        //for (int i = 0; i < shuffledItems.Count - 1; i++)
        //{
        //    int randIndex = Random.Range(i, shuffledItems.Count);
        //    (shuffledItems[i], shuffledItems[randIndex]) = (shuffledItems[randIndex], shuffledItems[i]);
        //}

        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x
        for (int i = 0; i < availableItems.Count - 1; i++)
        {
            int randIndex = Random.Range(i, availableItems.Count);
            (availableItems[i], availableItems[randIndex]) = (availableItems[randIndex], availableItems[i]);
        }
        // 이 코드 사용할거면 위에 10개중 6개 셔플 주석처리
        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x

        //for (int i = 0; i < 6; i++)
        //{
        //    currentItems.Add(shuffledItems[i]);

        //    // UI 생성
        //    GameObject newItem = Instantiate(itemPrefab, itemParent);
        //    Button button = newItem.GetComponent<Button>();

        //    newItem.transform.Find("ItemName").GetComponent<Text>().text = shuffledItems[i].itemName;
        //    newItem.transform.Find("ItemPrice").GetComponent<Text>().text = shuffledItems[i].price.ToString();
        //    newItem.transform.Find("ItemIcon").GetComponent<Image>().sprite = shuffledItems[i].itemIcon;

        //    //Debug.Log(currentItems[i].itemName);


        //    // ShopItemUI 연결
        //    ShopItemUI itemUI = newItem.AddComponent<ShopItemUI>();
        //    itemUI.itemData = shuffledItems[i];

        //    ShopItem currentItem = shuffledItems[i];
        //    button.onClick.AddListener(() => OnBuyButtonClicked(currentItem, newItem)); //각각의 아이템 정보들이 각각의 버튼에 연동되도록

        //    //button.onClick.AddListener(() => OnBuyButtonClicked(shuffledItems[i]));

        //}


        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x
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
        // 이 코드 사용할거면 위에 for문 주석 처리
        // 구매한 아이템 상점에서 등장 안하게 하는 로직 삭제 x
    }

    //
    // 플레이어 골드 UI 업데이트 함수
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
            items.Add(new ShopItem { id = -1 }); // 빈 슬롯의 ShopItem id 는 -1이다.
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

    public void OnBuyButtonClicked(ShopItem item, GameObject itemObject) //상점 아이템 누를때 일어나는 메서드
    {
        if (item != null)
        {
            if (!InventoryManager.InventoryInstance.ItemFull) //이 if 문이 인벤토리창 가득 찼을때 막는 조건문
            {
                // 아이템 구매 로직
                if (CanAffordItem(item.price))
                {
                    //playerMoney -= item.price;
                    //
                    GameManager.instance.playerMoney -= item.price;
                    //
                    //InventoryManager.InventoryInstance.AddItem(item); // 인벤토리에 아이템 추가

                    item.MarkAsBought(); //  count = 0으로 설정

                    Debug.Log($"{item.itemName}을(를) 클릭하셨읍니다.");
                    Destroy(itemObject); //화면 밑에  아이템 프리팹의 인스턴스는 삭제한다
                                         //ShopItem 이 horizontal layout이라 삭제하면 계속 딸려옴 차라리 위치 지정해서 놔두는게 나을지도?
                                         //InventoryManager.InventoryInstance.playerInventory.AddItem(item);

                    //

                    UpdatePlayerMoneyUI();
                    addItem(item);

                    StartCoroutine(ShowPurchaseMessage());
                    //이제 인벤에 있는 아이템 정보 받아와서 ui만 넘겨주면 될듯?
                    //playerInventory.AddItem(item);
                    //
                }
                else
                {
                    Debug.Log("돈이 부족합니다!");
                }
            }
            else
            {
                Debug.LogWarning("인벤토리 자리가 부족해서 아이템 구매를 못합니다.");
                //여기다가 팝업창을 만들어야함
                if (ItemFullPopUp != null && popupParent != null)
                {
                    GameObject popupInstance = Instantiate(ItemFullPopUp, popupParent);

                    
                    Text text = popupInstance.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.text = "인벤토리가 가득 찼습니다!";
                    }

                    
                    StartCoroutine(DestroyPopupAfterDelay(popupInstance, 0.5f)); //팝업 사라지는 시간 여기서 설정 가능
                }

            }
        }
        else
        {
            Debug.LogError("item is null!");
        }
    }

    private IEnumerator DestroyPopupAfterDelay(GameObject popup, float delay) //팝업 자동적으로 사라지게 코루틴으로 작성
    {
        yield return new WaitForSeconds(delay);
        if (popup != null)
        {
            Destroy(popup);
        }
    }

    public void addItem(ShopItem item) //아이템이 인벤토리에 추가되는 로직, inventory에 정보가 저장되어있다. 따라서 인벤토리용 프리펩을 만들어도 된다.
    {   //3.26 아이템 가격, 이름, 장비 속성 정해야됨
        if (InventoryManager.InventoryInstance.playerInventory != null && item != null)
        {
           
            InventoryManager.InventoryInstance.AddItemToInventory(item); //이 문장으로 inventory 에 아이템 정보가 들어간다.
            Debug.Log($"[ShopManager] {item.itemName} 인벤토리에 추가됨?");
            for (int i = 0; i < slots.Count; i++)
            {
                
                    if (slots[i].transform.childCount == 0)
                    {
                    GameObject itemUI = Instantiate(inventoryItemPrefab, slots[i].transform, false); //itemUI는 inventoryprefab을 인스턴스 시킨것이다.

                    /*Image icon = itemUI.GetComponent<Image>();
                    if (icon != null)
                    {
                        icon.sprite = item.itemIcon;
                    }*/
                    Image icon = itemUI.transform.Find("ItemIcon")?.GetComponent<Image>(); //직접 itemicon 이름 찾아 들어간다.
                    if (icon != null && item.itemIcon != null) //예외처리
                    {
                        icon.sprite = item.itemIcon;
                    }
                    else
                    {
                        Debug.LogWarning($"[ShopManager] 아이콘 적용 실패. icon: {icon}, itemIcon: {item.itemIcon}");
                    }

                    itemUI.name = item.itemName;

                    DragItem dragItem = itemUI.GetComponent<DragItem>();
                    if (dragItem != null)
                    {
                        dragItem.item= item;  //인벤에 아이템 정보 저장..?
                        
                    }

                    //
                    InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                    if (uiScript != null)
                    {
                        uiScript.itemData = item;
                    }
                    //

                    Debug.Log($"[ShopManager] {item.itemName} 인벤토리 슬롯 {i}에 UI로 추가됨");
                    Debug.Log($"[ShopManager] {item.equipType} 의 장비 속성");
                    return;
                }
            }

            Debug.LogWarning("[ShopManager] 빈 슬롯 없음!");

        }
        /*else
        {
            Debug.LogError("playerInventory가 비어있거나 아이템이 null입니다!");
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
            //InventoryManager.InventoryInstance.AddItem(item); // 인벤토리에 아이템 추가
            


            Debug.Log($"{item.itemName}을(를) 구매했습니다!");
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }
    */

    // 새로고침 버튼 클릭 시 실행될 함수
    void RefreshShop()
    {
        //GenerateShopItems(); // 상점 아이템 다시 생성
        if (GameManager.instance.playerMoney >= refreshCost)
        {
            // 돈 차감
            GameManager.instance.playerMoney -= refreshCost;
            UpdatePlayerMoneyUI();

            // 비용 증가 (30 -> 300 -> 3000 -> 30000)
            refreshCost *= 10;

            // 버튼 텍스트 업데이트
            UpdateReloadButtonText();

            // 상점 아이템 다시 생성
            GenerateShopItems();
        }
        else
        {
            Debug.Log("돈이 부족하여 새로고침할 수 없습니다!");
        }
    }

    // 새로고침 버튼의 텍스트 업데이트 함수
    void UpdateReloadButtonText()
    {
        if (reloadButtonText != null)
        {
            reloadButtonText.text = $"Reload ({refreshCost})";
        }
    }

    // 상점 구매 성공 메시지 팝업 함수
    IEnumerator ShowPurchaseMessage()
    {
        if (purchaseMessagePanel != null)
        {
            purchaseMessagePanel.transform.SetAsLastSibling();      // ui가장 앞쪽에 배치
            purchaseMessagePanel.SetActive(true);
            yield return new WaitForSeconds(1f); // 1초 대기
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
        int baseIndex = slotCount; // 장비 슬롯은 slotCount 이후부터 시작

        switch (type)
        {
            case EquipType.Head: return baseIndex;
            case EquipType.Top: return baseIndex + 1;
            case EquipType.Gloves: return baseIndex + 2;
            case EquipType.Weapon: return baseIndex + 3;
            case EquipType.Bottom: return baseIndex + 4;
            case EquipType.Shoes: return baseIndex + 5;
            case EquipType.Accessory: return baseIndex + 6 + accessoryOrder; // 액세서리는 4개
            // case 스페셜 추가해야함
            case EquipType.Special: return baseIndex + 7;
            default: return -1;
        }
    }

}
