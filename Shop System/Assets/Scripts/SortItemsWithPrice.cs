using System;
using System.Collections.Generic;
using UnityEngine;

public class SortItemsWithPrice : MonoBehaviour
{
    public List<Item> SortBuyPrice(List<Item> list)
    {
        List<Item> sortedList = new List<Item>();
        int arrayIndex = 0;
        int listIndex = 0;
        int[] array = new int[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i].baseBuyValue;
        }

        Array.Sort(array);
        Array.Reverse(array);

        while(sortedList.Count != list.Count)
        {
            if (array[arrayIndex] == list[listIndex].baseBuyValue && !sortedList.Contains(list[listIndex]))
            {
                sortedList.Add(list[listIndex]);
                arrayIndex++;
                listIndex = 0;
            }
            else
            {
                listIndex++;
            }
        }
        
        return sortedList;
    }
    
    public List<Item> SortSellPrice(List<Item> list)
    {
        List<Item> sortedList = new List<Item>();
        int arrayIndex = 0;
        int listIndex = 0;
        int[] array = new int[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i].baseSellValue;
        }

        Array.Sort(array);
        Array.Reverse(array);

        while(sortedList.Count != list.Count)
        {
            if (array[arrayIndex] == list[listIndex].baseSellValue && !sortedList.Contains(list[listIndex]))
            {
                sortedList.Add(list[listIndex]);
                arrayIndex++;
                listIndex = 0;
            }
            else
            {
                listIndex++;
            }
        }
        
        return sortedList;
    }
}
