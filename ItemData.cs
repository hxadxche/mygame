using UnityEngine;
using System.Collections.Generic;

public enum ItemType
{
    None,
    Weapon,
    Helmet,
    Armor,
    Legs,
    Ring,
    Amulet
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic,
    Ancient
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public Rarity rarity;

    [Header("Характеристики")]
    public int powerBonus;
    public int defenseBonus;
    public int regenBonus;

    [Header("Базовые характеристики")]
    public int basePower;
    public int baseDefense;
    public int baseRegen;

    [Header("Улучшение")]
    public int level = 0;
    public int maxLevel = 5;

    [Header("Инвентарь")]
    public int quantity = 1;
    public bool isEquipped = false;

    [Header("Лор предмета")]
    [TextArea(3, 6)]
    public string lore;

    [Header("Пассивные эффекты")]
    public List<PassiveEffectData> passiveEffects = new();

    [HideInInspector] public bool isDropped = false;
    public string itemID;

    public void InitializeBaseStats()
    {
        if (basePower == 0 && powerBonus > 0)
            basePower = powerBonus;
        if (baseDefense == 0 && defenseBonus > 0)
            baseDefense = defenseBonus;
        if (baseRegen == 0 && regenBonus > 0)
            baseRegen = regenBonus;
    }

    public void RecalculateStats()
    {
        float multiplier = 1f + (level * 0.3f);

        switch (type)
        {
            case ItemType.Weapon:
                powerBonus = Mathf.RoundToInt(basePower * multiplier);
                break;
            case ItemType.Helmet:
            case ItemType.Armor:
            case ItemType.Legs:
                defenseBonus = Mathf.RoundToInt(baseDefense * multiplier);
                break;
            case ItemType.Ring:
            case ItemType.Amulet:
                regenBonus = Mathf.RoundToInt(baseRegen * multiplier);
                break;
        }
    }

    public int RequiredDuplicates()
    {
        return level switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            _ => 99
        };
    }
}
