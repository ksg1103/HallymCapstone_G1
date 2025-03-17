using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopItem itemData; // 아이템 정보
    private GameObject tooltipPanel;
    private Text tooltipText;
    //
    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    //
    public GameObject Inventory;
    
    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //  ShopManager에서 tooltipPanel 가져오기 (오류 방지)
        if (ShopManager.instance == null)
        {
            Debug.LogError(" ShopManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        tooltipPanel = ShopManager.instance.tooltipPanel;

        if (tooltipPanel == null)
        {
            Debug.LogError(" tooltipPanel을 찾을 수 없습니다! ShopManager에서 설정되었는지 확인하세요.");
            return;
        }

        tooltipText = tooltipPanel.transform.Find("TooltipText")?.GetComponent<Text>();
        //
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        //
        if (tooltipText == null)
        {
            Debug.LogError(" TooltipText를 찾을 수 없습니다! 오브젝트 계층 구조를 확인하세요.");
            return;
        }

        //
        // Canvas의 RectTransform 찾기
        canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
        //

        // 시작할 때 툴팁 숨기기
        tooltipPanel.SetActive(false);
    }

    // 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel == null || tooltipText == null) return; // 예외 방지

        tooltipPanel.SetActive(true); // 툴팁 보이기
        tooltipText.text = $"{itemData.itemName}\n가격: {itemData.price}\n{GetItemDescription()}";

        //
        UpdateTooltipPosition(eventData);
        //

        // 툴팁 위치 조정 (마우스 따라가기)
        //tooltipPanel.transform.position = Input.mousePosition;
    }

    // 마우스를 뗐을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel == null) return; // 예외 방지
        tooltipPanel.SetActive(false); // 툴팁 숨기기
    }

    private void UpdateTooltipPosition(PointerEventData eventData)
    {
        if (tooltipPanel == null || canvasRect == null) return;

        // 마우스 포인터 위치를 캔버스 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, null, out localPoint);

        // 툴팁 위치 설정
        tooltipRect.anchoredPosition = localPoint + new Vector2(10, -10); // 약간의 오프셋 추가

        // 툴팁이 화면 밖으로 나가지 않도록 조정
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        //툴팀의 4개 모서리 좌표
        float minX = corners[0].x;  //왼쪽 아래 x
        float maxX = corners[2].x;  //오른쪽 위 x
        float minY = corners[0].y;  //왼쪽 아래 y
        float maxY = corners[2].y;  //오른쪽 위 y

        //화면 크기 가져오기
        float screenWidth = Screen.width;   //화면 가로 길이
        float screenHeight = Screen.height; // 화면 세로 길이

        Vector3 position = tooltipRect.position;    //현재 툴팀의 위치 저장

        if (maxX > screenWidth)
            position.x -= maxX - screenWidth;   // maxX가 화면너비보다 크면 maxX - screenWidth만큼 왼쪽 이동 (오른쪽으로 벗어나면 왼쪽 이동)
        if (minX < 0)
            position.x -= minX;                 // 위와 같음 왼쪽으로 벗어나면 오른쪽으로 이동 

        if (minY < 0)
            position.y -= minY;                 // 아래 벗어나면 위로 이동
        if (maxY > screenHeight)
            position.y -= maxY - screenHeight;  // 위 벗어나면 아래로 이동

        tooltipRect.position = position;        // 새로운 위치 적용
    }


    // 아이템 설명 반환 (설명이 따로 없다면 기본값 설정)
    private string GetItemDescription()
    {
        switch (itemData.itemName)
        {
            case "qwer":
                return "체력을 회복하는 아이템입니다.";
            case "aaa":
                return "공격력을 증가시킵니다.";
            case "abc":
                return "방어력을 증가시킵니다.";
            default:
                return "이 아이템에 대한 설명이 없습니다.";
        }
    }

    public void OnButtonClicked()
    {
        Inventory.SetActive(!Inventory.activeSelf);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
