using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    [SerializeField] private int healAmount = 1;

    [Header("Optional")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPos;
    private bool consumed = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Gentle floating effect
        float offsetY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + new Vector3(0f, offsetY, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryConsume(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Lets the potion be consumed after the player drops below full
        // while still standing on it.
        TryConsume(other);
    }

    private void TryConsume(Collider2D other)
    {
        if (consumed) return;

        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        // Returns false if already at full hearts -> potion stays
        if (!health.TryHeal(healAmount)) return;

        consumed = true;

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Play healing sound through AudioManager
        AudioManager.Instance?.PlaySFX("heal");

        Destroy(gameObject);
    }
}