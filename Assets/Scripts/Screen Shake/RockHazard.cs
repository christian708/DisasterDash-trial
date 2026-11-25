using UnityEngine;

/// <summary>
/// Attach to the rock prefab. Falls straight down and deals damage + knockback
/// to the player on contact. With the helmet equipped it deals no damage and
/// slows the player instead. Self-destroys after hitting the player or after
/// a safety lifetime expires (in case it never hits anything, e.g. falls past the ground).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RockHazard : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private int damage = 1;
    [Tooltip("Safety despawn time in case the rock never hits the player or leaves the screen.")]
    [SerializeField] private float lifetime = 5f;

    [Header("Helmet")]
    [Tooltip("Player speed multiplier after being hit while wearing the helmet (0.5 = half speed).")]
    [SerializeField] private float helmetSlowMultiplier = 0.5f;
    [SerializeField] private float helmetSlowDuration = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnEnable()
    {
        if (gameObject.activeInHierarchy)
            AudioManager.Instance?.PlaySFX("falling_boulder");
    }

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        bool hasHelmet = InventoryManager.Instance != null && InventoryManager.Instance.IsEquipped("helmet");

        if (hasHelmet)
        {
            PlayerSlowEffect.Apply(other.gameObject, helmetSlowMultiplier, helmetSlowDuration);
        }
        else if (other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage, transform.position);
        }

        Destroy(gameObject);
    }
}