using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Central inventory store. Put this on an empty GameObject in your first-loaded
/// scene (e.g. a "Managers" object). It persists across scenes and autosaves to JSON.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<InventoryItemData> Items { get; private set; } = new List<InventoryItemData>();

    // Currently held item (fire extinguisher, towel, bucket...). Wearables never go in here.
    public string EquippedItemId { get; private set; }
    public string EquippedItemName { get; private set; }
    public string EquippedItemRequiredTargetTag { get; private set; }

    // Wearables: max 2 can be equipped at the same time.
    // IDs must match the itemId values in item_database.json exactly.
    // NOTE: Unity keeps the old list on an existing component, so also check it in the Inspector.
    [SerializeField]
    private List<string> wearableItemIds = new List<string>
    {
        "raincoat", "boots", "helmet", "shoe", "mask", "mask45", "lifevest"
    };

    private const int MaxWearables = 2;

    // Kept in equip order, so the oldest one is at index 0.
    private readonly List<string> equippedWearables = new List<string>();

    public IReadOnlyList<string> WearableItemIds => wearableItemIds;
    public int WearablesVersion { get; private set; }

    public event Action<InventoryItemData> OnItemAdded;
    public event Action<string> OnItemRemoved;

    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadInventory();
    }

    private void Start()
    {
        EnsureFullRoster();
    }

    private void EnsureFullRoster()
    {
        if (ItemDatabase.Instance == null) return;

        bool changed = false;

        foreach (string itemId in ItemDatabase.Instance.GetAllItemIds())
        {
            if (!Items.Exists(i => i.itemId == itemId))
            {
                ItemDefinition def = ItemDatabase.Instance.GetDefinition(itemId);
                string name = def != null ? def.itemName : itemId;
                Items.Add(new InventoryItemData(itemId, name, 0, false));
                changed = true;
            }
        }

        if (changed) SaveInventory();
    }

    public void AddItem(string itemId, string itemName, int qty = 1)
    {
        InventoryItemData existing = Items.Find(i => i.itemId == itemId);

        if (existing != null)
        {
            existing.quantity += qty;
            existing.pickedUp = true;
        }
        else
        {
            existing = new InventoryItemData(itemId, itemName, qty, true);
            Items.Add(existing);
        }

        SaveInventory();
        OnItemAdded?.Invoke(existing);
        Debug.Log($"[InventoryManager] Added '{itemId}' - now have {existing.quantity}. Total unique items: {Items.Count}");
    }

    public bool HasItem(string itemId)
    {
        return Items.Exists(i => i.itemId == itemId);
    }

    public bool IsWearable(string itemId)
    {
        return !string.IsNullOrEmpty(itemId) && wearableItemIds.Contains(itemId);
    }

    public bool IsWearableEquipped(string itemId)
    {
        return equippedWearables.Contains(itemId);
    }

    /// <summary>
    /// True if the item is an equipped wearable, or is the currently held item.
    /// Use this in hazard scripts instead of comparing EquippedItemId / HeldItemId.
    /// </summary>
    public bool IsEquipped(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        return equippedWearables.Contains(itemId) || EquippedItemId == itemId;
    }

    public void SetEquippedItem(string itemId, string itemName, string requiredTargetTag)
    {
        // Wearables go into their own list (max 2) and never touch the held item.
        if (IsWearable(itemId))
        {
            if (equippedWearables.Contains(itemId)) return; // already worn

            if (equippedWearables.Count >= MaxWearables)
            {
                equippedWearables.RemoveAt(0); // take off the oldest one
            }

            equippedWearables.Add(itemId);
            WearablesVersion++;
            return;
        }

        EquippedItemId = itemId;
        EquippedItemName = itemName;
        EquippedItemRequiredTargetTag = requiredTargetTag;

        // Keep PlayerHeldItem in sync the moment an item is equipped.
        if (PlayerHeldItem.Instance != null)
        {
            PlayerHeldItem.Instance.PickUp(itemId, itemName, requiredTargetTag);
        }
    }

    /// <summary>
    /// Clears the held item only (fire extinguisher, towel, bucket...).
    /// Wearables stay on; use UnequipWearable to take one off.
    /// </summary>
    public void ClearEquippedItem()
    {
        EquippedItemId = null;
        EquippedItemName = null;
        EquippedItemRequiredTargetTag = null;

        if (PlayerHeldItem.Instance != null)
        {
            PlayerHeldItem.Instance.ClearHeldItem();
        }
    }

    /// <summary>
    /// Take off one specific wearable without touching the other.
    /// </summary>
    public void UnequipWearable(string itemId)
    {
        if (equippedWearables.Remove(itemId)) WearablesVersion++;
    }

    /// <summary>
    /// Call this from your New Game flow (alongside ResetProgressPlayerPrefs())
    /// to wipe all picked-up items back to locked/greyed-out.
    /// </summary>
    public void ResetInventory()
    {
        Items.Clear();
        equippedWearables.Clear();
        WearablesVersion++;
        EquippedItemId = null;
        EquippedItemName = null;
        EquippedItemRequiredTargetTag = null;

        SaveInventory();
        EnsureFullRoster();
    }

    public void RemoveItem(string itemId, int qty = 1)
    {
        InventoryItemData existing = Items.Find(i => i.itemId == itemId);
        if (existing == null) return;

        existing.quantity -= qty;
        if (existing.quantity <= 0)
        {
            Items.Remove(existing);
            if (equippedWearables.Remove(itemId)) WearablesVersion++;
        }

        SaveInventory();
        OnItemRemoved?.Invoke(itemId);
    }

    public void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData { items = Items };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void LoadInventory()
    {
        if (!File.Exists(SavePath))
        {
            Items = new List<InventoryItemData>();
            return;
        }

        string json = File.ReadAllText(SavePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
        Items = data != null ? data.items : new List<InventoryItemData>();
    }
}