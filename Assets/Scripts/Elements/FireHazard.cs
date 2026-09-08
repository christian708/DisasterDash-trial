using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageInterval = 1.5f;
    [SerializeField] private int damageAmount = 1;

    private float lastDamageTime = -999f;
    private PlayerHealth playerHealth;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[FireHazard] Trigger entered by: {other.name}, tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            TryDamage(transform.position);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryDamage(transform.position);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
        }
    }

    private void TryDamage(Vector2 firePosition)
    {
        Debug.Log($"[FireHazard] TryDamage called. enabled={this.enabled}, playerHealth={(playerHealth != null ? "set" : "null")}");
        if (playerHealth == null) return;

        if (Time.time - lastDamageTime >= damageInterval)
        {
            playerHealth.TakeDamage(damageAmount, firePosition);
            lastDamageTime = Time.time;
        }
    }
}