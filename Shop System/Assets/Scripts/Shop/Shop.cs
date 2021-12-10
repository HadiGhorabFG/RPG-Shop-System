using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IShop, IInteractableUI
{
    public delegate void OnEnterStoreRange(string message);
    public static OnEnterStoreRange onEnterStoreRange;
    
    public delegate void OnExitStoreRange();
    public static OnExitStoreRange onExitStoreRange;
    
    public List<Item> BuyingItems { get; set; }
    public List<int> ShopLevels { get; set; }

    private IShop activeShop;

    protected bool canOpenShop = false;

    private void Awake()
    {
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
            }
        }
    }

    public virtual void Sell(List<Item> itemsToBuy, PlayerStats sellerStats)
    {
        foreach (var item in itemsToBuy)
        {
            if(sellerStats.RemoveItemFromInventory(item))
            {
                sellerStats.money += item.baseSellValue;
            }
        }
    }

    public virtual void InitializeShop(Item.type type, IShop activeShop)
    {
        AddItemsToShop(type, ShopLevels);
        this.activeShop = activeShop;
    }

    public virtual void OpenMenu(GameObject shopUI)
    {
        if (shopUI.activeSelf == false)
        {
            shopUI.SetActive(true);
            UpdateShopUI(activeShop);
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
            onEnterStoreRange.Invoke("Press 'Space' to open shop!");
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
    }

    private void UpdateShopUI(IShop activeShop)
    {
        ShopUI.Instance.SetCurrentShop(activeShop);
    }
}
