using UnityEngine;
using System.Collections;

/// <summary>
/// One flood surge: rises into view at knee height, stays active (slowing
/// the player if they're standing in it), then recedes and cleans itself up.
/// Meant to be spawned by FloodSpawner at random positions along the level.
/// If the player has boots or a life vest equipped, the slow effect is skipped.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FloodSurge : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float riseDuration = 0.5f;
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float recedeDuration = 0.5f;

    [Header("Visual")]
    [Tooltip("Child object with the water surface sprite - scales up/down for rise and recede.")]
    [SerializeField] private Transform waterVisual;

    [Header("Effect")]
    [SerializeField] private float playerSpeedMultiplier = 0.5f; // 5 -> 2.5

    [Header("Protection")]
    [Tooltip("itemId of the boots in item_database.json")]
    [SerializeField] private string bootsItemId = "boots";
    [Tooltip("itemId of the life vest in item_database.json")]
    [SerializeField] private string lifeVestItemId = "lifevest";

    private Collider2D col;
    private PlayerController playerInZone;
    private Vector3 fullScale;
    private bool isSlowed;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;

        if (waterVisual != null)
        {
            fullScale = waterVisual.localScale;
            waterVisual.localScale = new Vector3(fullScale.x, 0f, fullScale.z);
        }
    }

    private void Start()
    {
        if (gameObject.activeInHierarchy)
            AudioManager.Instance?.PlaySFX("flood_walk");
        StartCoroutine(SurgeSequence());
    }

    private IEnumerator SurgeSequence()
    {
        // Rise
        float t = 0f;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / riseDuration);
            if (waterVisual != null) waterVisual.localScale = new Vector3(fullScale.x, fullScale.y * p, fullScale.z);
            yield return null;
        }
        if (waterVisual != null) waterVisual.localScale = fullScale;
        col.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        // Recede
        col.enabled = false;
        t = 0f;
        while (t < recedeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / recedeDuration);
            if (waterVisual != null) waterVisual.localScale = new Vector3(fullScale.x, fullScale.y * (1f - p), fullScale.z);
            yield return null;
        }

        // Safety: make sure the player isn't left permanently slowed if the
        // surge recedes while they're still standing in it.
        if (playerInZone != null)
        {
            playerInZone.SetSpeedMultiplier(1f);
            playerInZone = null;
        }
        isSlowed = false;

        Destroy(gameObject);
    }

    /// <summary>
    /// Returns true if the player has either boots or a life vest equipped —
    /// either item is enough to skip the slow effect.
    /// </summary>
    private bool HasProtectionEquipped()
    {
        if (InventoryManager.Instance == null) return false;
        return InventoryManager.Instance.IsEquipped(bootsItemId) ||
               InventoryManager.Instance.IsEquipped(lifeVestItemId);
    }

    private void ApplyEffect()
    {
        if (playerInZone == null) return;

        bool shouldSlow = !HasProtectionEquipped();
        if (shouldSlow == isSlowed) return;

        isSlowed = shouldSlow;
        playerInZone.SetSpeedMultiplier(shouldSlow ? playerSpeedMultiplier : 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        playerInZone = pc;
        isSlowed = false;
        ApplyEffect();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        ApplyEffect();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null) pc.SetSpeedMultiplier(1f);
        if (playerInZone == pc) playerInZone = null;
        isSlowed = false;
    }
}