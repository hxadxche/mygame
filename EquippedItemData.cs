using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipped Items Data")]
public class EquippedItemsData : ScriptableObject
{
    public ItemData weapon;
    public ItemData helmet;
    public ItemData armor;
    public ItemData legs;
    public ItemData ring;
    public ItemData amulet;

    public void ClearAll()
    {
        weapon = helmet = armor = legs = ring = amulet = null;
    }
}