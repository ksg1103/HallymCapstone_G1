using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public EquipType equipType; // ��� Ÿ�� ��������
    public int IfEquip = 0; //�κ��丮�� ������ 1, ���â�� ������ 2, ����Ʈ�� 0

    public int id;
    public string itemName;  // ������ �̸�
    public Sprite itemIcon;  // ������ ������
    public int price;        // ������ ����
    public int HP = 0;
    public int bleeding = 0;
    public int curse = 0;
    public int burn = 0;
    public int blind = 0;
    public int holy = 0;
    public int bang = 0;        // ���� ����





    // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x

    public int count = 1; // �⺻��: 1�̸� ������ ǥ�õ�

    public bool IsAvailableInShop()
    {
        return count > 0;
    }

    public void MarkAsBought()
    {
        count = 0;
    }

    public void MarkAsSold()
    {
        count = 1;
    }

    // ������ ������ �������� ���� ���ϰ� �ϴ� ���� ���� x

}