using UnityEngine;
using System.Collections;

/// <summary>
/// Put on an empty GameObject positioned in the level. Repeatedly spawns
/// LightningHazard prefab instances at random positions within the given
/// area, on a random time interval.
/// </summary>
public class LightningSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private LightningHazard lightningPrefab;

    [Header("Spawn Area (relative to this object's position)")]
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 5f);

    [Header("Timing Between Strikes")]
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 4f;

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

            SpawnLightning();
        }
    }

    private void SpawnLightning()
    {
        if (lightningPrefab == null) return;

        Vector2 randomOffset = new Vector2(
            Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
            Random.Range(-areaSize.y / 2f, areaSize.y / 2f));

        Vector3 spawnPos = transform.position + (Vector3)randomOffset;

        Instantiate(lightningPrefab, spawnPos, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}