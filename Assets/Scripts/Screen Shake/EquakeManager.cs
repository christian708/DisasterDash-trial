using System.Collections;
using UnityEngine;

/// <summary>
/// Alternates between a Screen Shake (earthquake) phase and a Falling Rocks phase,
/// looping until the level finishes or StopHazards() is called.
///
/// - During Screen Shake: if the player moves, they take damage. Standing still is safe.
/// - During Falling Rocks: the player can walk freely, but getting hit by a rock deals damage.
/// </summary>
public class EquakeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject rockPrefab;

    [Header("Early Warning")]
    [SerializeField] private Warningpopup warningPopup;
    [SerializeField] private float warningDuration = 2f;

    [Header("Screen Shake Phase")]
    [SerializeField] private float shakeDuration = 4f;
    [SerializeField] private float shakeTrauma = 0.7f;
    [SerializeField] private int shakeDamageAmount = 1;

    [Header("Falling Rocks Phase")]
    [SerializeField] private float rocksDuration = 5f;
    [SerializeField] private float rockSpawnInterval = 0.6f;
    [Tooltip("Min/Max X offset from this object's position where rocks can spawn.")]
    [SerializeField] private Vector2 spawnAreaWidth = new Vector2(-8f, 8f);
    [Tooltip("Y offset above this object's position where rocks spawn.")]
    [SerializeField] private float spawnHeight = 6f;

    private bool isRunning = true;
    private Coroutine loopRoutine;

    private void OnEnable()
    {
        isRunning = true;
        loopRoutine = StartCoroutine(HazardLoop());
    }

    private void OnDisable()
    {
        StopHazards();
    }

    private IEnumerator HazardLoop()
    {
        while (isRunning)
        {
            if (warningPopup != null)
                yield return StartCoroutine(warningPopup.Show(warningDuration));

            if (!isRunning) yield break;

            yield return StartCoroutine(ScreenShakePhase());
            if (!isRunning) yield break;

            yield return StartCoroutine(FallingRocksPhase());
        }
    }

    private IEnumerator ScreenShakePhase()
    {
        if (CameraShake.instance != null)
            CameraShake.instance.AddTrauma(shakeTrauma);

        float elapsed = 0f;
        while (elapsed < shakeDuration && isRunning)
        {
            // Keep the shake sustained across the whole phase
            if (CameraShake.instance != null && !CameraShake.instance.IsShaking)
                CameraShake.instance.AddTrauma(shakeTrauma);

            if (playerController != null && playerController.IsMoving && playerHealth != null)
            {
                // PlayerHealth already has its own invincibility window, so calling this
                // every frame while moving naturally throttles to one hit per window.
                playerHealth.TakeDamage(shakeDamageAmount);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (CameraShake.instance != null)
            CameraShake.instance.StopShake();
    }

    private IEnumerator FallingRocksPhase()
    {
        float elapsed = 0f;

        while (elapsed < rocksDuration && isRunning)
        {
            SpawnRock();
            yield return new WaitForSeconds(rockSpawnInterval);
            elapsed += rockSpawnInterval;
        }
    }

    private void SpawnRock()
    {
        if (rockPrefab == null) return;

        float x = transform.position.x + Random.Range(spawnAreaWidth.x, spawnAreaWidth.y);
        float y = transform.position.y + spawnHeight;
        Instantiate(rockPrefab, new Vector3(x, y, 0f), Quaternion.identity);
    }

    /// <summary>Call this when the level finishes to stop the earthquake loop cleanly.</summary>
    public void StopHazards()
    {
        isRunning = false;
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        if (CameraShake.instance != null) CameraShake.instance.StopShake();
    }
}