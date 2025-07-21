using System.Collections.Generic;
using UnityEngine;

public static class PassiveEffectLibrary
{
    public static List<PassiveEffectData> weapons = new();
    public static List<PassiveEffectData> armor = new();
    public static List<PassiveEffectData> accessories = new();

    public static void Load()
    {
        var all = Resources.LoadAll<PassiveEffectData>("ScriptableObjects/PassiveEffects");
        foreach (var effect in all)
        {
            switch (effect.effectType)
            {
                case PassiveEffectType.Lifesteal:
                case PassiveEffectType.BonusCrit:
                    weapons.Add(effect);
                    break;

                case PassiveEffectType.Thorns:
                    armor.Add(effect);
                    break;

                case PassiveEffectType.BonusCoins:
                case PassiveEffectType.ExtraXP:
                    accessories.Add(effect);
                    break;
            }
        }
    }

    public static List<PassiveEffectData> GetEffectsFor(ItemType type)
    {
        return type switch
        {
            ItemType.Weapon => weapons,
            ItemType.Helmet or ItemType.Armor or ItemType.Legs => armor,
            ItemType.Ring or ItemType.Amulet => accessories,
            _ => new List<PassiveEffectData>()
        };
    }
}
