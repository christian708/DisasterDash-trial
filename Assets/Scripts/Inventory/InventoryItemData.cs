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
    public int quantity;
    public bool pickedUp;

    public InventoryItemData(string id, string name, int qty = 0, bool pickedUp = false)
    {
        itemId = id;
        itemName = name;
        quantity = qty;
        this.pickedUp = pickedUp;
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