using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private static ShopUI instance;
    private static readonly object padlock = new object();
    public static ShopUI Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }
    
    public enum SortingState
    {
        Price,
        Level,
        Quantity,
    }

    public SortingState sortingState;
    
    public bool totalCostsDirtyFlag = false;
    
    private ShopItemUI[] buyingItemSlots;
    private ShopItemUI[] sellingItemSlots;
    
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private GameObject sellMenu;
    [SerializeField] private GameObject buyMenu;

    [SerializeField] private Text playerMoney;
    [SerializeField] private Text buyTotal;
    [SerializeField] private Text sellTotal;
    [SerializeField] private Text inventoryCountText;
    [SerializeField] private TextMeshProUGUI sortingOptionText;

    [SerializeField] private int maxItemSlots;
    private IShop activeShop;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
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
        buyTotal.text = "0";
        sellTotal.text = "0";
    
        OnSortingValueChange();
    }

    private void Update()
    {
        playerMoney.text = playerStats.money.ToString();
        inventoryCountText.text = playerStats.inventory.Count + "/" + playerStats.maxInvSlots;
        
        if(totalCostsDirtyFlag)
        {
            buyTotal.text = CalculateTotalPrice(buyingItemSlots).ToString();
            sellTotal.text = CalculateTotalPrice(sellingItemSlots).ToString();
            totalCostsDirtyFlag = false;
        }
    }

    public void OnSortingValueChange()
    {
        SetSortingState();
        playerStats.inventory = SortItems.Sort(playerStats.inventory, sortingState, false);
        activeShop.BuyingItems = SortItems.Sort(activeShop.BuyingItems, sortingState, true);
        SetItemSlots(activeShop, maxItemSlots,maxItemSlots);
    }

    private void SetSortingState()
    {
        string sortOption = sortingOptionText.text;

        if (sortOption == "Price")
            sortingState = SortingState.Price;
        else if (sortOption == "Level")
            sortingState = SortingState.Level;
        else if (sortOption == "Quantity")
            sortingState = SortingState.Quantity;
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
                buyingItemSlots[i].SetItem(shop.BuyingItems[i].item, ShopItemUI.SlotState.Active, ShopItemUI.TradeState.Buying);
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
                sellingItemSlots[i].SetItem(playerStats.Inventory[i].item, ShopItemUI.SlotState.Active, ShopItemUI.TradeState.Selling);
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
        buyTotal.text = "0";
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
        sellTotal.text = "0";
    }

    public void OnExitButtonClick()
    {
        gameObject.SetActive(false);
    }

    List<Item> GetSelectedItems(ShopItemUI[] itemsSlots)
    {
        var selectedItems = new List<Item>();

        foreach (var item in itemsSlots)
        {
            if (item.isSelected && item.slotState == ShopItemUI.SlotState.Active)
            {
                selectedItems.Add(item.item);
            }
        }

        return selectedItems;
    }

    private int CalculateTotalPrice(ShopItemUI[] items)
    {
        //todo: can refactor ?
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
        return totalSum;
    }
}
