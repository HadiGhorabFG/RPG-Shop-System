using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    private static readonly object padlock = new object();
    public static InventoryManager Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }
    
    public delegate void OnInventoryChangedDelegate(List<PlayerStats.FItemData> inventoryItems);
    
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };
    
    [SerializeField] private PlayerStats playerStats;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OnInventoryChanged.Invoke(playerStats.Inventory);
    }

    public void InventoryChanged()
    {
        OnInventoryChanged.Invoke(playerStats.Inventory);
    }

    //todo: change to void?
    public bool AddItemToInventory(Item item)
    {
        if (item.stackable && ContainsItem(item))
        {
            AddQuantityToItem(item);
            return true;
        }
            
        playerStats.Inventory.Add(new PlayerStats.FItemData(item, 1));
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
                playerStats.Inventory.Remove(playerStats.Inventory.First(s => item == s.item));
            }
            
            return true;
        }
        
        playerStats.Inventory.Remove(playerStats.Inventory.First(s => item == s.item));
        return true;
    }

    public PlayerStats.FItemData GetItem(Item Item)
    {
        foreach (var invItem in playerStats.Inventory)
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
        foreach (var invItem in playerStats.Inventory)
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
        for (int i = 0; i < playerStats.Inventory.Count; i++)
        {
            if (playerStats.Inventory[i].item == Item)
            {
                playerStats.Inventory[i] = new(playerStats.Inventory[i].item, playerStats.Inventory[i].quantity + 1);
            }
        }
    }

    private bool RemoveQuantityFromItem(Item Item)
    {
        for (int i = 0; i < playerStats.Inventory.Count; i++)
        {
            if (playerStats.Inventory[i].item == Item && playerStats.Inventory[i].quantity > 1)
            {
                playerStats.Inventory[i] = new(playerStats.Inventory[i].item, playerStats.Inventory[i].quantity - 1);
                return true;
            }
            else if(playerStats.Inventory
                        [i].item == Item && playerStats.Inventory[i].quantity <= 1)
            {
                return false;
            }
        }

        return false;
    }
}
