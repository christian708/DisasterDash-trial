using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put on a small placeholder box inside MobileControlsPanel. Shows the
/// currently held item's icon, hides itself when nothing is held.
/// </summary>
public class HeldItemIndicatorUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [Tooltip("Optional - the whole placeholder box, hidden when nothing is held. If left empty, iconImage's own GameObject is toggled instead.")]
    [SerializeField] private GameObject placeholderBox;

    private string lastShownItemId;

    private void Update()
    {
        if (PlayerHeldItem.Instance == null || !PlayerHeldItem.Instance.IsHoldingItem)
        {
            if (lastShownItemId != null)
            {
                lastShownItemId = null;
                SetVisible(false);
            }
            return;
        }

        string currentId = PlayerHeldItem.Instance.HeldItemId;
        if (currentId == lastShownItemId) return; // no change, skip lookup

        lastShownItemId = currentId;

        ItemDefinition def = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetDefinition(currentId) : null;

        if (iconImage != null) iconImage.sprite = def != null ? def.icon : null;
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (placeholderBox != null)
        {
            placeholderBox.SetActive(visible);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(visible);
        }
    }
}