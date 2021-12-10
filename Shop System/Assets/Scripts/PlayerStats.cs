using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int money;
    public int negotiationLevel;

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
        if (inventory.Count < 12)
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
