using System.Collections;
using UnityEngine;

/// <summary>
/// Attach directly to the fire GameObject itself (sprite + collider all on one object).
/// Implements IInteractable so your existing PlayerInteractor can detect and call it.
///
/// Supports "relighting": after being put out, the fire reappears after a delay,
/// up to a set number of times, showing a warning popup each relight. After the
/// final extinguish, it stays out permanently.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class UseTarget : MonoBehaviour, IInteractable
{
    [Tooltip("Must match the itemId on the PickableItem that resolves this, e.g. 'fire_extinguisher'")]
    public string requiredItemId;

    [Tooltip("Optional effect (smoke puff, sparkle, etc.) spawned when resolved")]
    public GameObject resolveEffectPrefab;

    [Header("Relight Settings")]
    [Tooltip("How many times this fire can be put out before it stays out for good.")]
    public int maxExtinguishes = 3;

    [Tooltip("Seconds after being put out before the fire relights (if under maxExtinguishes).")]
    public float relightDelay = 3f;

    [Tooltip("Popup shown when the fire relights, warning the player again. Leave empty for none. Starts inactive.")]
    public GameObject relightTipPanel;

    private int timesExtinguished = 0;
    private bool isPermanentlyOut = false;
    private bool hasShownRelightTip = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D fireCollider;
    private Animator animator;
    private ParticleSystem particles;
    private FireHazard fireHazard;
    private Collider2D fireHazardCollider;

    private void Awake()
    {
        // Collider lives on this same object (the range/trigger object).
        fireCollider = GetComponent<Collider2D>();

        // Visuals (sprite, animator, particles) may live on this object OR a
        // parent (e.g. this script sits on "Fire1_Range" while the actual
        // flame sprite/animator is on the parent "Fire1"). GetComponentInParent
        // checks this object first, then walks upward, so it works either way.
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        animator = GetComponentInParent<Animator>();
        particles = GetComponentInParent<ParticleSystem>();

        // FireHazard (the damage script) may live on a different object than
        // this collider too — same search pattern. It also has its OWN
        // Collider2D (separate from fireCollider above), which keeps
        // generating trigger events even if the FireHazard script itself is
        // disabled — so we must disable that specific collider directly.
        fireHazard = GetComponentInParent<FireHazard>();
        if (fireHazard != null)
        {
            fireHazardCollider = fireHazard.GetComponent<Collider2D>();
        }
    }

    public void OnPickUp()
    {
        // Targets aren't picked up.
    }

    public void OnUse()
    {
        if (isPermanentlyOut) return;
        if (!PlayerHeldItem.Instance.IsHoldingItem) return;
        if (PlayerHeldItem.Instance.HeldItemId != requiredItemId) return;

        Animator playerAnimator = PlayerHeldItem.Instance.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("BoyEx");
        }

        if (resolveEffectPrefab != null)
        {
            Instantiate(resolveEffectPrefab, transform.position, Quaternion.identity);
        }

        string heldItemId = PlayerHeldItem.Instance.HeldItemId;
        string heldItemName = PlayerHeldItem.Instance.HeldItemName;

        PlayerHeldItem.Instance.ClearHeldItem();
        SetFireVisible(false); // fire goes out immediately, GameObject stays active

        timesExtinguished++;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(heldItemId, heldItemName);
        }
        else
        {
            Debug.LogWarning("[UseTarget] InventoryManager.Instance was null — item not added back to inventory.");
        }

        if (InventoryNotificationUI.Instance != null)
        {
            InventoryNotificationUI.Instance.ShowMessage($"{heldItemName} added to inventory!");
        }
        else
        {
            Debug.LogWarning("[UseTarget] InventoryNotificationUI.Instance was null — message not shown.");
        }

        if (timesExtinguished >= maxExtinguishes)
        {
            isPermanentlyOut = true;
        }
        else
        {
            StartCoroutine(RelightAfterDelay());
        }
    }

    private IEnumerator RelightAfterDelay()
    {
        yield return new WaitForSeconds(relightDelay);

        SetFireVisible(true);

        // Only ever show this popup once per fire, on its very first relight.
        // The player must close it manually via its own X button from here on.
        if (relightTipPanel != null && !hasShownRelightTip)
        {
            relightTipPanel.SetActive(true);
            hasShownRelightTip = true;
        }
    }

    /// <summary>
    /// Toggles the fire's visuals and collider without disabling the GameObject
    /// itself, so this script (and its running coroutine) stays alive while
    /// the fire is "out."
    /// </summary>
    private void SetFireVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
        if (fireCollider != null) fireCollider.enabled = visible;
        if (animator != null) animator.enabled = visible;
        if (fireHazard != null) fireHazard.enabled = visible;
        if (fireHazardCollider != null) fireHazardCollider.enabled = visible; // actually stops trigger events

        if (particles != null)
        {
            if (visible) particles.Play();
            else particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}