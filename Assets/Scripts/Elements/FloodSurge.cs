using UnityEngine;
using System.Collections;

/// <summary>
/// One flood surge: rises into view at knee height, stays active (slowing
/// the player if they're standing in it), then recedes and cleans itself up.
/// Meant to be spawned by FloodSpawner at random positions along the level.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FloodSurge : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float riseDuration = 0.5f;
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float recedeDuration = 0.5f;

    [Header("Visual")]
    [Tooltip("Child object with the water surface sprite - scales up/down for rise and recede.")]
    [SerializeField] private Transform waterVisual;

    [Header("Effect")]
    [SerializeField] private float playerSpeedMultiplier = 0.5f; // 5 -> 2.5

    private Collider2D col;
    private PlayerController playerInZone;
    private Vector3 fullScale;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;

        if (waterVisual != null)
        {
            fullScale = waterVisual.localScale;
            waterVisual.localScale = new Vector3(fullScale.x, 0f, fullScale.z);
        }
    }

    private void Start()
    {
        StartCoroutine(SurgeSequence());
    }

    private IEnumerator SurgeSequence()
    {
        // Rise
        float t = 0f;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / riseDuration);
            if (waterVisual != null) waterVisual.localScale = new Vector3(fullScale.x, fullScale.y * p, fullScale.z);
            yield return null;
        }
        if (waterVisual != null) waterVisual.localScale = fullScale;
        col.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        // Recede
        col.enabled = false;
        t = 0f;
        while (t < recedeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / recedeDuration);
            if (waterVisual != null) waterVisual.localScale = new Vector3(fullScale.x, fullScale.y * (1f - p), fullScale.z);
            yield return null;
        }

        // Safety: make sure the player isn't left permanently slowed if the
        // surge recedes while they're still standing in it.
        if (playerInZone != null)
        {
            playerInZone.SetSpeedMultiplier(1f);
            playerInZone = null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                playerInZone = pc;
                pc.SetSpeedMultiplier(playerSpeedMultiplier);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.SetSpeedMultiplier(1f);
            if (playerInZone == pc) playerInZone = null;
        }
    }
}