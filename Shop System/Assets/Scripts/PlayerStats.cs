using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int money;
    [SerializeField] private int maxInvSlots = 12;
    [SerializeField] private List<Item> inventory;

    public List<Item> Inventory
    {
        get
        {
            return inventory;
        }
    }

    public bool AddItemToInventory(Item item)
    {
        if (inventory.Count < maxInvSlots)
        {
            inventory.Add(item);
            return true;
        }
        
        return false;
    }
    
    public bool RemoveItemFromInventory(Item item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            return true;
        }

        return false;
    }
}
