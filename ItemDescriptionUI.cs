using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class ItemDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI titleText, rarityText, statsText, quantityText, loreText, levelText, upgradeRequirementText, passiveEffectText;
    public Button equipButton, unequipButton, closeButton, upgradeButton;
    public Equipment equipment;

    private ItemData currentItem;
    private Action onEquipCallback;

    void Awake()
    {
        gameObject.SetActive(false);

        closeButton.onClick.AddListener(Hide);
        equipButton.onClick.AddListener(OnEquipClick);

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveAllListeners();
            unequipButton.onClick.AddListener(OnUnequipClick);
        }
        else
        {
            Debug.LogWarning("UnequipButton не назначена в инспекторе!");
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClick);
        }
        else
        {
            Debug.LogWarning("UpgradeButton не назначена в инспекторе!");
        }
    }

    public void Show(ItemData item, Action equipCallback)
    {
        currentItem = item;
        onEquipCallback = equipCallback;

        // Безопасная инициализация (если это первый апгрейд)
        item.InitializeBaseStats();

        titleText.text = item.itemName;
        rarityText.text = $"<color={GetRarityColor(item.rarity)}>{item.rarity}</color>";

        // Расчёт будущих значений от текущих бонусов
        int nextPower = item.powerBonus;
        int nextDefense = item.defenseBonus;
        int nextRegen = item.regenBonus;

        if (item.level < item.maxLevel)
        {
            float multiplier = 1f + 0.3f;
            if (item.powerBonus > 0)
                nextPower = Mathf.RoundToInt(item.powerBonus * multiplier);
            if (item.defenseBonus > 0)
                nextDefense = Mathf.RoundToInt(item.defenseBonus * multiplier);
            if (item.regenBonus > 0)
                nextRegen = Mathf.RoundToInt(item.regenBonus * multiplier);
        }

        List<string> statLines = new List<string>();

        if (item.powerBonus > 0)
        {
            string line = $"<color=#FF4D4D>+{item.powerBonus} dmg</color>";
            if (nextPower > item.powerBonus && item.level < item.maxLevel)
                line += $" → <b><color=#7CFF6E>+{nextPower}</color></b>";
            statLines.Add(line);
        }

        if (item.defenseBonus > 0)
        {
            string line = $"<color=#4D9FFF>+{item.defenseBonus} def</color>";
            if (nextDefense > item.defenseBonus && item.level < item.maxLevel)
                line += $" → <b><color=#7CFF6E>+{nextDefense}</color></b>";
            statLines.Add(line);
        }

        if (item.regenBonus > 0)
        {
            string line = $"<color=#4DFF88>+{item.regenBonus} regen</color>";
            if (nextRegen > item.regenBonus && item.level < item.maxLevel)
                line += $" → <b><color=#7CFF6E>+{nextRegen}</color></b>";
            statLines.Add(line);
        }

        statsText.text = string.Join("\n", statLines);

        quantityText.text = $"x{item.quantity}";
        loreText.text = item.lore;

        levelText.text = item.level >= item.maxLevel
            ? "Level MAX"
            : $"Level {item.level}/{item.maxLevel}";

        UpdateUpgradeRequirement(item);

        unequipButton.gameObject.SetActive(item.isEquipped);
        upgradeButton.gameObject.SetActive(item.level < item.maxLevel);

        // ⬇️ НАСТРОЙКА ГРАДИЕНТА
        var topLeft = new Color32(160, 102, 255, 255);   // #A066FF (фиолет)
        var topRight = new Color32(160, 102, 255, 255);
        var bottomLeft = new Color32(255, 213, 79, 255); // #FFD54F (золотой)
        var bottomRight = new Color32(255, 213, 79, 255);

        passiveEffectText.colorGradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
        passiveEffectText.enableVertexGradient = true;

        if (item.level < item.maxLevel || item.passiveEffects.Count == 0)
        {
            passiveEffectText.text = "Пассивный эффект: ?";
        }
        else
        {
            string effectName = item.passiveEffects[0].effectName;
            passiveEffectText.text = $"Пассивный эффект: {effectName}";
        }

        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEquipClick()
    {
        equipment?.Equip(currentItem);
        onEquipCallback?.Invoke();
        FindObjectOfType<InventoryUIManager>()?.RefreshAllSlots();
        Hide();
    }

    private void OnUnequipClick()
    {
        Debug.Log("UNEQUIP CLICKED");

        if (currentItem != null)
            Debug.Log($"Попытка снять предмет: {currentItem.itemName}");

        equipment?.Unequip(currentItem);
        FindObjectOfType<InventoryUIManager>()?.RefreshAllSlots();
        Hide();
    }

    private void OnUpgradeClick()
    {
        if (currentItem == null)
        {
            Debug.LogError("[ItemDescriptionUI] currentItem == null при улучшении.");
            return;
        }

        var upgrader = FindObjectOfType<ItemUpgrader>();
        if (upgrader == null)
        {
            Debug.LogError("[ItemDescriptionUI] Не найден компонент ItemUpgrader в сцене.");
            return;
        }

        bool success = upgrader.TryUpgrade(currentItem);

        if (success)
        {
            Debug.Log($"[ItemDescriptionUI] Успешное улучшение: {currentItem.itemName} до {currentItem.level} уровня");
            FindObjectOfType<InventoryUIManager>()?.RefreshAllSlots();
            Show(currentItem, onEquipCallback); // Обновить всё
        }
        else
        {
            Debug.Log($"[ItemDescriptionUI] Улучшение не удалось: {currentItem.itemName}");
        }
    }

    private void UpdateUpgradeRequirement(ItemData item)
    {
        if (upgradeRequirementText == null) return;

        if (item.level >= item.maxLevel)
        {
            upgradeRequirementText.text = "Максимальный уровень";
            upgradeButton.interactable = false; // <- дополнительно, отключить кнопку
            return;
        }

        int coinCost = item.level switch
        {
            0 => 10,
            1 => 30,
            2 => 70,
            _ => 9999
        };

        int requiredDuplicates = item.RequiredDuplicates();
        bool hasEnoughDuplicates = item.quantity > requiredDuplicates;
        bool hasEnoughCoins = PlayerResources.Instance.coins >= coinCost;

        string duplicateColor = hasEnoughDuplicates ? "#7CFF6E" : "#FF6E6E";
        string coinColor = hasEnoughCoins ? "#7CFF6E" : "#FF6E6E";

        if (item.rarity >= Rarity.Legendary)
        {
            upgradeRequirementText.text =
                $"<color={coinColor}>{coinCost} монет</color> + <color=#FFD700>шарды</color>";
        }
        else
        {
            upgradeRequirementText.text =
                $"Нужно: <color={duplicateColor}>{item.quantity - 1}/{requiredDuplicates}</color> копий, " +
                $"<color={coinColor}>{coinCost} монет</color>";
        }
    }


    private string GetRarityColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => "#A0A0A0",
            Rarity.Uncommon => "#5CD65C",
            Rarity.Rare => "#3C9BDC",
            Rarity.Epic => "#9A4DFF",
            Rarity.Legendary => "#FFA500",
            Rarity.Mythic => "#FF3C3C",
            Rarity.Ancient => "#FFD700",
            _ => "#FFFFFF"
        };
    }
}
