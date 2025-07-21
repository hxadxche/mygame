using System;
using UnityEngine;

public enum PassiveEffectType
{
    ExtraXP,       // Уменьшение XP до уровня
    BonusCoins,    // Шанс получить монеты при убийстве
    Thorns,        // Отражение урона врагу при получении урона
    Lifesteal,     // Частичное восстановление HP при атаке врага
    BonusCrit      // Увеличивает шанс критического удара
}

public enum PassiveTrigger
{
    OnEquip,
    OnUnequip,
    OnHitEnemy,
    OnKillEnemy,
    OnTakeDamage
}

[CreateAssetMenu(fileName = "New Passive Effect", menuName = "Items/Passive Effect")]
public class PassiveEffectData : ScriptableObject
{
    public string effectName;
    [TextArea(2, 5)] public string description;
    public PassiveTrigger trigger;

    [Header("Настройки")]
    public float chance = 1f;
    public int value = 0;
    public string extraTag;

    public PassiveEffectType effectType;
}
