using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public int StageLevel;
    public int playerMoney;

    // �κ��丮 �� ���� ������ �����
    public List<ShopItem> playerItems;
    public List<ShopItem> equippedItems;
}