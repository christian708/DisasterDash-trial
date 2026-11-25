using UnityEngine;

/// <summary>
/// Put this on the Player GameObject. Tracks what item is currently "in hand"
/// between the B (pickup) and A (use) button presses.
/// Wearables (coat, boots, helmet, shoe, mask) are never held; they live in InventoryManager.
/// Using items on world targets is handled by the target itself (UseTarget, TreeUseTarget).
/// </summary>
public class PlayerHeldItem : MonoBehaviour
{
    public static PlayerHeldItem Instance { get; private set; }

    public bool IsHoldingItem { get; private set; }
    public string HeldItemId { get; private set; }
    public string HeldItemName { get; private set; }
    public string HeldItemRequiredTargetTag { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (InventoryManager.Instance != null && !string.IsNullOrEmpty(InventoryManager.Instance.EquippedItemId))
        {
            PickUp(
                InventoryManager.Instance.EquippedItemId,
                InventoryManager.Instance.EquippedItemName,
                InventoryManager.Instance.EquippedItemRequiredTargetTag);
        }
    }

    public void PickUp(string itemId, string itemName, string requiredTargetTag)
    {
        // Wearables never go in the hand / left box.
        if (InventoryManager.Instance != null && InventoryManager.Instance.IsWearable(itemId))
        {
            return;
        }

        IsHoldingItem = true;
        HeldItemId = itemId;
        HeldItemName = itemName;
        HeldItemRequiredTargetTag = requiredTargetTag;
    }

    public void ClearHeldItem()
    {
        IsHoldingItem = false;
        HeldItemId = null;
        HeldItemName = null;
        HeldItemRequiredTargetTag = null;
    }
}