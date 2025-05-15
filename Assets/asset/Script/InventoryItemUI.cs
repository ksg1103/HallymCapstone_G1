using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopItem itemData;
    private GameObject tooltipPanel;
    private Text tooltipText;
    private Image tooltipImage;
    private Text tooltipName;
    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    private Text tooltipAttackAt;
    private Transform tooltipPart;
    private Dictionary<EquipType, string> equipTypeToTextName;
    
    void Start()
    {
        tooltipPanel = ShopManager.instance.tooltipPanel;
        tooltipText = tooltipPanel.transform.Find("TooltipText")?.GetComponent<Text>();
        tooltipImage = tooltipPanel.transform.Find("TooltipImage")?.GetComponent<Image>();
        tooltipName = tooltipPanel.transform.Find("TooltipName")?.GetComponent<Text>();
        tooltipAttackAt = tooltipPanel.transform.Find("TooltipAttackAt")?.GetComponent<Text>();
        tooltipPart = tooltipPanel.transform.Find("TooltipPart");
        canvasRect = tooltipPanel.transform.parent.GetComponent<RectTransform>();
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();

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

        tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData == null) return;
        tooltipPanel.SetActive(true);
        tooltipName.text = $"{itemData.itemName}";
        tooltipText.text = GetItemDescription();
        tooltipAttackAt.text = $"Bleeding: {itemData.bleeding}   Curse: {itemData.curse}   Burn: {itemData.burn}\n" +
                               $"Blind: {itemData.blind}   Holy: {itemData.holy}";
        tooltipImage.sprite = itemData.itemIcon;
        tooltipPanel.transform.SetAsLastSibling();

        HighlightEquipPart(itemData.equipType);
        UpdateTooltipPosition(eventData);
    }
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

            default:
                return "�� �����ۿ� ���� ������ �����ϴ�.";
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }

    private void UpdateTooltipPosition(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, null, out localPoint);
        tooltipRect.anchoredPosition = localPoint + new Vector2(10, -10);
        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[2].y;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector3 position = tooltipRect.position;

        if (maxX > screenWidth) position.x -= maxX - screenWidth;
        if (minX < 0) position.x -= minX;
        if (minY < 0) position.y -= minY;
        if (maxY > screenHeight) position.y -= maxY - screenHeight;

        tooltipRect.position = position;
    }

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
}