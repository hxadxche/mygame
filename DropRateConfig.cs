using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DropRateConfig", menuName = "Inventory/DropRateConfig")]
public class DropRateConfig : ScriptableObject
{
    [Serializable]
    public class DropChance
    {
        public Rarity rarity;
        [Range(0f, 100f)]
        public float chance;
    }

    public List<DropChance> dropChances = new();

    /// <summary>
    /// ¬озвращает случайную редкость на основе шансов.
    /// </summary>
    public Rarity GetRandomRarity()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var entry in dropChances)
        {
            cumulative += entry.chance;
            if (roll <= cumulative)
                return entry.rarity;
        }

        return Rarity.Common; // fallback
    }
}