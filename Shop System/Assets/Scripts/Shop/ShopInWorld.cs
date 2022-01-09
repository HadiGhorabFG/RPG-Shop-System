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
    
    public List<Item> BuyingItems { get; set; }
    public List<int> ShopLevels { get; set; }

    protected bool canOpenShop = false;

    private SortItemsWithPrice sortItemsWithPrice;

    private void Awake()
    {
        sortItemsWithPrice = GetComponent<SortItemsWithPrice>();
        BuyingItems = new List<Item>();
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
        BuyingItems = sortItemsWithPrice.SortBuyPrice(BuyingItems);
        //this.activeShop = activeShop;
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
        if (levelValues.Contains(1))
        {
            for (int i = 0; i < ItemsManager.Instance.levelOneItems.Count; i++)
            {
                if (ItemsManager.Instance.levelOneItems[i].Type == typeValue)
                {
                    BuyingItems.Add(ItemsManager.Instance.levelOneItems[i]);
                }
            }
        }
        
        if (levelValues.Contains(2))
        {
            for (int i = 0; i < ItemsManager.Instance.levelTwoItems.Count; i++)
            {
                if (ItemsManager.Instance.levelTwoItems[i].Type == typeValue)
                {
                    BuyingItems.Add(ItemsManager.Instance.levelTwoItems[i]);
                }
            }
        }
        
        if (levelValues.Contains(3))
        {
            for (int i = 0; i < ItemsManager.Instance.levelThreeItems.Count; i++)
            {
                if (ItemsManager.Instance.levelThreeItems[i].Type == typeValue)
                {
                    BuyingItems.Add(ItemsManager.Instance.levelThreeItems[i]);
                }
            }
        }
    }
    
    private void UpdateShopUI(IShop activeShop)
    {
        ShopUI.Instance.SetCurrentShop(activeShop);
    }
}
