using UnityEngine;

public class SmokeDrift : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 0.15f;        // lower = slower
    [SerializeField] private float swayAmount = 0.05f;   // up/down wobble
    [SerializeField] private float swaySpeed = 0.5f;

    [Header("Looping (optional)")]
    [SerializeField] private bool loop = true;
    [SerializeField] private float leftLimit = -10f;     // when smoke reaches this X...
    [SerializeField] private float rightStart = 10f;     // ...it reappears here

    [Header("Fade pulse (optional)")]
    [SerializeField] private bool pulseAlpha = true;
    [SerializeField] private float minAlpha = 0.6f;
    [SerializeField] private float maxAlpha = 0.9f;
    [SerializeField] private float pulseSpeed = 0.4f;

    private SpriteRenderer sr;
    private float startY;
    private float offset;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startY = transform.position.y;
        offset = Random.Range(0f, 100f); // so multiple smokes don't move in sync
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        // Move left
        pos.x -= speed * Time.deltaTime;

        // Gentle sway
        pos.y = startY + Mathf.Sin((Time.time + offset) * swaySpeed) * swayAmount;

        // Loop back to the right side
        if (loop && pos.x < leftLimit)
            pos.x = rightStart;

        transform.position = pos;

        // Soft fade in/out
        if (pulseAlpha && sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha,
                (Mathf.Sin((Time.time + offset) * pulseSpeed) + 1f) * 0.5f);
            sr.color = c;
        }
    }
}