using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopItem itemData; // ������ ����
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
        //  ShopManager���� tooltipPanel �������� (���� ����)
        if (ShopManager.instance == null)
        {
            Debug.LogError(" ShopManager �ν��Ͻ��� ã�� �� �����ϴ�!");
            return;
        }

        tooltipPanel = ShopManager.instance.tooltipPanel;

        if (tooltipPanel == null)
        {
            Debug.LogError(" tooltipPanel�� ã�� �� �����ϴ�! ShopManager���� �����Ǿ����� Ȯ���ϼ���.");
            return;
        }

        tooltipText = tooltipPanel.transform.Find("TooltipText")?.GetComponent<Text>();
        //
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        //
        if (tooltipText == null)
        {
            Debug.LogError(" TooltipText�� ã�� �� �����ϴ�! ������Ʈ ���� ������ Ȯ���ϼ���.");
            return;
        }

        //
        // Canvas�� RectTransform ã��
        canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
        //

        // ������ �� ���� �����
        tooltipPanel.SetActive(false);
    }

    // ���콺�� �÷��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel == null || tooltipText == null) return; // ���� ����

        tooltipPanel.SetActive(true); // ���� ���̱�
        tooltipText.text = $"{itemData.itemName}\n����: {itemData.price}\n{GetItemDescription()}";

        //
        UpdateTooltipPosition(eventData);
        //

        // ���� ��ġ ���� (���콺 ���󰡱�)
        //tooltipPanel.transform.position = Input.mousePosition;
    }

    // ���콺�� ���� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel == null) return; // ���� ����
        tooltipPanel.SetActive(false); // ���� �����
    }

    private void UpdateTooltipPosition(PointerEventData eventData)
    {
        if (tooltipPanel == null || canvasRect == null) return;

        // ���콺 ������ ��ġ�� ĵ���� ���� ��ǥ�� ��ȯ
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, null, out localPoint);

        // ���� ��ġ ����
        tooltipRect.anchoredPosition = localPoint + new Vector2(10, -10); // �ణ�� ������ �߰�

        // ������ ȭ�� ������ ������ �ʵ��� ����
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        //������ 4�� �𼭸� ��ǥ
        float minX = corners[0].x;  //���� �Ʒ� x
        float maxX = corners[2].x;  //������ �� x
        float minY = corners[0].y;  //���� �Ʒ� y
        float maxY = corners[2].y;  //������ �� y

        //ȭ�� ũ�� ��������
        float screenWidth = Screen.width;   //ȭ�� ���� ����
        float screenHeight = Screen.height; // ȭ�� ���� ����

        Vector3 position = tooltipRect.position;    //���� ������ ��ġ ����

        if (maxX > screenWidth)
            position.x -= maxX - screenWidth;   // maxX�� ȭ��ʺ񺸴� ũ�� maxX - screenWidth��ŭ ���� �̵� (���������� ����� ���� �̵�)
        if (minX < 0)
            position.x -= minX;                 // ���� ���� �������� ����� ���������� �̵� 

        if (minY < 0)
            position.y -= minY;                 // �Ʒ� ����� ���� �̵�
        if (maxY > screenHeight)
            position.y -= maxY - screenHeight;  // �� ����� �Ʒ��� �̵�

        tooltipRect.position = position;        // ���ο� ��ġ ����
    }


    // ������ ���� ��ȯ (������ ���� ���ٸ� �⺻�� ����)
    private string GetItemDescription()
    {
        switch (itemData.itemName)
        {
            case "qwer":
                return "ü���� ȸ���ϴ� �������Դϴ�.";
            case "aaa":
                return "���ݷ��� ������ŵ�ϴ�.";
            case "abc":
                return "������ ������ŵ�ϴ�.";
            default:
                return "�� �����ۿ� ���� ������ �����ϴ�.";
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
