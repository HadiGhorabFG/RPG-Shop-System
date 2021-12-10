using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    private static ItemsManager instance;

    public static ItemsManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }
    
    public List<Item> allItems;
    
    public List<Item> levelOneItems;
    public List<Item> levelTwoItems;

    private void Awake()
    {
        instance = this;
        
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
            }
        }
    }
}
