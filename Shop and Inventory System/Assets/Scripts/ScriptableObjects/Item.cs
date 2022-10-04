using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum type
    {
        weapon, general, armour
    }

    public type Type;
    public Sprite icon;
    public string name;
    public int itemLevel;
    public bool stackable = false;
    public int baseBuyValue;
    public int baseSellValue;
}
