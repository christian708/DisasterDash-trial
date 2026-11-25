using UnityEngine;

/// <summary>
/// Attach to a falling hazard prefab (tree branch, rock, etc.) - this is
/// what EquakeManager's "Debris Prefabs" field should point to. Falls straight
/// down, deals damage and destroys itself on hitting the player (or slows the
/// player instead if the helmet is equipped), and cleans itself up if it falls
/// off the bottom of the level without being hit.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Branch : MonoBehaviour
{
    [Header("Falling")]
    [SerializeField] private float fallSpeed = 4f;
    [Tooltip("Once the debris falls below this world Y, it destroys itself (safety cleanup if it never hits anything).")]
    [SerializeField] private float destroyYThreshold = -10f;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    [Header("Helmet")]
    [Tooltip("Player speed multiplier after being hit while wearing the helmet (0.5 = half speed).")]
    [SerializeField] private float helmetSlowMultiplier = 0.5f;
    [SerializeField] private float helmetSlowDuration = 2f;

    private void OnEnable()
    {
        // Only play sound if the object is truly active in the hierarchy,
        // not when instantiated as a template (immediately set inactive)
        if (gameObject.activeInHierarchy)
            AudioManager.Instance?.PlaySFX("falling_branches");
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < destroyYThreshold)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        bool hasHelmet = InventoryManager.Instance != null && InventoryManager.Instance.IsEquipped("helmet");

        if (hasHelmet)
        {
            PlayerSlowEffect.Apply(other.gameObject, helmetSlowMultiplier, helmetSlowDuration);
        }
        else
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, transform.position);
            }
        }

        Destroy(gameObject);
    }
}