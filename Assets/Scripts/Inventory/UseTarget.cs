using UnityEngine;

/// <summary>
/// Attach to anything a picked-up item can be used on (the fire, a spill, etc.).
/// Implements IInteractable so your existing PlayerInteractor can detect and call it.
/// Give the GameObject a matching Tag (e.g. "Fire") and set requiredItemId to the
/// itemId of the PickableItem that should resolve it.
/// </summary>
public class UseTarget : MonoBehaviour, IInteractable
{
    [Tooltip("Must match the itemId on the PickableItem that resolves this, e.g. 'fire_extinguisher'")]
    public string requiredItemId;

    [Tooltip("Optional effect (smoke puff, sparkle, etc.) spawned when resolved")]
    public GameObject resolveEffectPrefab;

    [Tooltip("What actually gets deactivated when resolved. Leave empty to deactivate this object itself; assign the parent Fire object if UseTarget sits on a separate wider-range child.")]
    public GameObject objectToDeactivate;

    public void OnPickUp()
    {
        // Targets aren't picked up.
    }

    public void OnUse()
    {
        if (!PlayerHeldItem.Instance.IsHoldingItem) return;
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId) return;

        Animator playerAnimator = PlayerHeldItem.Instance.GetComponentInChildren<Animator>();
        Debug.Log($"[UseTarget] playerAnimator = {(playerAnimator != null ? playerAnimator.gameObject.name : "NULL")}");
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("BoyEx");
            Debug.Log("[UseTarget] SetTrigger(BoyEx) called");
        }

        GameObject target = objectToDeactivate != null ? objectToDeactivate : gameObject;

        if (resolveEffectPrefab != null)
        {
            Instantiate(resolveEffectPrefab, target.transform.position, Quaternion.identity);
        }

        InventoryManager.Instance.AddItem(PlayerHeldItem.Instance.HeldItemId, PlayerHeldItem.Instance.HeldItemName);
        InventoryNotificationUI.Instance.ShowMessage($"{PlayerHeldItem.Instance.HeldItemName} added to inventory!");

        PlayerHeldItem.Instance.ClearHeldItem();
        target.SetActive(false);
    }
}