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

    public void OnPickUp()
    {
        PlayerHeldItem.Instance.PickUp(itemId, itemName, requiredTargetTag);

        if (unlocksOnPickup)
        {
            InventoryManager.Instance.AddItem(itemId, itemName);
        }

        gameObject.SetActive(false);
    }

    public void OnUse()
    {
        // Items aren't "used" directly while standing near them -
        // Use is triggered by standing near the matching UseTarget instead (e.g. the fire).
    }
}