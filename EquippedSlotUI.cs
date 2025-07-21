using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    public void Set(ItemData item)
    {
        if (iconImage != null) iconImage.sprite = item.icon;
        if (nameText != null) nameText.text = item.itemName;

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        if (iconImage != null) iconImage.sprite = null;
        if (nameText != null) nameText.text = "";
        gameObject.SetActive(false);
    }
}
