using System.Collections;
using UnityEngine;

public class BoulderHazard : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private string groundTag = "ground";
    [SerializeField] private float crumbleDelay = 3f;
    [SerializeField] private float fadeTime = 0.5f;
    [Tooltip("Safety: if the boulder hasn't landed this many seconds after dropping, it fades away so it can respawn.")]
    [SerializeField] private float fallTimeout = 8f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D boulderCollider;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Color startColor;
    private string startTag;

    private bool dropped = false;
    private bool landed = false;
    private bool hitPlayer = false;

    /// <summary>True while the boulder is falling, resting or crumbling.</summary>
    public bool IsDropping => dropped;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        boulderCollider = GetComponent<Collider2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;
        startColor = sr != null ? sr.color : Color.white;
        startTag = gameObject.tag;

        // Before Drop(): can't be pushed or moved by anything.
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    /// Puts the boulder back at its start position, fully visible, ready to drop.
    /// </summary>
    public void ResetBoulder()
    {
        StopAllCoroutines();

        dropped = false;
        landed = false;
        hitPlayer = false;

        gameObject.SetActive(true);
        gameObject.tag = startTag;

        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.SetPositionAndRotation(startPosition, startRotation);

        if (sr != null) sr.color = startColor;
        if (boulderCollider != null) boulderCollider.enabled = true;
    }

    public void Drop()
    {
        dropped = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        AudioManager.Instance?.PlaySFX("falling_boulder");
        StartCoroutine(FallTimeout());
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Not dropped yet, or already landed: no damage, no pushing.
        if (!dropped || landed) return;

        if (col.gameObject.CompareTag("Player"))
        {
            // Only the falling boulder can hurt, and only once.
            if (!hitPlayer)
            {
                hitPlayer = true;

                PlayerHealth health = col.gameObject.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage, transform.position);
                }
            }
            return;
        }

        // Landed on the ground tag, or on anything below the boulder's center.
        bool onGroundTag = col.gameObject.CompareTag(groundTag);
        bool surfaceBelow = col.contactCount > 0 && col.GetContact(0).point.y < transform.position.y;

        if (onGroundTag || surfaceBelow)
        {
            Land();
        }
    }

    private void Land()
    {
        landed = true;
        hitPlayer = true; // can never damage the player again

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static; // can't be pushed anymore

        // If any other script checks tags (e.g. PlayerHealth hurting the player on a
        // "Hazard" tag), it will now ignore the landed boulder.
        gameObject.tag = "Untagged";

        StartCoroutine(Crumble());
    }

    private IEnumerator FallTimeout()
    {
        yield return new WaitForSeconds(fallTimeout);

        if (!landed)
        {
            landed = true;
            hitPlayer = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            gameObject.tag = "Untagged";
            StartCoroutine(Crumble());
        }
    }

    private IEnumerator Crumble()
    {
        yield return new WaitForSeconds(crumbleDelay);

        // let the player pass while it fades
        if (boulderCollider != null) boulderCollider.enabled = false;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f - (t / fadeTime);
                sr.color = c;
            }
            yield return null;
        }

        // Hide instead of destroying, so the drop zone can respawn it.
        dropped = false;
        gameObject.SetActive(false);
    }
}