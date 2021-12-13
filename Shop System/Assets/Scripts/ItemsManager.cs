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
    
    public List<Item> allItems;
    
    public List<Item> levelOneItems;
    public List<Item> levelTwoItems;
    public List<Item> levelThreeItems;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        for (int i = 0; i < allItems.Count; i++)
        {
            switch (allItems[i].itemLevel)
            {
                case 1: 
                    levelOneItems.Add(allItems[i]);
                    break;
                case 2:
                    levelTwoItems.Add(allItems[i]);
                    break;
                case 3:
                    levelThreeItems.Add(allItems[i]);
                    break;
            }
        }
    }
}
