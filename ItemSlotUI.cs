using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : MonoBehaviour
{
    [Header("Иконка и название")]
    public Image icon;                  // Иконка предмета
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;

    [Header("Галочка «экипировано»")]
    public GameObject equippedIndicator;

    private ItemData itemData;
    private Equipment equipment;
    private InventoryUIManager manager;

    // Этот Image — сама рамка, висит на слоте
    private Image slotFrameImage;

    private void Awake()
    {
        // Берём Image этого объекта — рамку
        slotFrameImage = GetComponent<Image>();
    }

    public void Setup(ItemData data, Equipment eq, InventoryUIManager mgr)
    {
        itemData = data;
        equipment = eq;
        manager = mgr;

        icon.sprite = data.icon;
        itemNameText.text = data.itemName;

        if (quantityText != null)
        {
            quantityText.text = data.quantity > 1 ? $"x{data.quantity}" : "";
        }

        UpdateSlotFrame(data.rarity);
        UpdateEquippedIcon();

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(ShowDescription);
    }

    private void UpdateSlotFrame(Rarity rarity)
    {
        if (slotFrameImage == null) return;

        string frameName = $"Frame_{rarity}";
        Sprite frameSprite = Resources.Load<Sprite>($"SlotFrames/{frameName}");

        if (frameSprite != null)
        {
            slotFrameImage.sprite = frameSprite;
        }
        else
        {
            Debug.LogWarning($"❗️ Не найден спрайт рамки для редкости: {rarity}");
        }
    }

    private void ShowDescription()
    {
        manager.descriptionUI.Show(itemData, OnEquipConfirmed);
    }

    private void OnEquipConfirmed()
    {
        equipment.Equip(itemData);
        manager.RefreshAllSlots();
        manager.descriptionUI.Hide();
    }

    public void UpdateEquippedIcon()
    {
        if (equippedIndicator == null) return;

        var eq = FindObjectOfType<Equipment>();
        bool isEquipped = eq != null && eq.IsItemEquipped(itemData);

        equippedIndicator.SetActive(isEquipped);
    }

    public void SetData(ItemData data)
    {
        if (manager == null)
            manager = FindObjectOfType<InventoryUIManager>();
        if (equipment == null)
            equipment = FindObjectOfType<Equipment>();

        Setup(data, equipment, manager);
    }
}
public static class RarityFrameLoader
    {
        public static Sprite GetFrameSpriteByRarity(Rarity rarity)
        {
            string path = $"SlotFrames/Frame_{rarity}";
            return Resources.Load<Sprite>(path);
        }
    }

