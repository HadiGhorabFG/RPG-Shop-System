using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ItemsContainer : ScriptableObject
{
    public static List<Item> allItems = new List<Item>();

    private void OnEnable()
    {
        UpdateItems();
    }

    public static void UpdateItems()
    {
        allItems.Clear();
        
        string[] allPaths = Directory.GetFiles("Assets/Scripts/ScriptableObjects/Items", "*asset",
            SearchOption.AllDirectories);
        
        foreach (string path in allPaths)
        {
            allItems.Add((Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item)));
        }
    }
}
