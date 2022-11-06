using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopInWorld : MonoBehaviour, IShop, IInteractableUI
{
    public delegate void OnEnterStoreRange();
    public static OnEnterStoreRange onEnterStoreRange;
    
    public delegate void OnExitStoreRange();
    public static OnExitStoreRange onExitStoreRange;

    public List<PlayerStats.FItemData> BuyingItems { get; set; }
    public List<int> ShopLevels { get; set; }
    public string ShopName { get; set; }

    protected bool canOpenShop = false;
    private void Awake()
    {
        BuyingItems = new List<PlayerStats.FItemData>();
    }

    public virtual bool Buy(List<InventorySlot> itemsToBuy, PlayerStats buyerStats)
    {
        bool exceededInventory = buyerStats.Inventory.Count + itemsToBuy.Count > buyerStats.maxInvSlots;
        bool exceededMoney = buyerStats.money < UIStorageController.CalculateTotalPrice(itemsToBuy, true);

        //if items cost/exceed inventory space more than available dont buy any.
        if (exceededMoney || exceededInventory || itemsToBuy.Count <= 0)
            return false;

        foreach (var itemSlot in itemsToBuy)
        {
            //loop through amount to buy
            for (int i = 0; i < itemSlot.GetQuantitySliderValue(); i++)
            {
                if(InventoryManager.Instance.AddItemToInventory(itemSlot.itemData.item))
                {
                    buyerStats.money -= itemSlot.itemData.item.baseBuyValue;
                    UIHandler.Instance.changeMoneyDirtyFlag = true;
                }
            }
        }

        return true;
    }

    public virtual bool Sell(List<InventorySlot> itemsToSell, PlayerStats sellerStats)
    {
        if (itemsToSell.Count <= 0)
            return false;
        
        foreach (var itemSlot in itemsToSell)
        {
            for (int i = 0; i < itemSlot.GetQuantitySliderValue(); i++)
            {
                if(InventoryManager.Instance.RemoveItemFromInventory(itemSlot.itemData.item))
                {
                    sellerStats.money += itemSlot.itemData.item.baseSellValue;
                    UIHandler.Instance.changeMoneyDirtyFlag = true;
                }
            }
        }

        return true;
    }

    public virtual void InitializeShop(Item.type type, IShop activeShop)
    {
        AddItemsToShop(type, ShopLevels);
    }

    public virtual void OpenMenu()
    {
        UpdateShopUI(this);
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
        ShopUIController.Instance.SetCurrentShop(activeShop);
    }
}
