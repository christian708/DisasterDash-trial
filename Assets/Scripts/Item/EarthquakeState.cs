using System;

/// <summary>
/// Not attached to anything. Checks whether the player is currently holding the shoes.
/// </summary>
public static class EarthquakeState
{
    // Must match the "Item Id" on your shoes' PickItem component (capitalization ignored).
    public const string ShoesItemId = "shoe";

    public static bool HasShoes
    {
        get
        {
            // Main check: item picked up with the A button
            if (PlayerHeldItem.Instance != null
                && PlayerHeldItem.Instance.IsHoldingItem
                && Matches(PlayerHeldItem.Instance.HeldItemId))
                return true;

            // Backup: item equipped through InventoryManager
            if (InventoryManager.Instance != null
                && Matches(InventoryManager.Instance.EquippedItemId))
                return true;

            return false;
        }
    }

    private static bool Matches(string id)
    {
        return !string.IsNullOrEmpty(id)
            && string.Equals(id, ShoesItemId, StringComparison.OrdinalIgnoreCase);
    }
}