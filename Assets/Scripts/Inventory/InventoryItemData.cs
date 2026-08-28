using System;
using System.Collections.Generic;

/// <summary>
/// A single item entry stored in the inventory.
/// </summary>
[Serializable]
public class InventoryItemData
{
    public string itemId;
    public string itemName;
    public string iconPath; // Resources path, e.g. "Icons/fire_extinguisher"
    public int quantity;

    public InventoryItemData(string id, string name, string icon, int qty = 1)
    {
        itemId = id;
        itemName = name;
        iconPath = icon;
        quantity = qty;
    }
}

/// <summary>
/// JsonUtility can't serialize a top-level List directly, so we wrap it.
/// </summary>
[Serializable]
public class InventorySaveData
{
    public List<InventoryItemData> items = new List<InventoryItemData>();
}
