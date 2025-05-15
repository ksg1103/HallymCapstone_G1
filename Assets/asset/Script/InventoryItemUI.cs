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
            case "역병의사 마스크":
                return "흑사병이 돌던 시절, 사람들을 살리려 노력하던 사람들의 흔적입니다.";
            case "역병의사 신발":
                return "흑사병이 돌던 시절, 사람들을 살리려 노력하던 사람들의 흔적입니다.";
            case "역병의사 의상":
                return "흑사병이 돌던 시절, 사람들을 살리려 노력하던 사람들의 흔적입니다.";
            case "역병의사 장갑":
                return "흑사병이 돌던 시절, 사람들을 살리려 노력하던 사람들의 흔적입니다.";
            case "역병의사 벨트":
                return "흑사병이 돌던 시절, 사람들을 살리려 노력하던 사람들의 흔적입니다.";
            case "홀스터":
                return "권총을 집어넣을수 있는 홀스터 입니다.";
            case "벤돌리어":
                return "이름 없는 병사의 탄주머니입니다.";
            case "Dear Boss":
                return "전설적인 살인마, 잭 더 리퍼가 사회에 자신을 소개하던 편지입니다.";
            case "From Hell":
                return "전설적인 살인마, 잭더 리퍼가 사회에 보내는 2번째 편지입니다.";

            default:
                return "이 아이템에 대한 설명이 없습니다.";
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