using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum type
    {
        sword, food, armour
    }

    public type Type;
    public Sprite icon;
    public string name;
    public int itemLevel;
    public int baseBuyValue;
    public int baseSellValue;
}
