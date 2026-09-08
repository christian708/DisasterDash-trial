using UnityEngine;

/// <summary>
/// Attach to the rock prefab. Falls straight down and deals damage + knockback
/// to the player on contact. Self-destroys after hitting the player or after
/// a safety lifetime expires (in case it never hits anything, e.g. falls past the ground).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class RockHazard : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private int damage = 1;
    [Tooltip("Safety despawn time in case the rock never hits the player or leaves the screen.")]
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}