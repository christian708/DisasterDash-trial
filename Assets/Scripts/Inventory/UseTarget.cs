using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to anything a held item can be used on (a fire, a spill, etc.).
/// Each target has exactly ONE specific item that resolves it - set
/// requiredItemId to match the itemId of the PickableItem that should work here.
/// </summary>
public class UseTarget : MonoBehaviour, IInteractable
{
    [Tooltip("The one specific itemId that resolves this target, e.g. 'fire_extinguisher'")]
    public string requiredItemId;

    [Tooltip("Optional effect (smoke puff, sparkle, etc.) spawned when resolved")]
    public GameObject resolveEffectPrefab;

    [Tooltip("What actually gets deactivated when resolved. Leave empty to deactivate this object itself; assign the parent Fire object if UseTarget sits on a separate wider-range child.")]
    public GameObject objectToDeactivate;

    [Header("Relight (optional)")]
    [Tooltip("How many times this target can relight after being resolved. 0 = never relights (stays resolved permanently). e.g. 3 means it can flare back up 3 times, then stays out for good on the 4th extinguish.")]
    public int maxRelights = 0;
    [Tooltip("Seconds before the target comes back, each time it relights.")]
    public float relightDelay = 5f;

    [Tooltip("Optional popup shown when this target relights (e.g. TipPopup_Extinguisher). Leave empty for no popup.")]
    public GameObject relightPopup;

    private int timesRelit = 0;

    public void OnPickUp()
    {
        // Targets aren't picked up.
    }

    public void OnUse()
    {
        if (!PlayerHeldItem.Instance.IsHoldingItem) return;
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId) return;

        Animator playerAnimator = PlayerHeldItem.Instance.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("BoyEx");
        }

        GameObject target = objectToDeactivate != null ? objectToDeactivate : gameObject;

        if (resolveEffectPrefab != null)
        {
            Instantiate(resolveEffectPrefab, target.transform.position, Quaternion.identity);
        }

        InventoryManager.Instance.AddItem(PlayerHeldItem.Instance.HeldItemId, PlayerHeldItem.Instance.HeldItemName);
        InventoryNotificationUI.Instance.ShowMessage($"{PlayerHeldItem.Instance.HeldItemName} added to inventory!");

        target.SetActive(false);

        if (timesRelit < maxRelights)
        {
            timesRelit++;
            // Runs on a separate always-active object so the timer survives
            // this target's GameObject being turned off.
            CoroutineRunner.Instance.StartCoroutine(RelightAfterDelay(target));
        }
    }

    private IEnumerator RelightAfterDelay(GameObject target)
    {
        yield return new WaitForSeconds(relightDelay);

        if (target != null)
        {
            target.SetActive(true);

            if (relightPopup != null)
            {
                relightPopup.SetActive(true);
            }
        }
    }
}