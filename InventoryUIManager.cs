using UnityEngine;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Ссылки")]
    public Transform itemContentParent;
    public GameObject itemSlotPrefab;
    public Equipment equipment;
    public InventoryScroller scroller;

    [Header("Description UI")]
    public ItemDescriptionUI descriptionUI;

    [Header("Ручной режим (если не Resources)")]
    public ItemData[] itemsToDisplay;
    public bool loadFromResources = false;
    public string resourcesPath = "Items";

    public static List<ItemSlotUI> allSlots = new();

    void Start()
    {
        RefreshInventoryUI(); // ← запуск отображения при старте
    }

    public void RefreshInventoryUI()
    {
        allSlots.Clear();
        foreach (Transform c in itemContentParent) Destroy(c.gameObject);

        foreach (var item in InventoryItemStorage.playerItems)
        {
            if (item == null) continue;

            var go = Instantiate(itemSlotPrefab, itemContentParent);
            var ui = go.GetComponent<ItemSlotUI>();
            ui.Setup(item, equipment, this);
            allSlots.Add(ui);
        }

        RefreshAllSlots();

        if (scroller != null)
            scroller.ResetScroll(); // сброс прокрутки
    }

    public void RefreshAllSlots()
    {
        foreach (var slot in allSlots)
            slot.UpdateEquippedIcon();
    }

    public void FilterInventoryByType(ItemType typeToShow)
    {
        allSlots.Clear();
        foreach (Transform c in itemContentParent) Destroy(c.gameObject);

        foreach (var item in InventoryItemStorage.playerItems)
        {
            if (item == null || item.type != typeToShow) continue;

            var go = Instantiate(itemSlotPrefab, itemContentParent);
            var ui = go.GetComponent<ItemSlotUI>();
            ui.Setup(item, equipment, this);
            allSlots.Add(ui);
        }

        RefreshAllSlots();
        if (scroller != null)
            scroller.ResetScroll();
    }

    public void AddNewItemToUI(ItemData item)
    {
        var go = Instantiate(itemSlotPrefab, itemContentParent);
        var ui = go.GetComponent<ItemSlotUI>();
        ui.Setup(item, equipment, this);
        allSlots.Add(ui);

        RefreshAllSlots();
    }
}