using System.Collections.Generic;
using UnityEngine;

public static class InventoryItemStorage
{
    public static List<ItemData> playerItems = new();

    public static void AddItemToInventory(ItemData item)
    {
        bool isStackable = item.rarity <= Rarity.Epic;

        if (isStackable)
        {
            // Ищем такой же предмет (по имени, типу и редкости)
            foreach (var existing in playerItems)
            {
                if (existing.itemName == item.itemName && existing.rarity == item.rarity && existing.type == item.type)
                {
                    existing.quantity += item.quantity;
                    Debug.Log($"🔁 Увеличено количество предмета: {item.itemName} x{existing.quantity}");

                    // Обновляем только количество (через UI, если нужно)
                    InventoryUIManager ui = GameObject.FindObjectOfType<InventoryUIManager>();
                    if (ui != null) ui.RefreshInventoryUI(); // Обновим весь UI

                    return;
                }
            }
        }

        // Добавляем как новый предмет
        playerItems.Add(item);
        Debug.Log($"🎁 Добавлен предмет: {item.itemName}");

        InventoryUIManager manager = GameObject.FindObjectOfType<InventoryUIManager>();
        if (manager != null)
        {
            manager.AddNewItemToUI(item);
        }
    }
}