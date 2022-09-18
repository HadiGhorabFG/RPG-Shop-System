using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopInWorld : MonoBehaviour, IShop, IInteractableUI
{
    public delegate void OnEnterStoreRange();
    public static OnEnterStoreRange onEnterStoreRange;
    
    public delegate void OnExitStoreRange();
    public static OnExitStoreRange onExitStoreRange;

    public List<PlayerStats.FItemData> BuyingItems { get; set; }
    public List<int> ShopLevels { get; set; }

    protected bool canOpenShop = false;
    private void Awake()
    {
        BuyingItems = new List<PlayerStats.FItemData>();
    }

    public virtual void Buy(List<Item> itemsToBuy, PlayerStats buyerStats)
    {
        foreach (var item in itemsToBuy)
        {
            if ((buyerStats.money - item.baseBuyValue) < 0)
            {
                break;
            }
            
            if(buyerStats.AddItemToInventory(item))
            {
                buyerStats.money -= item.baseBuyValue;
                UIHandler.Instance.changeMoneyDirtyFlag = true;
            }
        }
    }

    public virtual void Sell(List<Item> itemsToSell, PlayerStats sellerStats)
    {
        foreach (var item in itemsToSell)
        {
            if(sellerStats.RemoveItemFromInventory(item))
            {
                sellerStats.money += item.baseSellValue;
                UIHandler.Instance.changeMoneyDirtyFlag = true;
            }
        } 
    }

    public virtual void InitializeShop(Item.type type, IShop activeShop)
    {
        AddItemsToShop(type, ShopLevels);
    }

    public virtual void OpenMenu(GameObject shopUI)
    {
        if (shopUI.activeSelf == false)
        {
            shopUI.SetActive(true);
            UpdateShopUI(this);
        }
        else
        {
            shopUI.SetActive(false);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        canOpenShop = true;
        
        if (onEnterStoreRange != null)
        {
            onEnterStoreRange.Invoke();
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        canOpenShop = false;

        if (onExitStoreRange != null)
        {
            onExitStoreRange.Invoke();
        }
    }

    private void AddItemsToShop(Item.type typeValue, List<int> levelValues)
    {
        foreach (var level in ShopLevels)
        {
            if (levelValues.Contains(level))
            {
                foreach (var item in ItemsManager.Instance.itemsByLevel[level])
                {
                    if(item.Type != typeValue)
                        continue;
                    
                    BuyingItems.Add(new PlayerStats.FItemData(item, 1));
                }
            }
        }
    }
    
    private void UpdateShopUI(IShop activeShop)
    {
        ShopUI.Instance.SetCurrentShop(activeShop);
    }
}
