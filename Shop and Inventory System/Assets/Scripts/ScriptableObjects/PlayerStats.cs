using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    public int cp;
    
    public int money;
    public int maxInvSlots = 24;
    [SerializeField] private List<FItemData> inventory = new List<FItemData>();

    [Serializable]
    public class FItemData
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

    public string GetInventorySpaceText()
    {
        return inventory.Count + "/" + maxInvSlots;
    }
    
    public void SetInventory(List<FItemData> items)
    {
        inventory = items;
    }
}
