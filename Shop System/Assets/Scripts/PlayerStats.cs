using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int money;
    [SerializeField] private int maxInvSlots = 12;
    [SerializeField] private List<Item> inventory;

    private void OnEnable()
    {
        inventory = SortItemsWithPrice.SortSellPrice(Inventory);
    }

    public List<Item> Inventory
    {
        get => inventory;
    }

    public bool AddItemToInventory(Item item)
    {
        if (inventory.Count >= maxInvSlots)
            return false;
        
        inventory.Add(item);
        inventory = SortItemsWithPrice.SortSellPrice(Inventory);
        return true;
    }
    
    public bool RemoveItemFromInventory(Item item)
    {
        if (!inventory.Contains(item))
            return false;
        
        inventory.Remove(item);
        inventory = SortItemsWithPrice.SortSellPrice(Inventory); //is this static correct?
        return true;
    }
}
