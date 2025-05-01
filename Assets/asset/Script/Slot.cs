using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Slot : MonoBehaviour, IDropHandler
{
    public int id;
    private ShopManager inv;
    

    void Start()
    {
        
        inv = GameObject.Find("ShopManager").GetComponent<ShopManager>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Debug.LogWarning("OnDrop: pointerDrag�� null�Դϴ�.");
            return;
        }
        DragItem droppedItem = eventData.pointerDrag.GetComponent<DragItem>(); //pointerDrag**��� �ʵ�� ���� �巡�׵ǰ� �ִ� ���� ������Ʈ�� ��Ÿ��
        if (droppedItem == null)
        {
            Debug.LogWarning("OnDrop: DragItem ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }
        if (this.tag.Contains("Equip"))
        {
            EquipType slotType = GetSlotEquipTypeFromTag(this.tag);
            if (droppedItem.item.equipType != slotType)
            {
                Debug.LogWarning($"[Slot] ��� Ÿ�� ����ġ! {droppedItem.item.equipType} �������� {slotType} ���Կ� ���� �Ұ�.");
                droppedItem.ResetToOriginalSlot(); //  ���� ó�� (DragItem.cs�� �߰��ؾ� ��)
                return;
            }
        }
        if (inv.items[id].id == -1)
        {
            Debug.Log("OnDrop�޼���"); //slot.cs �� �ִ� ������Ʈ���� �� �޼��尡 ���� �Ѵ�..?
            /*droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;*/

            inv.items[droppedItem.slot] = new ShopItem(); //���⼭ �����ִ����� �ٽ� -1������ �Ѵ�.
            inv.items[droppedItem.slot].id = -1;
            inv.items[id] = droppedItem.item;
            droppedItem.slot = id; //slot ���� ���̵� �ְ� �� ������ ���̵� �ְ� �Ǵ� ����ΰ� ����.
            
            if (this.tag.Contains("Equip"))
            {   
                InventoryManager.InventoryInstance.EquipItem(droppedItem.item);
                //Debug.Log($"{droppedItem.item.itemName}�� ���â �������� �̸�");
                //InventoryManager.InventoryInstance.UnequipItem(droppedItem.item);
            }
            else //else�̸� ���� ���ԵȰ��� �κ��丮 ��
            {
                InventoryManager.InventoryInstance.UnequipItem(droppedItem.item);
            }


        }
        else
        {
            if (this.transform.childCount > 0)
            {
                Transform targetItemTransform = this.transform.GetChild(0);
                DragItem targetItem = targetItemTransform.GetComponent<DragItem>();

                int fromSlot = droppedItem.slot; // �巡���ϴ� �������� ���� ����
                int toSlot = id; // ���� ��ӵ� ����

                // 1. ���� ���� �ٲٱ�
                droppedItem.slot = toSlot;
                targetItem.slot = fromSlot;

                // 2. ���� ������ ��ȯ
                ShopItem tempItem = inv.items[toSlot];
                inv.items[toSlot] = inv.items[fromSlot];
                inv.items[fromSlot] = tempItem;

                // 3. ��ġ ���ġ
                targetItemTransform.SetParent(inv.slots[fromSlot].transform);
                targetItemTransform.position = inv.slots[fromSlot].transform.position;

                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;
            }
        }/*
        else
        {
            if (this.transform.childCount > 0)
            {


                Transform item = this.transform.GetChild(0);
                item.GetComponent<DragItem>().slot = droppedItem.slot;
                item.transform.SetParent(inv.slots[droppedItem.slot].transform);
                item.transform.position = inv.slots[droppedItem.slot].transform.position;

                droppedItem.slot = id;
                droppedItem.transform.SetParent(this.transform);
                droppedItem.transform.position = this.transform.position;

                inv.items[droppedItem.slot] = item.GetComponent<DragItem>().item;
                inv.items[id] = droppedItem.item;

            }
        }*/
    }
    private EquipType GetSlotEquipTypeFromTag(string tag) // �� �޼��尡 ����
    {
        if (tag.Contains("Head")) return EquipType.Head;
        if (tag.Contains("Top")) return EquipType.Top;
        if (tag.Contains("Bottom")) return EquipType.Bottom;
        if (tag.Contains("Gloves")) return EquipType.Gloves;
        if (tag.Contains("Shoes")) return EquipType.Shoes;
        if (tag.Contains("Weapon")) return EquipType.Weapon;
        if (tag.Contains("Accessory")) return EquipType.Accessory;
        if (tag.Contains("Special")) return EquipType.Special;
        return EquipType.None;
    }

}
