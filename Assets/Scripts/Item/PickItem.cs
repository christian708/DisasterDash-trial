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
    [Tooltip("Tag of the object this item can be used on, e.g. 'Fire'.")]
    public string requiredTargetTag = "";

    public void OnPickUp()
    {
        PlayerHeldItem.Instance.PickUp(itemId, itemName, requiredTargetTag);
        gameObject.SetActive(false);
    }

    public void OnUse()
    {
        // Items aren't "used" directly while standing near them -
        // Use is triggered by standing near the matching UseTarget instead (e.g. the fire).
    }
}