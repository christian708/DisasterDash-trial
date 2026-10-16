using UnityEngine;
using System.Collections;

/// <summary>
/// Put on an empty GameObject positioned at knee height in the level.
/// Repeatedly spawns FloodSurge prefab instances at random X positions
/// within the given area, on a random time interval. Keep Area Size's Y
/// at 0 to keep every surge at the same knee height, only varying X.
/// </summary>
public class FloodSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private FloodSurge floodSurgePrefab;

    [Header("Spawn Area (relative to this object's position)")]
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 0f);

    [Header("Timing Between Surges")]
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 4f;

    [Header("Size (per-scene override)")]
    [Tooltip("Scales each spawned surge independently on X and Y. (1, 1) = prefab's normal size. Use e.g. (1.5, 0.7) to stretch it wider but shorter for a different scene, all from the same shared prefab.")]
    [SerializeField] private Vector2 surgeScale = Vector2.one;

    [Tooltip("Shifts every spawned surge up/down from this spawner's own position - use a negative value to sit the water lower.")]
    [SerializeField] private float verticalOffset = 0f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            SpawnSurge();
        }
    }

    private void SpawnSurge()
    {
        if (floodSurgePrefab == null) return;

        Vector2 randomOffset = new Vector2(
            Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
            Random.Range(-areaSize.y / 2f, areaSize.y / 2f));

        Vector3 spawnPos = transform.position + (Vector3)randomOffset + new Vector3(0f, verticalOffset, 0f);

        FloodSurge instance = Instantiate(floodSurgePrefab, spawnPos, Quaternion.identity);
        instance.transform.localScale = Vector3.Scale(
            instance.transform.localScale,
            new Vector3(surgeScale.x, surgeScale.y, 1f));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}