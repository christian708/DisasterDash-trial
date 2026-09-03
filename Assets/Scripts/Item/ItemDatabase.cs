using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemDefinition
{
    public string itemId;
    public string itemName;
    public Sprite icon;
    public string description;
    public string requiredTargetTag;
}

[System.Serializable]
public class ItemDefinitionData
{
    public string itemId;
    public string itemName;
    public string description;
    public string requiredTargetTag;
}

[System.Serializable]
public class ItemDatabaseJson
{
    public List<ItemDefinitionData> items = new List<ItemDefinitionData>();
}

[System.Serializable]
public class ItemIconEntry
{
    public string itemId;
    public Sprite icon;
}

/// <summary>
/// Put this on the same GameObject as InventoryManager (e.g. INVManager) so it
/// persists across scenes too.
///
/// Text data (name/description/requiredTargetTag) loads from
/// Assets/Resources/item_database.json at startup - edit that file directly.
///
/// Icons can't be stored in JSON (Sprites aren't serializable to text), so
/// they're assigned directly in the Inspector list below, matched by itemId.
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private List<ItemIconEntry> icons = new List<ItemIconEntry>();

    private Dictionary<string, ItemDefinitionData> textData = new Dictionary<string, ItemDefinitionData>();
    private List<string> orderedItemIds = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("item_database");
        if (jsonAsset == null)
        {
            Debug.LogWarning("[ItemDatabase] Could not find Resources/item_database.json");
            return;
        }

        ItemDatabaseJson data = JsonUtility.FromJson<ItemDatabaseJson>(jsonAsset.text);

        textData.Clear();
        orderedItemIds.Clear();
        foreach (ItemDefinitionData entry in data.items)
        {
            textData[entry.itemId] = entry;
            orderedItemIds.Add(entry.itemId);
        }
    }

    public List<string> GetAllItemIds()
    {
        return orderedItemIds;
    }

    public ItemDefinition GetDefinition(string itemId)
    {
        if (!textData.TryGetValue(itemId, out ItemDefinitionData text))
            return null;

        ItemIconEntry iconEntry = icons.Find(i => i.itemId == itemId);

        return new ItemDefinition
        {
            itemId = text.itemId,
            itemName = text.itemName,
            description = text.description,
            requiredTargetTag = text.requiredTargetTag,
            icon = iconEntry != null ? iconEntry.icon : null
        };
    }
}