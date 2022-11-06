using System;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum type
    {
        weapon, general, armour
    }

    public string id = Guid.NewGuid().ToString().ToUpper();
    
    public type Type;
    public Sprite icon;
    public string name;
    public int itemLevel = 1;
    public bool stackable = false;
    
    [Header("Vendor")]
    public int baseBuyValue;
    public int baseSellValue;
    public float sellPercentage = 0.5f;

    //attributes
    [Header("Attributes")]
    public int health;
    public int mana;
        
    public int damage;
    [Range(0, 1)] 
    public float criticalChance;

    public int armour;
    public int armourHealth;
    
    //crafting
    [Header("Crafting")]
    public int craftingMaxDurability;
    public int craftingMaxProgress;
    public int craftingMaxQuality;
    
    private void OnValidate()
    {
        baseSellValue = (int)(baseBuyValue * sellPercentage);
    }
}
