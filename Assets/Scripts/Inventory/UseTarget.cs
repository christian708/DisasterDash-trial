using UnityEngine;

/// <summary>
/// Attach to anything a draggable item can be used on (the fire, a spill, etc.).
/// Give the GameObject a matching Tag (e.g. "Fire") and set requiredItemId to the
/// itemId of the DraggableInteractable that should resolve it.
/// </summary>
public class UseTarget : MonoBehaviour
{
    [Tooltip("Must match the itemId on the DraggableInteractable that resolves this, e.g. 'fire_extinguisher'")]
    public string requiredItemId;

    [Tooltip("Optional effect (smoke puff, sparkle, etc.) spawned when resolved")]
    public GameObject resolveEffectPrefab;

    public void OnUsedBy(string incomingItemId)
    {
        if (incomingItemId != requiredItemId) return;

        if (resolveEffectPrefab != null)
        {
            Instantiate(resolveEffectPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}
