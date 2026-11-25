using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StarPickup : MonoBehaviour
{
    [Header("Star ID")]
    [Tooltip("Play button number: Level0_Fire_Interior and Level1_Fire are both Level 1")]
    [SerializeField] private int levelNumber = 1;
    [Tooltip("1 = indoor star, 2 and 3 = outdoor stars")]
    [Range(1, 3)]
    [SerializeField] private int starIndex = 1;

    [Header("Optional")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float alreadyCollectedAlpha = 0.35f;

    private SpriteRenderer sr;
    private Vector3 startPos;
    private bool alreadyCollected;
    private bool consumed;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        startPos = transform.position;
        alreadyCollected = StarManager.IsCollected(levelNumber, starIndex);

        // Collected on an earlier run: show it faded and don't give it again
        if (alreadyCollected && sr != null)
        {
            Color c = sr.color;
            c.a = alreadyCollectedAlpha;
            sr.color = c;
        }
    }

    void Update()
    {
        if (consumed) return;
        float offsetY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + new Vector3(0f, offsetY, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed || alreadyCollected) return;
        if (other.GetComponentInParent<PlayerHealth>() == null) return;

        consumed = true;
        StarManager.Collect(levelNumber, starIndex);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        StartCoroutine(PopAndDestroy());
    }

    private IEnumerator PopAndDestroy()
    {
        float duration = 0.25f;
        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            transform.localScale = startScale * (1f + p * 0.8f);

            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f - p;
                sr.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}