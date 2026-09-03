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

    public string EquippedItemId { get; private set; }
    public string EquippedItemName { get; private set; }
    public string EquippedItemRequiredTargetTag { get; private set; }

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

    /// <summary>
    /// Makes sure every item defined in ItemDatabase has an entry here (even if
    /// pickedUp = false), so locked/unpicked items can still show as greyed-out
    /// slots in the UI. Runs in Start() so ItemDatabase's Awake has already loaded.
    /// </summary>
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

    public void SetEquippedItem(string itemId, string itemName, string requiredTargetTag)
    {
        EquippedItemId = itemId;
        EquippedItemName = itemName;
        EquippedItemRequiredTargetTag = requiredTargetTag;
    }

    /// <summary>
    /// Call this from your New Game flow (alongside ResetProgressPlayerPrefs())
    /// to wipe all picked-up items back to locked/greyed-out.
    /// </summary>
    public void ResetInventory()
    {
        Items.Clear();
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