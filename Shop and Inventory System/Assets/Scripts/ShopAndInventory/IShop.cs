using System.Collections.Generic;
using UnityEngine;

public interface IShop
{
    List<PlayerStats.FItemData> BuyingItems {get; set; }
    List<int> ShopLevels { get; set; }
    string ShopName { get; set; }

    bool Buy(List<InventorySlot> itemsToSell, PlayerStats sellerStats);
    bool Sell(List<InventorySlot> itemsToSell, PlayerStats sellerStats);
    void InitializeShop(Item.type type, IShop shop);
}