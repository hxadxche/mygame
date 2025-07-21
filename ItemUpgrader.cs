using UnityEngine;

public class ItemUpgrader : MonoBehaviour
{
    public bool TryUpgrade(ItemData item)
    {
        if (item.level >= item.maxLevel)
        {
            Debug.Log("Максимальный уровень достигнут.");
            return false;
        }

        if (item.rarity >= Rarity.Legendary)
        {
            Debug.Log("Улучшение предметов Legendary и выше пока отключено.");
            return false;
        }

        if (!HasDuplicate(item))
        {
            Debug.Log("Нет дубликатов для улучшения.");
            return false;
        }

        int coinCost = item.level switch
        {
            0 => 10,
            1 => 30,
            2 => 70,
            _ => 9999
        };

        if (!PlayerResources.Instance.SpendCoins(coinCost))
        {
            Debug.Log("Недостаточно монет.");
            return false;
        }

        RemoveDuplicate(item);

        item.InitializeBaseStats();

        item.level++;
        item.RecalculateStats();

        Debug.Log($"[Upgrade] {item.itemName} улучшен до уровня {item.level}");

        // ✅ ДОБАВЛЕНИЕ ПАССИВНОГО ЭФФЕКТА НА МАКС. УРОВНЕ
        if (item.level == item.maxLevel && item.passiveEffects.Count == 0)
        {
            var pool = PassiveEffectLibrary.GetEffectsFor(item.type);
            if (pool != null && pool.Count > 0)
            {
                var selected = pool[Random.Range(0, pool.Count)];
                item.passiveEffects.Add(selected);
                Debug.Log($"[Passive] {item.itemName} получил эффект: {selected.effectName}");
            }
        }

        return true;
    }

    private bool HasDuplicate(ItemData item)
    {
        return item.quantity > 1;
    }

    private void RemoveDuplicate(ItemData item)
    {
        item.quantity -= 1;
    }
}
