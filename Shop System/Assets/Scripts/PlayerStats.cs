using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int money;
    public int maxInvSlots = 12;
    public List<FItemData> inventory = new List<FItemData>();

    [Serializable]
    public struct FItemData
    {
        public FItemData(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
            this.name = item.name;
        }

        public string name;
        public Item item;
        public int quantity;
    }

    public List<FItemData> Inventory
    {
        get => inventory;
    }

    public bool AddItemToInventory(Item item)
    {
        
        if (inventory.Count >= maxInvSlots && !item.stackable)
            return false;

        if (item.stackable && ContainsItem(item))
        {
            AddQuantityToItem(item);
            inventory = SortItems.Sort(Inventory, ShopUI.Instance.sortingState, false);
            return true;
        }
            
        inventory.Add(new FItemData(item, 1));
        inventory = SortItems.Sort(Inventory, ShopUI.Instance.sortingState, false);
        return true;
    }
    
    public bool RemoveItemFromInventory(Item item)
    {
        if (!ContainsItem(item))
            return false;

        if (item.stackable && ContainsItem(item))
        {
            if(!RemoveQuantityFromItem(item))
            {
                inventory.Remove(inventory.First(s => item == s.item)); //using LINQ?
            }
            
            inventory = SortItems.Sort(Inventory, ShopUI.Instance.sortingState, false);
            return true;
        }
        
        inventory.Remove(inventory.First(s => item == s.item));
        inventory = SortItems.Sort(Inventory, ShopUI.Instance.sortingState, false); //is this static correct?
        return true;
    }

    public FItemData GetItem(Item Item)
    {
        foreach (var invItem in inventory)
        {
            if (invItem.item == Item)
            {
                return invItem;
            }
        }
        
        Debug.LogError("Item doesnt exist in Inventory, you shouldn't be getting this error.");
        return new(Item, -1);
    }
    
    private bool ContainsItem(Item Item)
    {
        foreach (var invItem in inventory)
        {
            if (invItem.item == Item)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void AddQuantityToItem(Item Item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == Item)
            {
                inventory[i] = new(inventory[i].item, inventory[i].quantity + 1);
            }
        }
    }

    private bool RemoveQuantityFromItem(Item Item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == Item && inventory[i].quantity > 1)
            {
                inventory[i] = new(inventory[i].item, inventory[i].quantity - 1);
                return true;
            }
            else if(inventory[i].item == Item && inventory[i].quantity <= 1)
            {
                return false;
            }
        }

        return false;
    }
}
