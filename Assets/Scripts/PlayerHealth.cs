using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Heart UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Health Settings")]
    [SerializeField] private int maxHearts = 3;
    [SerializeField] private float invincibilityTime = 1f;

    [Header("Damage Feedback")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private float shakeMagnitude = 0.08f;
    [SerializeField] private float shakeDuration = 0.25f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalLocalPosition;

    private int currentHearts;
    private float lastDamageTime = -999f;
    private bool isInvincible => Time.time - lastDamageTime < invincibilityTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FindActiveSprite();
    }

    void Start()
    {
        currentHearts = maxHearts;
        UpdateHeartsUI();
    }

    private void FindActiveSprite()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer s in sprites)
        {
            if (s.gameObject.activeInHierarchy)
            {
                spriteRenderer = s;
                originalColor = s.color;
                break;
            }
        }
    }

    public void TakeDamage(int amount = 1, Vector2? damageSourcePosition = null)
    {
        if (isInvincible || currentHearts <= 0) return;

        // Re-check active sprite in case character was swapped since last hit
        if (spriteRenderer == null || !spriteRenderer.gameObject.activeInHierarchy)
        {
            FindActiveSprite();
        }

        currentHearts = Mathf.Max(currentHearts - amount, 0);
        lastDamageTime = Time.time;
        UpdateHeartsUI();

        StopAllCoroutines();
        StartCoroutine(DamageFeedbackRoutine());

        if (damageSourcePosition.HasValue && rb != null)
        {
            ApplyKnockback(damageSourcePosition.Value);
        }

        if (currentHearts <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFeedbackRoutine()
    {
        yield return StartCoroutine(FlashSprite());
        yield return StartCoroutine(ShakeSprite());
    }

    private IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    private IEnumerator ShakeSprite()
    {
        if (spriteRenderer == null) yield break;

        Transform t = spriteRenderer.transform;
        originalLocalPosition = t.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            t.localPosition = originalLocalPosition + new Vector3(offsetX, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        t.localPosition = originalLocalPosition;
    }

    private void ApplyKnockback(Vector2 sourcePosition)
    {
        Vector2 direction = ((Vector2)transform.position - sourcePosition).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    public void Heal(int amount = 1)
    {
        currentHearts = Mathf.Min(currentHearts + amount, maxHearts);
        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < currentHearts) ? fullHeartSprite : emptyHeartSprite;
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }

    public int GetCurrentHearts() => currentHearts;
}