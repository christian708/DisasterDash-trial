using UnityEngine;
using System.Collections;

/// <summary>
/// One lightning strike sequence: warning glow -> brief strike window that
/// deals damage if the player is standing in it -> cleans itself up.
/// Meant to be spawned by LightningSpawner at random positions.
/// </summary>
public class LightningHazard : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float warningDuration = 1f;
    [SerializeField] private float strikeDuration = 0.3f;

    [Header("Visuals")]
    [Tooltip("Glow shown during the warning delay before the strike.")]
    [SerializeField] private GameObject warningGlow;
    [Tooltip("Lightning bolt visual shown during the actual strike.")]
    [SerializeField] private GameObject strikeVisual;

    [Header("Damage")]
    [SerializeField] private Collider2D damageCollider; // keep disabled by default in the Inspector
    [SerializeField] private int damageAmount = 1;

    private PlayerHealth playerHealth;

    private void Start()
    {
        if (damageCollider != null) damageCollider.enabled = false;
        if (strikeVisual != null) strikeVisual.SetActive(false);
        if (warningGlow != null) warningGlow.SetActive(true);

        StartCoroutine(StrikeSequence());
    }

    private IEnumerator StrikeSequence()
    {
        yield return new WaitForSeconds(warningDuration);

        if (warningGlow != null) warningGlow.SetActive(false);
        if (strikeVisual != null) strikeVisual.SetActive(true);
        if (damageCollider != null) damageCollider.enabled = true;

        yield return new WaitForSeconds(strikeDuration);

        if (damageCollider != null) damageCollider.enabled = false;
        if (strikeVisual != null) strikeVisual.SetActive(false);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) => HandleTriggerEnter(other);
    private void OnTriggerStay2D(Collider2D other) => HandleTriggerStay(other);
    private void OnTriggerExit2D(Collider2D other) => HandleTriggerExit(other);

    /// <summary>
    /// Call these from a relay script if the damage collider sits on a
    /// separate child object instead of this root object (see LightningDamageRelay.cs).
    /// </summary>
    public void HandleTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            TryDamage();
        }
    }

    public void HandleTriggerStay(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryDamage();
        }
    }

    public void HandleTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
        }
    }

    private void TryDamage()
    {
        if (playerHealth == null) return;
        playerHealth.TakeDamage(damageAmount, transform.position);
    }
}