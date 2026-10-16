using System.Collections;
using UnityEngine;

public class BoulderHazard : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private string groundTag = "ground";
    [SerializeField] private float crumbleDelay = 3f;
    [SerializeField] private float fadeTime = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool landed = false;
    private bool hitPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f; // waits until Drop()
    }

    public void Drop()
    {
        rb.gravityScale = 1f;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (landed) return;

        if (col.gameObject.CompareTag("Player") && !hitPlayer)
        {
            hitPlayer = true;

            PlayerHealth health = col.gameObject.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage, transform.position);
            }
        }
        else if (col.gameObject.CompareTag(groundTag))
        {
            landed = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            StartCoroutine(Crumble());
        }
    }

    private IEnumerator Crumble()
    {
        yield return new WaitForSeconds(crumbleDelay);

        // let the player pass while it fades
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

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

        Destroy(gameObject);
    }
}