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

    [Header("Heal Feedback")]
    [SerializeField] private Color healGlowColor = new Color(0.3f, 1f, 0.4f, 1f);
    [SerializeField] private int healPulseCount = 3;
    [SerializeField] private float healPulseDuration = 0.12f;
    [SerializeField] private ParticleSystem healParticles; // optional

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalLocalPosition;
    private Coroutine healRoutine;

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
        currentHearts = (HealthCarry.savedHealth >= 0)
            ? Mathf.Clamp(HealthCarry.savedHealth, 1, maxHearts)
            : maxHearts;

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

        if (spriteRenderer == null || !spriteRenderer.gameObject.activeInHierarchy)
        {
            FindActiveSprite();
        }

        currentHearts = Mathf.Max(currentHearts - amount, 0);
        lastDamageTime = Time.time;
        UpdateHeartsUI();
        HealthCarry.savedHealth = currentHearts;

        // Cancel any heal glow so the sprite doesn't get stuck green
        StopAllCoroutines();
        healRoutine = null;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;

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
        // Horizontal-only push so it doesn't fight gravity.
        float directionX = Mathf.Sign(transform.position.x - sourcePosition.x);
        if (directionX == 0f) directionX = 1f;

        rb.AddForce(new Vector2(directionX, 0f) * knockbackForce, ForceMode2D.Impulse);
    }

    // Returns false (and does nothing) if already at full hearts.
    // Used by the potion so it is only consumed when it can actually heal.
    public bool TryHeal(int amount = 1)
    {
        if (currentHearts <= 0) return false;
        if (currentHearts >= maxHearts) return false;

        currentHearts = Mathf.Min(currentHearts + amount, maxHearts);
        UpdateHeartsUI();
        HealthCarry.savedHealth = currentHearts;

        PlayHealEffect();
        return true;
    }

    public void Heal(int amount = 1)
    {
        TryHeal(amount);
    }

    public bool IsFullHealth() => currentHearts >= maxHearts;

    private void PlayHealEffect()
    {
        if (spriteRenderer == null || !spriteRenderer.gameObject.activeInHierarchy)
        {
            FindActiveSprite();
        }

        if (healParticles != null)
        {
            healParticles.Play();
        }

        if (healRoutine != null) StopCoroutine(healRoutine);
        healRoutine = StartCoroutine(HealGlowRoutine());
    }

    private IEnumerator HealGlowRoutine()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < healPulseCount; i++)
        {
            spriteRenderer.color = healGlowColor;
            yield return new WaitForSeconds(healPulseDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(healPulseDuration);
        }

        spriteRenderer.color = originalColor;
        healRoutine = null;
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

        // Retry starts with full hearts
        HealthCarry.Reset();

        if (DeathController.Instance != null)
        {
            DeathController.Instance.ShowDeathScreen();
        }
    }

    public int GetCurrentHearts() => currentHearts;
}