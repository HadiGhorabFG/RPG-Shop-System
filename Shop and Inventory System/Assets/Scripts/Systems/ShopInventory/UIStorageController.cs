using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStorageController : MonoBehaviour
{
    public static bool menuOpen = false;
    
    public virtual void InitializeMenu(IShop shop)
    {
        
    }

    protected virtual void ClearMenu(List<InventorySlot> selectedSlots, List<InventorySlot> slotItems, VisualElement slotContainer)
    {
        selectedSlots.Clear();
        slotItems.Clear();
        slotContainer.Clear();
        InventoryUIController.Instance.InventoryItems.Clear();
    }
    protected virtual void OnExitButtonClick(List<InventorySlot> selectedSlots, List<InventorySlot> slotItems, VisualElement slotContainer, UIDocument document)
    {
        ClearMenu(selectedSlots, slotItems, slotContainer);
        menuOpen = false;
        document.enabled = false;
    }
    
    protected virtual void OnTradeButtonClick(IShop activeShop, List<InventorySlot> selectedSlots, PlayerStats playerStats, Label tradeText, bool shop)
    {
        if(shop)
        {
            if(activeShop.Buy(selectedSlots, playerStats))
                ResetUI();
        }
        else
        {
            if(activeShop.Sell(selectedSlots, playerStats))
                ResetUI();
        }


        void ResetUI()
        {
            foreach (var item in selectedSlots)
            {
                item.SetSelection();
            }
            
            tradeText.text = "0" + "€";

            ClearSlots(selectedSlots);
        }
    }
    
    protected virtual void ClearSlots(List<InventorySlot> selectedSlots)
    {
        selectedSlots.Clear();
        InventoryManager.Instance.InventoryChanged();
        ShopUIController.Instance.OnShopChanged();
    }
    
    public static int CalculateTotalPrice(List<InventorySlot> selectedSlots, bool shop)
    {
        //todo: can refactor ?
        int totalSum = 0;

        if (shop)
        {
            for (int i = 0; i < selectedSlots.Count; i++)
            {
                for (int j = 0; j < selectedSlots[i].GetQuantitySliderValue(); j++)
                {
                    totalSum += selectedSlots[i].itemData.item.baseBuyValue;
                }
            }
        }
        else
        {
            for (int i = 0; i < selectedSlots.Count; i++)
            {
                for (int j = 0; j < selectedSlots[i].GetQuantitySliderValue(); j++)
                {
                    totalSum += selectedSlots[i].itemData.item.baseSellValue;
                }
            }
        }
        
        return totalSum;
    }

    public virtual void UpdateCurrentHighlightedSlot(string name, int sliderQuantity, int itemQuantity)
    {
        
    }
}
