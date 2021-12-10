using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private static ShopUI instance;
    public static ShopUI Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }
    
    private ShopItemUI[] buyingItemSlots;
    private ShopItemUI[] sellingItemSlots;
    
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private GameObject sellMenu;
    [SerializeField] private GameObject buyMenu;

    [SerializeField] private Text playerMoney;
    [SerializeField] private Text buyTotal;
    [SerializeField] private Text sellTotal;

    [SerializeField] private int maxItemSlots;
    private int prevCountTotalBuy = 0;
    private int prevCountTotalSell = 0;
        
    private IShop activeShop;

    private void Awake()
    {
        instance = this;
        
        buyingItemSlots = new ShopItemUI[maxItemSlots];
        sellingItemSlots = new ShopItemUI[maxItemSlots];
        
        if(buyMenu.transform.Find("Slots") && sellMenu.transform.Find("Slots"))
        {
            AddItemsToArray(buyingItemSlots, buyMenu.transform.Find("Slots").gameObject);
            AddItemsToArray(sellingItemSlots, sellMenu.transform.Find("Slots").gameObject);
        }
        else
        {
            throw new ArgumentException("Slots child object not found");
        }
    }

    private void Start()
    {
        buyTotal.text = 0.ToString();
        sellTotal.text = 0.ToString();
    }

    private void Update()
    {
        playerMoney.text = playerStats.money.ToString();
        
        if(GetSelectedItems(buyingItemSlots).Count != prevCountTotalBuy)
        {
            buyTotal.text = CalculateTotalPrice(buyingItemSlots, prevCountTotalBuy).ToString();
        }
        
        if(GetSelectedItems(sellingItemSlots).Count != prevCountTotalSell)
        {
            sellTotal.text = CalculateTotalPrice(sellingItemSlots, prevCountTotalSell).ToString();
        }
    }

    public void SetCurrentShop(IShop activeShop)
    {
        this.activeShop = activeShop;
        SetItemSlots(activeShop, maxItemSlots, maxItemSlots);
    }

    private void SetItemSlots(IShop shop, int maxSlotsValueBuy, int maxSlotsValueInv)
    {
        for (int i = 0; i < maxSlotsValueBuy; i++)
        {
            if (i < shop.BuyingItems.Count)
            {
                buyingItemSlots[i].SetItem(shop.BuyingItems[i], ShopItemUI.SlotState.Active, ShopItemUI.TradeState.Buying);
            }
            else
            {
                buyingItemSlots[i].SetItem(null, ShopItemUI.SlotState.Empty, ShopItemUI.TradeState.Buying);
            }
        }
        
        for (int i = 0; i < maxSlotsValueInv; i++)
        {
            if (i < playerStats.Inventory.Count)
            {
                sellingItemSlots[i].SetItem(playerStats.Inventory[i], ShopItemUI.SlotState.Active, ShopItemUI.TradeState.Selling);
            }
            else
            {
                sellingItemSlots[i].SetItem(null, ShopItemUI.SlotState.Empty, ShopItemUI.TradeState.Selling);
            }
        }
    }

    private void AddItemsToArray(ShopItemUI[] array, GameObject from)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = from.transform.GetChild(i).gameObject.GetComponent<ShopItemUI>();
        }
    }

    public void OnBuyButtonClick()
    {
        activeShop.Buy(GetSelectedItems(buyingItemSlots), playerStats);

        foreach (var item in buyingItemSlots)
        {
            if (item.slotState == ShopItemUI.SlotState.Active)
            {
                item.toggle.isOn = false;
                item.ChangeToggleSelection();
            }
        }
        
        SetItemSlots(activeShop, maxItemSlots, maxItemSlots);
        buyTotal.text = 0.ToString();
    }
    
    public void OnSellButtonClick()
    {
        activeShop.Sell(GetSelectedItems(sellingItemSlots), playerStats);
        
        foreach (var item in sellingItemSlots)
        {
            if (item.slotState == ShopItemUI.SlotState.Active)
            {
                item.toggle.isOn = false;
                item.ChangeToggleSelection();
            }
        }
        
        SetItemSlots(activeShop, maxItemSlots, maxItemSlots);
        sellTotal.text = 0.ToString();
    }

    public void OnExitButtonClick()
    {
        gameObject.SetActive(false);
    }

    List<Item> GetSelectedItems(ShopItemUI[] itemsSlots)
    {
        List<Item> selectedItems = new List<Item>();

        for (int i = 0; i < itemsSlots.Length; i++)
        {
            if (itemsSlots[i].isSelected && itemsSlots[i].slotState == ShopItemUI.SlotState.Active)
            {
                selectedItems.Add(itemsSlots[i].item);
            }
        }

        return selectedItems;
    }

    private int CalculateTotalPrice(ShopItemUI[] items, int prevCount)
    {
        int totalSum = 0;
            
        for (int i = 0; i < GetSelectedItems(items).Count; i++)
        {
            if (items == buyingItemSlots)
            {
                totalSum += GetSelectedItems(items)[i].baseBuyValue;
            }
            else if (items == sellingItemSlots)
            {
                totalSum += GetSelectedItems(items)[i].baseSellValue;
            }
        }

        prevCount = GetSelectedItems(items).Count;
        return totalSum;
    }
}
