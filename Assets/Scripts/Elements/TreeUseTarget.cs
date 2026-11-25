using UnityEngine;

/// <summary>
/// Put on the tree's UseZone child (trigger collider).
/// Pressing A with the machete in hand makes the player chop and the tree fade.
/// </summary>
public class TreeUseTarget : MonoBehaviour, IInteractable
{
    [Tooltip("itemId that resolves this tree, e.g. 'machete'")]
    public string requiredItemId = "machete";

    [Tooltip("Leave empty to find FallenTree on the parent.")]
    public FallenTree tree;

    private bool used;

    public void OnPickUp() { }

    public void OnUse()
    {
        if (used) return;
        if (PlayerHeldItem.Instance == null) return;
        if (!PlayerHeldItem.Instance.IsHoldingItem) return;
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId) return;

        if (tree == null) tree = GetComponentInParent<FallenTree>();
        if (tree == null)
        {
            Debug.LogWarning("[TreeUseTarget] No FallenTree found on the parent.");
            return;
        }

        used = true;

        // FallenTree fires the player's "Chop" trigger itself
        tree.Chop(PlayerHeldItem.Instance.GetComponentInChildren<Animator>());

        string itemId = PlayerHeldItem.Instance.HeldItemId;
        string itemName = PlayerHeldItem.Instance.HeldItemName;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(itemId, itemName);

        if (InventoryNotificationUI.Instance != null)
            InventoryNotificationUI.Instance.ShowMessage($"{itemName} added to inventory!");
    }
}