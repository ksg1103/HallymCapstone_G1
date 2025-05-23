using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopItem itemData; // ������ ����
    private GameObject tooltipPanel;
    private Text tooltipText;
    //
    private Image tooltipImage;
    //
    private Text tooltipName;
    //
    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    //
    private Text tooltipAttackAt;
    //
    public GameObject Inventory; //�κ� â

    //
    private Transform tooltipPart;
    private Dictionary<EquipType, string> equipTypeToTextName;
    //
    private bool isPointerOverTooltip = false;
    private bool isPointerOverItem = false;
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
        tooltipImage = tooltipPanel.transform.Find("TooltipImage")?.GetComponent<Image>();

        if (tooltipImage == null)
        {
            Debug.LogError(" tooltipImage ã�� �� �����ϴ�! ������Ʈ ���� ������ Ȯ���ϼ���.");
            return;
        }
        //

        ////    ������ �̸� ����
        tooltipName = tooltipPanel.transform.Find("TooltipName")?.GetComponent<Text>();

        if (tooltipName == null)
        {
            Debug.LogError(" tooltipName ã�� �� �����ϴ�! ������Ʈ ���� ������ Ȯ���ϼ���.");
            return;
        }
        ///

        ////    ������ �Ӽ� ���ݷ� ����
        tooltipAttackAt = tooltipPanel.transform.Find("TooltipAttackAt")?.GetComponent<Text>();

        if (tooltipAttackAt == null)
        {
            Debug.LogError(" tooltipAttackAt ã�� �� �����ϴ�! ������Ʈ ���� ������ Ȯ���ϼ���.");
            return;
        }
        ///

        ///     ������ ��� ���� ����
        tooltipPart = tooltipPanel.transform.Find("TooltipPart");
        equipTypeToTextName = new Dictionary<EquipType, string>
        {
            { EquipType.Head, "HeadText" },
            { EquipType.Top, "TopText" },
            { EquipType.Bottom, "BottomText" },
            { EquipType.Gloves, "GlovesText" },
            { EquipType.Shoes, "ShoesText" },
            { EquipType.Weapon, "WeaponText" },
            { EquipType.Accessory, "AccessoryText" },
            { EquipType.Special, "SpecialText" }
        };


        //
        // Canvas�� RectTransform ã��
        canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
        //

        // ������ �� ���� �����
        tooltipPanel.SetActive(false);



        //
        //AddTooltipEventTriggers();
        //

    }

    // ���콺�� �÷��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        //
        isPointerOverItem = true;
        //



        if (tooltipPanel == null || tooltipText == null || tooltipImage == null || tooltipName == null) return; // ���� ����

        tooltipPanel.SetActive(true); // ���� ���̱�
        tooltipText.text = $"{GetItemDescription()}";
        tooltipName.text = $"{itemData.itemName}";
        tooltipAttackAt.text = $"Bleeding: {itemData.bleeding}   Curse: {itemData.curse}   Burn: {itemData.burn}\n" +
                                $"Blind: {itemData.blind}   Holy: {itemData.holy}";
        


        tooltipImage.sprite = itemData.itemIcon;
        tooltipPanel.transform.SetAsLastSibling();
        // ��� ui���� ���� �Լ� ȣ��
        HighlightEquipPart(itemData.equipType);
        UpdateTooltipPosition(eventData);
    }

    // ���콺�� ���� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOverItem = false;
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
            case "�����ǻ� ����ũ":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "�����ǻ� �Ź�":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "�����ǻ� �ǻ�":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "�����ǻ� �尩":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "�����ǻ� ��Ʈ":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "Ȧ����":
                return "������ ��������� �ִ� Ȧ���� �Դϴ�.";
            case "��������":
                return "�̸� ���� ������ ź�ָӴ��Դϴ�.";
            case "Dear Boss":
                return "�������� ���θ�, �� �� ���۰� ��ȸ�� �ڽ��� �Ұ��ϴ� �����Դϴ�.";
            case "From Hell":
                return "�������� ���θ�, ��� ���۰� ��ȸ�� ������ 2��° �����Դϴ�.";

            //
            case "����":
                return "���÷��� 4����� ������ ����ϴ� ��簡 ������ϴ�.";
            case "���� ����":
                return "�������� �ǿ� ������ �����Դϴ�.";
            case "�䳢�� ����":
                return "�������� �䳢�� ���� �����Դϴ�.";
            case "����":
                return "���÷��� 4����� ������ ����ϴ� ��簡 ������ϴ�.";
            case "���":
                return "���÷��� 4����� ����� ����ϴ� ��簡 ������ϴ�.";
            case "����":
                return "���÷��� 4����� ������ ����ϴ� ��簡 ������ϴ�.";
            case "�䳢�� ����":
                return "�������� �䳢�� �Դ� �����Դϴ�.";
            case "�ȷ�Ȩ���� ����":
                return "������ Ž�� �ȷ�Ȩ� ���� �����Դϴ�.";
            case "�ø�Ʈ�� ��Ŷ":
                return " ������ ���� �ӽ�Ŷ�Դϴ�. �������� ������ �����ֽ��ϴ�.";
            case "�ż������ ��":
                return "�����ڵ��� ���� �㸮 ���Դϴ�.";
            case "�������ν�":
                return "�������� ������ ��������, �ÿ��� ���� �Ź��Դϴ�.";
            case "ĳ�����Ź�":
                return "ü�����󿡼� ���� �Ź��Դϴ�.";
            case "������ũ�� ����":
                return "������ �ǹ̰� �����Ǿ� ���� �����Դϴ�.";
            case "���� �Ͱ���":
                return "�׹������� ������ ���� ���з� ���� �Ͱ����Դϴ�.";
            case "���� ��ȭ���� �ܺ�":
                return "���� ��ȭ�翡�� �߰ߵ� ������ �ʴ� �ܺ��Դϴ�.";
            case "������ ��":
                return "������ ������ �ִ�, �Ҵ�Ʈ�Դϴ�.";
            case "�ȷ�Ȩ���� ���":
                return "������ Ž�� �ȷ�Ȩ� �Ǵ� ����Դϴ�.";
            case "�ȷ�Ȩ���� ������":
                return "������ Ž�� �ȷ�Ȩ� ���� �������Դϴ�.";
            case "�����ǻ��� ����ָӴ�":
                return "��纴�� ���� ����, ������� �츮�� ����ϴ� ������� �����Դϴ�.";
            case "���Ͽ�ġ":
                return "�䳢�� ���Ͽ�ġ�Դϴ�.";
            case "ǳ���� ����":
                return "ǳ�ڷ� �����ϴ� ������ �����Դϴ�, �� ������ �� ���� ���� �ḻ�� �¾ҽ��ϴ�.";
            case "��ũ ������ ���ġ":
                return "���� ���� ��ũ������ ������ �ִ� ���ġ�Դϴ�.";
            case "�ȷ�Ȩ���� ��Ʈ":
                return "������ Ž�� �ȷ�Ȩ� ��ġ�� ��Ʈ�Դϴ�.";
            case "���Ļ��� �վƱ�":
                return "ü�����󿡼� ���� �尩�Դϴ�.";
            case "������ ��":
                return "���谡 ����ȭ �� ��Ʋ�� �Դϴ�.";
            case "�������ӽ�Ŷ":
                return "������ũ ����� ������ �� �ӽ�Ŷ�Դϴ�, 5���� �������� ��� �ֽ��ϴ�.";
            case "���۹ڽ� �ǽ���":
                return "4���� �������� ��� �ִ� �������� �����Դϴ�.";
            case "��Ŭ��":
                return "�������� ������ ������ �Ҽ� �ְ� ���ִ� ��Ŭ���Դϴ�. �������Ǵ� ź���� �����մϴ�.";
            //
            default:
                return "�� �����ۿ� ���� ������ �����ϴ�.";
        }
    }

    public void OnInventoryButtonClicked()
    {
        Inventory.SetActive(!Inventory.activeSelf);
        

    }


    // ��� ���� ui���� ��ȭ
    private void HighlightEquipPart(EquipType type)
    {
        if (tooltipPart == null) return;

        foreach (var kvp in equipTypeToTextName)
        {
            Transform partTextTransform = tooltipPart.Find(kvp.Value);
            if (partTextTransform != null)
            {
                Text text = partTextTransform.GetComponent<Text>();
                if (text != null)
                {
                    text.color = (kvp.Key == type) ? Color.white : Color.gray;
                }
            }
        }
    }




    // Update is called once per frame
    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)        // ������ �����ϰ� ���̴� ������ �� �۵�
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition) &&               //RectTransformUtility.RectangleContainsScreenPoint : ���콺�� �ش� ���� UI�� �ִ���
                !RectTransformUtility.RectangleContainsScreenPoint(tooltipRect, Input.mousePosition))                                   // �� ù��° ���� ���콺 �ø� ���������� ��ü, �ι�°�� ���� UI����
            {
                tooltipPanel.SetActive(false);
            }
        }
    }
}
