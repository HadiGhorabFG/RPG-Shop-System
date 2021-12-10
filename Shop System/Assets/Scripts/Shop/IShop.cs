using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShop
{
    List<Item> BuyingItems {get; set; }
    List<int> ShopLevels { get; set; }
    
    void Buy(List<Item> itemsToBuy, PlayerStats buyerStats);
    void Sell(List<Item> itemsToSell, PlayerStats sellerStats);
    void InitializeShop(Item.type type, IShop shop);
    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);
}
