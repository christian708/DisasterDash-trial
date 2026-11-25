using UnityEngine;

/// <summary>
/// Attach to any world item the player can pick up (extinguisher, towel, etc.).
/// Implements IInteractable so your existing PlayerInteractor can detect and call it.
/// </summary>
public class PickItem : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    public string itemId;
    public string itemName;

    [Header("Behavior")]
    [Tooltip("Tag of the object this item can be used on, e.g. 'Fire'. Leave empty for passive equipment items with no target to resolve (e.g. shoes).")]
    public string requiredTargetTag = "";

    [Tooltip("Check this for passive equip-and-hold items with no UseTarget (e.g. shoes slowing earthquake penalty) - unlocks in the bag immediately on pickup instead of waiting for a target to be resolved.")]
    public bool unlocksOnPickup = false;

    [Tooltip("Check this for wearables like the raincoat and boots - marks the item as equipped in InventoryManager on pickup (Wind.cs and FloodSurge.cs read this).")]
    public bool equipOnPickup = false;

    public void OnPickUp()
    {
        if (PlayerHeldItem.Instance != null)
        {
            PlayerHeldItem.Instance.PickUp(itemId, itemName, requiredTargetTag);
        }
        else
        {
            Debug.LogWarning("[PickItem] PlayerHeldItem.Instance is null - add PlayerHeldItem to the Player in this scene.");
        }

        // Play the pickup sound if an ElementSoundPlayer is attached
        ElementSoundPlayer esp = GetComponent<ElementSoundPlayer>();
        if (esp != null) esp.PlayStartSound();

        if (InventoryManager.Instance != null)
        {
            if (unlocksOnPickup)
            {
                InventoryManager.Instance.AddItem(itemId, itemName);
            }

            if (equipOnPickup)
            {
                InventoryManager.Instance.SetEquippedItem(itemId, itemName, requiredTargetTag);
            }
        }
        else
        {
            Debug.LogWarning("[PickItem] InventoryManager.Instance is null - start the game from the Main scene.");
        }

        gameObject.SetActive(false);
    }

    public void OnUse()
    {
        // Items aren't "used" directly while standing near them -
        // Use is triggered by standing near the matching UseTarget instead (e.g. the fire).
    }
}