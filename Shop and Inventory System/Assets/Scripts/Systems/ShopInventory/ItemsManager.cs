using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    private static ItemsManager instance;
    private static readonly object padlock = new object();
    public static ItemsManager Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }

    [SerializeField] private PlayerStats playerStats;
    
    public Dictionary<int, List<Item>> itemsByLevel = new Dictionary<int, List<Item>>();

    [HideInInspector] public List<Item> levelOneItems;
    [HideInInspector] public List<Item> levelTwoItems;
    [HideInInspector] public List<Item> levelThreeItems;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ItemsContainer.UpdateItems();
        List<Item> items = ItemsContainer.allItems;
        
        for (int i = 0; i < items.Count; i++)
        {
            switch (items[i].itemLevel)
            {
                case 1: 
                    levelOneItems.Add(items[i]);
                    break;
                case 2:
                    levelTwoItems.Add(items[i]);
                    break;
                case 3:
                    levelThreeItems.Add(items[i]);
                    break;
            }
        }
        
        itemsByLevel.Add(1, levelOneItems);
        itemsByLevel.Add(2, levelTwoItems);
        itemsByLevel.Add(3, levelThreeItems);
    }
}
