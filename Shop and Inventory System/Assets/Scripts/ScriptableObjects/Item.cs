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
    public int baseBuyValue;
    public int baseSellValue;
    public float sellPercentage = 0.5f;
    
    private void OnValidate()
    {
        baseSellValue = (int)(baseBuyValue * sellPercentage);
    }
}
