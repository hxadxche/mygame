using UnityEngine;
using System.Collections.Generic;

public static class PassiveEffectManager
{
    public static void Trigger(PassiveTrigger trigger, GameObject context = null, int amount = 0)
    {
        Equipment eq = Object.FindObjectOfType<Equipment>();
        if (eq == null) return;

        List<ItemData> items = new List<ItemData>
        {
            eq.GetItem(ItemType.Weapon),
            eq.GetItem(ItemType.Helmet),
            eq.GetItem(ItemType.Armor),
            eq.GetItem(ItemType.Legs),
            eq.GetItem(ItemType.Ring),
            eq.GetItem(ItemType.Amulet)
        };

        foreach (var item in items)
        {
            if (item == null || item.passiveEffects == null) continue;

            foreach (var effect in item.passiveEffects)
            {
                if (effect.trigger != trigger) continue;
                if (Random.value > effect.chance) continue;

                ApplyEffect(effect, context, amount);
            }
        }
    }

    private static void ApplyEffect(PassiveEffectData effect, GameObject context, int amount)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var stats = player.GetComponent<PlayerStats>();
        var health = player.GetComponent<PlayerHealth>();

        switch (effect.effectType)
        {
            case PassiveEffectType.ExtraXP:
                if (stats != null)
                    stats.extraXPFlatBonus += effect.value;
                break;

            case PassiveEffectType.BonusCoins:
                PlayerResources.Instance?.AddCoins(effect.value);
                break;

            case PassiveEffectType.Lifesteal:
                if (health != null)
                    health.Heal(effect.value);
                break;

            case PassiveEffectType.Thorns:
                if (context != null)
                {
                    var enemy = context.GetComponent<EnemyHealth>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(effect.value);
                        Debug.Log($"[PASSIVE] Thorns: нанесено {effect.value} урона врагу {enemy.enemyName}");
                    }
                    else
                    {
                        Debug.Log("[PASSIVE] Thorns: context есть, но EnemyHealth не найден.");
                    }
                }
                else
                {
                    Debug.Log("[PASSIVE] Thorns: context == null");
                }
                break;


            case PassiveEffectType.BonusCrit:
                if (stats != null)
                    stats.bonusCritChance += effect.value;
                break;
        }
    }
}