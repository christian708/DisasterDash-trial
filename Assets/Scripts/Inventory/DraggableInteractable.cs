using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to any world item the player can pick up or use (fire extinguisher, towel, etc.).
/// Requires a Collider2D on the object, and a Physics2DRaycaster on the main camera
/// so the EventSystem can route pointer/touch events to it. This is separate from your
/// mobile movement joystick input, so it won't fight with player-avatar controls.
///
/// TAP (no meaningful movement)  -> item is collected straight into inventory.
/// DRAG onto a matching UseTarget -> target is resolved (e.g. fire put out) AND the
/// item is also logged into inventory before disappearing.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class DraggableInteractable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Item Info")]
    public string itemId;
    public string itemName;
    [Tooltip("Assign the item's sprite directly here - not persisted to JSON, just for display purposes.")]
    public Sprite icon;

    [Header("Behavior")]
    [Tooltip("Tag of the object this item can be used on, e.g. 'Fire'. Leave empty if this item is tap-to-collect only.")]
    public string requiredTargetTag = "";
    public bool destroyOnUse = true;

    private Vector3 startPosition;
    private Vector3 dragOffset;
    private bool isDragging;
    private bool didDragEnoughToCountAsDrag;
    private const float TAP_THRESHOLD = 0.15f; // world units of movement before it counts as a drag

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
        startPosition = transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[DraggableInteractable] OnPointerDown on {gameObject.name}");
        isDragging = true;
        didDragEnoughToCountAsDrag = false;

        Vector3 worldPoint = ScreenToWorld(eventData.position);
        dragOffset = transform.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 worldPoint = ScreenToWorld(eventData.position);
        Vector3 newPos = worldPoint + dragOffset;
        newPos.z = startPosition.z;

        if (Vector3.Distance(newPos, startPosition) > TAP_THRESHOLD)
            didDragEnoughToCountAsDrag = true;

        transform.position = newPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (!didDragEnoughToCountAsDrag)
        {
            CollectItem();
            return;
        }

        if (!string.IsNullOrEmpty(requiredTargetTag))
        {
            Collider2D hit = Physics2D.OverlapPoint(transform.position);
            if (hit != null && hit.CompareTag(requiredTargetTag))
            {
                UseItemOnTarget(hit.gameObject);
                return;
            }
        }

        // Dropped somewhere invalid -> snap back to where it started
        transform.position = startPosition;
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        return mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCam.transform.position.z));
    }

    private void CollectItem()
    {
        InventoryManager.Instance.AddItem(itemId, itemName);
        InventoryNotificationUI.Instance.ShowMessage($"{itemName} added to inventory!");
        gameObject.SetActive(false);
    }

    private void UseItemOnTarget(GameObject target)
    {
        UseTarget useTarget = target.GetComponent<UseTarget>();
        if (useTarget != null)
        {
            useTarget.OnUsedBy(itemId);
        }

        InventoryManager.Instance.AddItem(itemId, itemName);
        InventoryNotificationUI.Instance.ShowMessage($"{itemName} added to inventory!");

        if (destroyOnUse)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = startPosition;
        }
    }
}