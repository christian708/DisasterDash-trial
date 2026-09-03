using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Put this on your item slot prefab. Needs an Image (for the icon) and
/// optionally a TMP_Text (for quantity) and a Button as children/self.
/// </summary>
public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button button;

    private string itemId;

    public void Setup(string itemId, int quantity, Sprite icon, bool pickedUp)
    {
        this.itemId = itemId;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.color = pickedUp ? Color.white : new Color(0.3f, 0.3f, 0.3f, 0.6f);
        }

        if (quantityText != null)
        {
            quantityText.text = (pickedUp && quantity > 1) ? quantity.ToString() : "";
        }

        if (button != null)
        {
            button.interactable = pickedUp;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => InvUICon.Instance.ShowItem(itemId));
        }
    }
}