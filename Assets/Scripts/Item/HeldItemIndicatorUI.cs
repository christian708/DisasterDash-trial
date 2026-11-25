using UnityEngine;
using UnityEngine.UI;

public class HeldItemIndicatorUI : MonoBehaviour
{
    [Header("Held item (in hand)")]
    [SerializeField] private Image iconImage;
    [Tooltip("Optional - the whole placeholder box, hidden when nothing is held. If left empty, iconImage's own GameObject is toggled instead.")]
    [SerializeField] private GameObject placeholderBox;

    [Header("Worn items (wearables)")]
    [Tooltip("Pre-placed Image slots, filled in order. Use at least as many as your wearables (5).")]
    [SerializeField] private Image[] wearableSlots;

    private string lastShownItemId;
    private int lastWearablesVersion = -1;

    private void Update()
    {
        UpdateHeldItem();
        UpdateWearables();
    }

    private void UpdateHeldItem()
    {
        if (PlayerHeldItem.Instance == null || !PlayerHeldItem.Instance.IsHoldingItem)
        {
            if (lastShownItemId != null)
            {
                lastShownItemId = null;
                SetHeldVisible(false);
            }
            return;
        }

        string currentId = PlayerHeldItem.Instance.HeldItemId;
        if (currentId == lastShownItemId) return;

        lastShownItemId = currentId;

        ItemDefinition def = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetDefinition(currentId) : null;

        if (iconImage != null) iconImage.sprite = def != null ? def.icon : null;
        SetHeldVisible(true);
    }

    private void UpdateWearables()
    {
        if (InventoryManager.Instance == null || wearableSlots == null) return;

        int version = InventoryManager.Instance.WearablesVersion;
        if (version == lastWearablesVersion) return;
        lastWearablesVersion = version;

        int slot = 0;

        foreach (string id in InventoryManager.Instance.WearableItemIds)
        {
            if (slot >= wearableSlots.Length) break;
            if (!InventoryManager.Instance.IsWearableEquipped(id)) continue;

            ItemDefinition def = ItemDatabase.Instance != null ? ItemDatabase.Instance.GetDefinition(id) : null;

            
            if (wearableSlots[slot] != null)
            {
                wearableSlots[slot].sprite = def != null ? def.icon : null;
                wearableSlots[slot].gameObject.SetActive(true);
            }
            slot++;
        }

        // Hide unused slots
        for (; slot < wearableSlots.Length; slot++)
        {
            if (wearableSlots[slot] != null) wearableSlots[slot].gameObject.SetActive(false);
        }
    }

    private void SetHeldVisible(bool visible)
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