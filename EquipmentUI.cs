using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public ItemType type;
        public Transform slotRoot;
        [HideInInspector] public Image iconImage;
        [HideInInspector] public TextMeshProUGUI nameText;
    }

    [Header("Правые слоты UI (перенесите сюда корни слотов)")]
    public SlotUI[] slotUIs;

    private Equipment _equipment;

    private void Awake()
    {
        _equipment = FindObjectOfType<Equipment>();
        if (_equipment == null)
        {
            Debug.LogError("[EquipmentUI] Не найден компонент Equipment!");
            enabled = false;
            return;
        }

        foreach (var s in slotUIs)
        {
            if (s.slotRoot == null) continue;
            s.iconImage = s.slotRoot.Find(s.type + "Icon")?.GetComponent<Image>();
            s.nameText = s.slotRoot.Find(s.type + "Text")?.GetComponent<TextMeshProUGUI>();

            if (s.iconImage != null)
            {
                s.iconImage.sprite = null;
                s.iconImage.enabled = false;
            }
            if (s.nameText != null)
                s.nameText.text = "";
        }
    }

    private void OnEnable() => _equipment.OnItemEquipped += HandleOnItemEquipped;
    private void OnDisable() => _equipment.OnItemEquipped -= HandleOnItemEquipped;

    private void HandleOnItemEquipped(ItemData item)
    {
        var slot = System.Array.Find(slotUIs, s => s.type == item.type);
        if (slot == null)
        {
            Debug.LogWarning($"[EquipmentUI] Слот для {item.type} не найден!");
            return;
        }
        if (slot.iconImage != null)
        {
            slot.iconImage.sprite = item.icon;
            slot.iconImage.enabled = true;
        }
        if (slot.nameText != null)
            slot.nameText.text = item.itemName;
    }
    public void RefreshAll()
    {
        foreach (var slot in slotUIs)
        {
            if (slot.iconImage != null)
            {
                slot.iconImage.sprite = null;
                slot.iconImage.enabled = false;
            }
            if (slot.nameText != null)
            {
                slot.nameText.text = "";
            }
        }
    }
}
