using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour //�κ� ���� �����ϴ� �鿣�� ����
{
    

    [Header("�÷��̾� ������ ���")]
    public List<ShopItem> playerItems = new List<ShopItem>(); // ���� ���� ������(�κ��丮)
    public List<ShopItem> currentPlayerItems = new List<ShopItem>(); //���â���� ������ ������(���� �������� ������)



    /*
    public void AddItem(ShopItem itemToAdd)
    {
        Debug.Log($"{slots.Count}InventoryŬ������ Additem �޼��� ����");
        for (int i = 0; i < slots.Count; i++)
        {
            Transform slotTransform = slots[i].transform;
            

            // ���Կ� �������� ������
            if (slotTransform.childCount == 0)
            {
                Debug.Log("����");
                playerItems.Add(itemToAdd); // ���� �����Ϳ� �߰�

                // ������ UI ����
                GameObject itemObj = Instantiate(inventoryItem);
                itemObj.transform.SetParent(slots[i].transform); // ���Կ� �߰�
                itemObj.transform.position = Vector2.zero;

                // ������ �̹��� ����
                Image icon = itemObj.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = itemToAdd.itemIcon;
                }

                // ItemData ������ ���� (����)
                /*
                ItemData itemData = itemObj.GetComponent<ItemData>();
                if (itemData != null)
                {
                    itemData.item = itemToAdd;
                    itemData.slot = i;
                }

                itemObj.name = itemToAdd.itemName;

                Debug.Log($"[�κ��丮] {itemToAdd.itemName} �߰��� (���� {i})");

                break;
            }
        }
    }

    public void RemoveItem(ShopItem item)
    {
        if (playerItems.Contains(item))
        {
            playerItems.Remove(item);
            Debug.Log($"{item.itemName} ���ŵ�");

            // ���Կ����� �����ϱ� (�ɼ�)
            for (int i = 0; i < slots.Count; i++)
            {
                Transform slotTransform = slots[i].transform;
                if (slotTransform.childCount > 0)
                {
                    ItemData data = slotTransform.GetChild(0).GetComponent<ItemData>();
                    if (data != null && data.item == item)
                    {
                        Destroy(slotTransform.GetChild(0).gameObject);
                        break;
                    }
                }
            }
        }
    }*/
}
