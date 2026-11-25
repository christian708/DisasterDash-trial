using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SmokeHazard : MonoBehaviour
{
    [Header("Mask")]
    public string maskItemId = "mask45";

    [Header("Effects")]
    [Range(0.1f, 1f)] public float slowMultiplier = 0.5f;
    public int damage = 1;
    public float damageInterval = 1.5f;

    private float damageTimer;

    // Cached player references — set on Enter, cleared on Exit
    private PlayerController cachedPC;
    private PlayerHealth cachedHealth;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        cachedPC     = other.GetComponentInParent<PlayerController>();
        cachedHealth = other.GetComponentInParent<PlayerHealth>();
        damageTimer  = 0f;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (cachedPC != null) cachedPC.SetSpeedMultiplier(slowMultiplier);

        if (HasMask()) return;

        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0f)
        {
            damageTimer = damageInterval;
            if (cachedHealth != null)
                cachedHealth.TakeDamage(damage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (cachedPC != null) cachedPC.SetSpeedMultiplier(1f);
        cachedPC     = null;
        cachedHealth = null;
    }

    // Direct call — no reflection
    private bool HasMask()
    {
        return InventoryManager.Instance != null &&
               InventoryManager.Instance.IsEquipped(maskItemId);
    }
}
