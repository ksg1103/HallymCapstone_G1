using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{   
    public ShopItem item;
    public int amount;
    public int slot;

    private Transform originalParent;
    private Vector3 originalPosition;
    private ShopManager inv;
    private Vector2 offset;

    

    void Start()
    {
        inv = GameObject.Find("ShopManager").GetComponent<ShopManager>();
    }
    public void OnBeginDrag(PointerEventData eventData) //�巡�׸� �����Ҷ� �߻��ϴ� �޼���, �̰͸� ������ �� ������������ �巡�� �ǰ� �ܼ�â�� �����޼��� �߻�, �������Ӿ��� ������ �� �ִ�(����°���)
    {
        eventData.pointerDrag = gameObject;
        if (item != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y); //offset�� ����Ͽ� �������� Ŭ���Ҷ� Ƣ�°��� �����Ѵ�.
            originalParent = this.transform.parent; //inv = GameObject.Find("Inventory").GetComponent<Inventory>(); �� �� �߰��� �ʿ䰡 ��������.
            originalPosition = this.transform.position;
            this.transform.SetParent(this.transform.parent.parent);
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData) //�巡�װ� ����ɶ� ����Ǵ� �޼���
    {   
        

        this.transform.SetParent(inv.slots[slot].transform);
        //this.transform.SetParent(originalParent); //�巡�װ� ����Ǹ� �����ִ� ���̾ƿ�(���̾��Űâ���� �����ִ� ����)���� ���ư�����
        this.transform.position = inv.slots[slot].transform.position;
        //this.transform.position = originalParent.position; // �� �������� �巡�װ� ������ ���� �ִ������� ���ư��� �Ѵ�.
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 ������ Ŭ�� ����
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            
            //Debug.Log($"[DragItem] {item.itemName} ������ ���ŵ� (������ Ŭ��)");

            // InventoryManager���� ������ ���� ����
            InventoryManager.InventoryInstance.RequestRemoveItem(slot, this.gameObject);

            // ������Ʈ ����
            //Destroy(this.gameObject);
        }
    }
    public void ResetToOriginalSlot()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
