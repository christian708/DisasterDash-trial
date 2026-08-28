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

        public void AddItem(string itemId, string itemName, int qty = 1)
    {
        InventoryItemData existing = Items.Find(i => i.itemId == itemId);

        if (existing != null)
        {
            existing.quantity += qty;
        }
        else
        {
            existing = new InventoryItemData(itemId, itemName, qty.ToString());
            Items.Add(existing);
        }

        SaveInventory();
        OnItemAdded?.Invoke(existing);
    }

    public bool HasItem(string itemId)
    {
        return Items.Exists(i => i.itemId == itemId);
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
