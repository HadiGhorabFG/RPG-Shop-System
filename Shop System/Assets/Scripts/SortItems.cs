using System;
using System.Collections.Generic;
using UnityEngine;

public class SortItems : MonoBehaviour
{
    public static List<PlayerStats.FItemData> Sort(List<PlayerStats.FItemData> list, ShopUI.SortingState state, bool buyingItems) //idk, static??
    {
        return SortItems();
        
        List<PlayerStats.FItemData> SortItems()
        {
            switch (state)
            {
                case ShopUI.SortingState.Price:
                    SortPrice(buyingItems);
                    break;
                case ShopUI.SortingState.Level:
                    SortLevel();
                    break;
                case ShopUI.SortingState.Quantity:
                    SortQuantity();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            void SortPrice(bool buyingItems)
            {
                if (buyingItems)
                {
                    list.Sort((a, b) => a.item.baseBuyValue.CompareTo(b.item.baseBuyValue));
                    list.Reverse(); 
                }
                else
                {
                    list.Sort((a, b) => a.item.baseSellValue.CompareTo(b.item.baseSellValue));
                    list.Reverse();
                }
            }            
            
            void SortLevel()
            {
                list.Sort((a, b) => a.item.itemLevel.CompareTo(b.item.itemLevel));
                list.Reverse();
            }            
            
            void SortQuantity()
            {
                list.Sort((a, b) => a.quantity.CompareTo(b.quantity));
                list.Reverse();
            }

            return list;
        }
    }
}
