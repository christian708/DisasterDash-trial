using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private Vector2 direction = new Vector2(-1f, -1f); // down-left diagonal, like your screenshot

    [Header("Travel Distance")]
    [Tooltip("How far the star travels (in world units) before looping back to its start position.")]
    [SerializeField] private float travelDistance = 10f;

    [Tooltip("Randomizes the respawn height a bit each loop, relative to the original placed position. Set to 0 for no variation.")]
    [SerializeField] private float respawnHeightVariance = 1f;

    private Vector3 startPosition; // wherever you drag the sprite to in the Scene view
    private Vector3 dirNormalized;

    private void Awake()
    {
        startPosition = transform.position; // captured from wherever it's placed in the editor
        dirNormalized = direction.normalized;
    }

    private void Update()
    {
        transform.position += dirNormalized * speed * Time.deltaTime;

        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= travelDistance)
        {
            RespawnAtStart();
        }
    }

    private void RespawnAtStart()
    {
        float yOffset = Random.Range(-respawnHeightVariance, respawnHeightVariance);
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }
}