using UnityEngine;

public class TreeChopZone : MonoBehaviour, IInteractable
{
    [Tooltip("itemId that resolves this tree, e.g. 'machete'")]
    public string requiredItemId = "machete";

    [Tooltip("Leave empty to find FallenTree on the parent.")]
    public FallenTree tree;

    private bool used;

    private void Awake()
    {
        Debug.Log("[TreeChopZone] loaded on " + gameObject.name);
    }

    public void OnPickUp() { }

    public void OnUse()
    {
        Debug.Log("[TreeChopZone] OnUse called");

        if (used) return;
        if (PlayerHeldItem.Instance == null) { Debug.Log("[TreeChopZone] no PlayerHeldItem"); return; }
        if (!PlayerHeldItem.Instance.IsHoldingItem) { Debug.Log("[TreeChopZone] not holding anything"); return; }
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId)
        {
            Debug.Log("[TreeChopZone] held='" + PlayerHeldItem.Instance.HeldItemId + "' required='" + requiredItemId + "'");
            return;
        }

        if (tree == null) tree = GetComponentInParent<FallenTree>();
        if (tree == null) { Debug.LogWarning("[TreeChopZone] no FallenTree on the parent"); return; }

        used = true;
        Debug.Log("[TreeChopZone] chopping");

        tree.Chop(PlayerHeldItem.Instance.GetComponentInChildren<Animator>());

        string itemId = PlayerHeldItem.Instance.HeldItemId;
        string itemName = PlayerHeldItem.Instance.HeldItemName;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(itemId, itemName);

        if (InventoryNotificationUI.Instance != null)
            InventoryNotificationUI.Instance.ShowMessage($"{itemName} added to inventory!");
    }
}