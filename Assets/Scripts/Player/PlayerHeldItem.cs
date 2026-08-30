using UnityEngine;

/// <summary>
/// Put this on the Player GameObject. Tracks what item is currently "in hand"
/// between the A (pickup) and B (use) button presses. Your B-button script
/// should call TryUseOnNearbyTarget(nearbyTargetGameObject) when B is pressed.
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

    public void PickUp(string itemId, string itemName, string requiredTargetTag)
    {
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