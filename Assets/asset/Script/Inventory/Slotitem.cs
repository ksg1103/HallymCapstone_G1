using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour, IDropHandler
{   /*
    public void OnDrop(PointerEventData eventData)
    {
        if (DragItem.beingDraggedIcon != null)
        {
            DragItem.beingDraggedIcon.transform.SetParent(transform, false); // worldPositionStays=false
            DragItem.beingDraggedIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        Debug.Log("�巡�� �Ǿ���");
    }
    */
    private Image image;
    private RectTransform rect;

    private void Awake()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// ���콺 ����Ʈ�� ���� ������ ���� ���� ���η� �� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ������ ������ ������ ��������� ����
        image.color = Color.yellow;
    }

    /// <summary>
    /// ���콺 ����Ʈ�� ���� ������ ���� ������ �������� �� 1ȸ ȣ��
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // ������ ������ ������ �Ͼ������ ����
        image.color = Color.white;
    }

    /// <summary>
    /// ���� ������ ���� ���� ���ο��� ����� ���� �� 1ȸ ȣ��
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // pointerDrag�� ���� �巡���ϰ� �ִ� ���(=������)
        if (eventData.pointerDrag != null)
        {
            // �巡���ϰ� �ִ� ����� �θ� ���� ������Ʈ�� �����ϰ�, ��ġ�� ���� ������Ʈ ��ġ�� �����ϰ� ����
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }
}
