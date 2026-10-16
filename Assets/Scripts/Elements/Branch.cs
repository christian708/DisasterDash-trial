using UnityEngine;

/// <summary>
/// Attach to a falling hazard prefab (tree branch, rock, etc.) - this is
/// what EquakeManager's "Rock Prefab" field should point to. Falls straight
/// down, deals damage and destroys itself on hitting the player, and cleans
/// itself up if it falls off the bottom of the level without being hit.
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
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, transform.position);
            }

            Destroy(gameObject);
        }
    }
}