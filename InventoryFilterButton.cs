using UnityEngine;
using UnityEngine.UI;

public class InventoryFilterButton : MonoBehaviour
{
    public ItemType filterType = ItemType.None; // None = показать всё
    public InventoryUIManager inventoryManager;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ApplyFilter);
    }

    private void ApplyFilter()
    {
        if (inventoryManager == null) return;

        // Сброс и очистка контента
        inventoryManager.scroller.ClearAndResetContent();

        // Применяем фильтрацию
        if (filterType == ItemType.None)
        {
            inventoryManager.RefreshInventoryUI(); // ← Показываем всё, что у игрока
        }
        else
        {
            inventoryManager.FilterInventoryByType(filterType); // ← Применяем фильтр по типу
        }
    }
}