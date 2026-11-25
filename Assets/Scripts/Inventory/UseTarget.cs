using UnityEngine;
using System.Collections;

public class UseTarget : MonoBehaviour, IInteractable
{
    [Tooltip("The one specific itemId that resolves this target, e.g. 'fire_extinguisher'")]
    public string requiredItemId;

    [Tooltip("Optional effect (smoke puff, sparkle, etc.) spawned when resolved")]
    public GameObject resolveEffectPrefab;

    [Tooltip("What actually gets deactivated when resolved. Leave empty to deactivate this object itself; assign the parent Fire object if UseTarget sits on a separate wider-range child.")]
    public GameObject objectToDeactivate;

    [Header("Player Animation")]
    [Tooltip("Animator trigger fired on the player when used, e.g. BoyEx or Tow")]
    public string playerAnimTrigger = "BoyEx";

    [Header("Sound")]
    [Tooltip("SFX ID played from AudioManager when this target is resolved (e.g. 'fire_extinguish', 'fire_pour').")]
    public string resolveSoundId = "";

    [Header("Relight (optional)")]
    public int maxRelights = 0;
    public float relightDelay = 5f;
    public GameObject relightPopup;

    private int timesRelit = 0;

    public void OnPickUp() { }

    public void OnUse()
    {
        if (PlayerHeldItem.Instance == null) return;
        if (!PlayerHeldItem.Instance.IsHoldingItem) return;
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId) return;

        Animator playerAnimator = PlayerHeldItem.Instance.GetComponentInChildren<Animator>();
        if (playerAnimator != null && !string.IsNullOrEmpty(playerAnimTrigger))
        {
            playerAnimator.SetTrigger(playerAnimTrigger);
        }

        GameObject target = objectToDeactivate != null ? objectToDeactivate : gameObject;

        // Play resolve sound (fire_extinguish, fire_pour, etc.)
        if (!string.IsNullOrEmpty(resolveSoundId))
            AudioManager.Instance?.PlaySFX(resolveSoundId);

        if (resolveEffectPrefab != null)
        {
            Instantiate(resolveEffectPrefab, target.transform.position, Quaternion.identity);
        }

        string itemId = PlayerHeldItem.Instance.HeldItemId;
        string itemName = PlayerHeldItem.Instance.HeldItemName;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(itemId, itemName);

        if (InventoryNotificationUI.Instance != null)
            InventoryNotificationUI.Instance.ShowMessage($"{itemName} added to inventory!");

        target.SetActive(false);

        if (timesRelit < maxRelights && CoroutineRunner.Instance != null)
        {
            timesRelit++;
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