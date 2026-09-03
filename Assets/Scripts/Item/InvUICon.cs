using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Put this in the Inventory scene, e.g. on the Canvas or ItemsPanel.
/// </summary>
public class InvUICon : MonoBehaviour
{
    public static InvUICon Instance { get; private set; }

    [Header("Slot Grid")]
    [Tooltip("The parent Transform slots get instantiated into - ideally has a Grid Layout Group.")]
    [SerializeField] private Transform slotsParent;
    [SerializeField] private ItemSlotUI slotPrefab;

    [Header("Description Panel (ItemDes)")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image descriptionImage;
    [SerializeField] private TMP_Text descriptionNameText;
    [SerializeField] private TMP_Text descriptionBodyText;
    [SerializeField] private Button equipButton;

    private string selectedItemId;
    private bool equipListenerAdded;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (descriptionPanel != null) descriptionPanel.SetActive(false);

        PopulateSlots();

        if (equipButton != null && !equipListenerAdded)
        {
            equipButton.onClick.AddListener(OnEquipPressed);
            equipListenerAdded = true;
        }
    }

    private void PopulateSlots()
    {
        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItemData item in InventoryManager.Instance.Items)
        {
            ItemDefinition def = ItemDatabase.Instance.GetDefinition(item.itemId);
            Sprite icon = def != null ? def.icon : null;

            ItemSlotUI slot = Instantiate(slotPrefab, slotsParent);
            slot.Setup(item.itemId, item.quantity, icon, item.pickedUp);
        }
    }

    public void ShowItem(string itemId)
    {
        selectedItemId = itemId;

        ItemDefinition def = ItemDatabase.Instance.GetDefinition(itemId);
        if (def == null) return;

        if (descriptionImage != null) descriptionImage.sprite = def.icon;
        if (descriptionNameText != null) descriptionNameText.text = def.itemName;
        if (descriptionBodyText != null) descriptionBodyText.text = def.description;

        if (descriptionPanel != null) descriptionPanel.SetActive(true);
    }

    private void OnEquipPressed()
    {
        if (string.IsNullOrEmpty(selectedItemId)) return;

        ItemDefinition def = ItemDatabase.Instance.GetDefinition(selectedItemId);
        if (def == null) return;

        InventoryManager.Instance.SetEquippedItem(def.itemId, def.itemName, def.requiredTargetTag);

        // BagPanel is an overlay in the same scene, so PlayerHeldItem's Start()
        // already ran and won't fire again - equip directly, live, right now.
        if (PlayerHeldItem.Instance != null)
        {
            PlayerHeldItem.Instance.PickUp(def.itemId, def.itemName, def.requiredTargetTag);
        }

        if (descriptionPanel != null) descriptionPanel.SetActive(false);
    }
}