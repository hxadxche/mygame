using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Equipment : MonoBehaviour
{
    public EquippedItemsData equippedItemsData;
    public event Action<ItemData> OnItemEquipped;
    private bool suppressMemorySave = false;

    [System.Serializable]
    public class EquipmentSlot
    {
        public ItemData item;
        public Image icon;
        public TextMeshProUGUI label;
    }

    [Header("Слоты экипировки")]
    [SerializeField] private EquipmentSlot weapon;
    [SerializeField] private EquipmentSlot helmet;
    [SerializeField] private EquipmentSlot armor;
    [SerializeField] private EquipmentSlot legs;
    [SerializeField] private EquipmentSlot ring;
    [SerializeField] private EquipmentSlot amulet;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        TryRestoreEquippedFromMemory();
        EquipmentMemory.Reset();
    }

    private void TryRestoreEquippedFromMemory()
    {
        if (!EquipmentMemory.dataIsValid) return;

        suppressMemorySave = true;

        foreach (var kvp in EquipmentMemory.GetEquippedItemsResolved())
        {
            if (kvp.Value != null)
            {
                Equip(kvp.Value);
            }
        }

        suppressMemorySave = false;
    }

    public void Equip(ItemData newItem)
    {
        var slot = newItem.type switch
        {
            ItemType.Weapon => weapon,
            ItemType.Helmet => helmet,
            ItemType.Armor => armor,
            ItemType.Legs => legs,
            ItemType.Ring => ring,
            ItemType.Amulet => amulet,
            _ => null
        };
        if (slot == null) return;

        if (slot.item != null)
            slot.item.isEquipped = false;

        slot.item = newItem;
        newItem.isEquipped = true;

        if (equippedItemsData != null)
        {
            switch (newItem.type)
            {
                case ItemType.Weapon: equippedItemsData.weapon = newItem; break;
                case ItemType.Helmet: equippedItemsData.helmet = newItem; break;
                case ItemType.Armor: equippedItemsData.armor = newItem; break;
                case ItemType.Legs: equippedItemsData.legs = newItem; break;
                case ItemType.Ring: equippedItemsData.ring = newItem; break;
                case ItemType.Amulet: equippedItemsData.amulet = newItem; break;
            }
        }

        if (slot.icon != null)
        {
            slot.icon.sprite = newItem.icon;
            slot.icon.enabled = true;
        }
        if (slot.label != null)
            slot.label.text = newItem.itemName;

        ApplyAllBonuses();

        OnItemEquipped?.Invoke(newItem);
        if (!suppressMemorySave)
            EquipmentMemory.SaveFromEquipment(this);
    }

    private void ApplyAllBonuses()
    {
        if (playerStats == null) return;
        playerStats.ResetBonuses();

        void TryApply(EquipmentSlot s)
        {
            if (s.item != null)
            {
                playerStats.bonusDamageFromItems += s.item.powerBonus;
                playerStats.bonusDefenseFromItems += s.item.defenseBonus;
                playerStats.bonusRegenFromItems += s.item.regenBonus;
            }
        }

        TryApply(weapon);
        TryApply(helmet);
        TryApply(armor);
        TryApply(legs);
        TryApply(ring);
        TryApply(amulet);

        playerStats.RecalculateFinalStats();
        ApplyPassiveEffects();
    }

    public void Unequip(ItemData item)
    {
        Debug.Log($"Попытка снять: {item.itemName}");

        EquipmentSlot slot = item.type switch
        {
            ItemType.Weapon => weapon,
            ItemType.Helmet => helmet,
            ItemType.Armor => armor,
            ItemType.Legs => legs,
            ItemType.Ring => ring,
            ItemType.Amulet => amulet,
            _ => null
        };

        if (slot == null || slot.item == null || slot.item != item)
        {
            Debug.LogWarning("Ошибка снятия предмета.");
            return;
        }

        slot.item.isEquipped = false;
        slot.item = null;

        if (equippedItemsData != null)
        {
            switch (item.type)
            {
                case ItemType.Weapon: equippedItemsData.weapon = null; break;
                case ItemType.Helmet: equippedItemsData.helmet = null; break;
                case ItemType.Armor: equippedItemsData.armor = null; break;
                case ItemType.Legs: equippedItemsData.legs = null; break;
                case ItemType.Ring: equippedItemsData.ring = null; break;
                case ItemType.Amulet: equippedItemsData.amulet = null; break;
            }
        }

        if (slot.icon != null) slot.icon.enabled = false;
        if (slot.label != null) slot.label.text = "";

        ApplyAllBonuses();
        if (!suppressMemorySave)
            EquipmentMemory.SaveFromEquipment(this);
    }

    public void UnequipAll()
    {
        Debug.Log("Снятие всех предметов!");

        void TryUnequip(ref EquipmentSlot slot)
        {
            if (slot.item != null)
            {
                slot.item.isEquipped = false;
                slot.item = null;
                if (slot.icon != null) slot.icon.enabled = false;
                if (slot.label != null) slot.label.text = "";
            }
        }

        TryUnequip(ref weapon);
        TryUnequip(ref helmet);
        TryUnequip(ref armor);
        TryUnequip(ref legs);
        TryUnequip(ref ring);
        TryUnequip(ref amulet);

        ApplyAllBonuses();
        if (!suppressMemorySave)
            EquipmentMemory.SaveFromEquipment(this);
    }

    public bool IsItemEquipped(ItemData item)
    {
        return weapon.item == item || helmet.item == item || armor.item == item ||
               legs.item == item || ring.item == item || amulet.item == item;
    }

    public ItemData GetItem(ItemType type)
    {
        return type switch
        {
            ItemType.Weapon => weapon.item,
            ItemType.Helmet => helmet.item,
            ItemType.Armor => armor.item,
            ItemType.Legs => legs.item,
            ItemType.Ring => ring.item,
            ItemType.Amulet => amulet.item,
            _ => null
        };
    }

    private void ApplyPassiveEffects()
    {
        List<ItemData> allEquipped = new List<ItemData>
        {
            weapon.item, helmet.item, armor.item, legs.item, ring.item, amulet.item
        };

        foreach (var item in allEquipped)
        {
            if (item == null || item.passiveEffects == null) continue;

            foreach (var effect in item.passiveEffects)
            {
                ApplyPassiveEffect(effect);
            }
        }
    }

    private void ApplyPassiveEffect(PassiveEffectData effect)
    {
        if (playerStats == null) return;

        switch (effect.effectType)
        {
            case PassiveEffectType.ExtraXP:
                playerStats.xpToNextLevel = Mathf.Max(5, playerStats.xpToNextLevel - effect.value);
                break;

            case PassiveEffectType.BonusCoins:
                // Обработается позже в OnKillEnemy, не здесь
                break;

            default:
                break;
        }
    }

}
