using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns "wind" hazard objects that travel from right to left across the screen,
/// matching the same Inspector layout style as LightningSpawner (Prefab + Spawn Area).
///
/// Spawn timing is fixed (not random like Lightning's Min/Max Interval):
/// first spawn at 2s, then alternates 5s -> 3s -> 5s -> 3s ... forever.
/// </summary>
public class WindSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject windPrefab;

    [Header("Spawn Area (relative to this object's position)")]
    [Tooltip("X = fixed spawn distance to the right of this spawner. Y = vertical range winds can spawn within.")]
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 1f);

    [Header("Timing Pattern")]
    [SerializeField] private float firstSpawnDelay = 2f;
    [SerializeField] private float intervalA = 5f;
    [SerializeField] private float intervalB = 3f;

    [Header("Wind Behaviour")]
    [SerializeField] private float windTravelSpeed = 6f;
    [Tooltip("How far past this spawner's position (to the left) before the wind destroys itself.")]
    [SerializeField] private float despawnDistanceLeft = 20f;

    [Header("Player Speed Effect")]
    [SerializeField] private float normalPlayerSpeed = 5f;
    [SerializeField] private float slowedPlayerSpeed = 2.5f;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnLoop()
    {
        // First spawn happens after the initial delay
        yield return new WaitForSeconds(firstSpawnDelay);
        SpawnWind();

        // Then loop: 5s, 3s, 5s, 3s, ...
        bool useIntervalA = true;
        while (true)
        {
            float wait = useIntervalA ? intervalA : intervalB;
            yield return new WaitForSeconds(wait);
            SpawnWind();
            useIntervalA = !useIntervalA;
        }
    }

    private void SpawnWind()
    {
        if (windPrefab == null)
        {
            Debug.LogWarning("WindSpawner: no Wind Prefab assigned.");
            return;
        }

        float spawnX = transform.position.x + areaSize.x;
        float spawnY = transform.position.y + Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, transform.position.z);

        GameObject windObj = Instantiate(windPrefab, spawnPos, Quaternion.identity);

        if (windObj.TryGetComponent(out Wind wind))
        {
            wind.Init(
                travelSpeed: windTravelSpeed,
                normalPlayerSpeed: normalPlayerSpeed,
                slowedPlayerSpeed: slowedPlayerSpeed,
                despawnX: transform.position.x - despawnDistanceLeft
            );
        }
    }

    // Visualize the spawn area in the editor, same idea as Lightning's Area Size gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 spawnPoint = transform.position + new Vector3(areaSize.x, 0f, 0f);
        Gizmos.DrawWireCube(spawnPoint, new Vector3(0.2f, areaSize.y, 0.2f));
    }
}