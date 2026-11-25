using UnityEngine;

public class CementDebris : MonoBehaviour
{
    [SerializeField] private GameObject dustEffectPrefab; // optional

    private Rigidbody2D rb;
    private Collider2D col;
    private bool falling = false;
    private bool landed = false;
    private float landY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Static;   // frozen until Drop() is called
    }

    // Only the spawner calls this. landY = world Y of the tabletop surface.
    public void Drop(float tabletopY)
    {
        landY = tabletopY;
        falling = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void FixedUpdate()
    {
        if (!falling || landed) return;

        // Backstop: stop at the tabletop even if physics misses it
        float bottom = col.bounds.min.y;
        if (bottom <= landY)
        {
            transform.position += new Vector3(0f, landY - bottom, 0f);
            Land();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling && !landed) Land();
    }

    private void Land()
    {
        landed = true;

        if (dustEffectPrefab != null)
            Instantiate(dustEffectPrefab, transform.position, Quaternion.identity);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;   // stays there forever
    }
}