using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquakeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;

    [Tooltip("Debris to drop (rock, branch, etc). Prefabs from the Project window are best, but scene objects also work - they get cloned and the original is switched off.")]
    [SerializeField] private GameObject[] debrisPrefabs;

    [Header("Early Warning")]
    [SerializeField] private Warningpopup warningPopup;
    [SerializeField] private float warningDuration = 2f;

    [Header("Screen Shake Phase")]
    [SerializeField] private float shakeDuration = 4f;
    [SerializeField] private float shakeTrauma = 0.7f;
    [SerializeField] private int shakeDamageAmount = 1;
    [Tooltip("Seconds between damage ticks while the player is moving during the shake.")]
    [SerializeField] private float shakeDamageInterval = 0.5f;
    [Tooltip("Player speed multiplier during the shake when wearing the shoes (0.5 = half speed).")]
    [SerializeField] private float shoeSlowMultiplier = 0.5f;

    [Header("Falling Debris Phase")]
    [Tooltip("How long the debris phase lasts, in seconds.")]
    [SerializeField] private float rocksDuration = 15f;
    [Tooltip("How many of EACH debris type fall at the same time. 1 = one rock + one branch.")]
    [SerializeField] private int slotsPerPrefab = 1;
    [Tooltip("Seconds to wait after a debris is gone (fell off camera / destroyed) before the next one spawns above the player.")]
    [SerializeField] private float respawnDelay = 3f;
    [Tooltip("Extra random delay before each slot's FIRST spawn, so they don't all drop together.")]
    [SerializeField] private float maxStartStagger = 1f;
    [Tooltip("Max random X offset from the player's CURRENT position. 0 = directly above the player.")]
    [SerializeField] private float spawnXJitter = 0f;
    [Tooltip("Y offset above the player's current position where debris spawns.")]
    [SerializeField] private float spawnHeight = 6f;
    [Tooltip("How far below the bottom of the camera a debris must be to count as 'fell off screen'.")]
    [SerializeField] private float offScreenMargin = 2f;
    [Tooltip("Safety cleanup: a debris that is still alive after this many seconds counts as gone.")]
    [SerializeField] private float debrisLifetime = 10f;

    [Header("Timing")]
    [SerializeField] private float cooldownBetweenCycles = 3f;

    private bool isRunning = true;
    private bool debrisPhaseActive = false;
    private Coroutine loopRoutine;

    // Safe, inactive copies of the debris that we clone from.
    // These can never be destroyed by the debris scripts themselves.
    private GameObject[] templates = new GameObject[0];

    private void Awake()
    {
        BuildTemplates();
    }

    private void BuildTemplates()
    {
        List<GameObject> list = new List<GameObject>();

        if (debrisPrefabs != null)
        {
            foreach (GameObject p in debrisPrefabs)
            {
                if (p == null) continue;

                if (p.scene.IsValid())
                {
                    // It's an object in the scene (not a prefab asset). If we used it directly,
                    // its own script (Branch / RockHazard) would destroy it and the reference
                    // would become Missing. So: switch the original off and keep an inactive clone.
                    p.SetActive(false);
                    GameObject template = Instantiate(p, transform);
                    template.name = p.name + "_Template";
                    list.Add(template);
                }
                else
                {
                    list.Add(p);
                }
            }
        }

        templates = list.ToArray();

        if (templates.Length == 0)
            Debug.LogWarning("[Equake] No debris assigned in 'Debris Prefabs'. Nothing will fall.");
    }

    private void OnEnable()
    {
        isRunning = true;
        if (loopRoutine != null) StopCoroutine(loopRoutine);
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

            yield return StartCoroutine(FallingDebrisPhase());
            if (!isRunning) yield break;

            yield return new WaitForSeconds(cooldownBetweenCycles);
        }
    }

    private IEnumerator ScreenShakePhase()
    {
        if (CameraShake.instance != null)
            CameraShake.instance.AddTrauma(shakeTrauma);

#if UNITY_EDITOR
        Debug.Log($"[Equake] Shake started. Equipped id = '{(InventoryManager.Instance != null ? InventoryManager.Instance.EquippedItemId : "NO INVENTORY MANAGER")}', HasShoes = {EarthquakeState.HasShoes}");
#endif

        float elapsed = 0f;
        float damageTimer = 0f;

        while (elapsed < shakeDuration && isRunning)
        {
            // Keep the shake sustained across the whole phase
            if (CameraShake.instance != null && !CameraShake.instance.IsShaking)
                CameraShake.instance.AddTrauma(shakeTrauma);

            if (playerController != null)
            {
                if (EarthquakeState.HasShoes)
                {
                    // Shoes: no damage, just slowed
                    playerController.SetSpeedMultiplier(shoeSlowMultiplier);
                }
                else
                {
                    playerController.SetSpeedMultiplier(1f);

                    damageTimer += Time.deltaTime;
                    if (damageTimer >= shakeDamageInterval)
                    {
                        damageTimer = 0f;
                        if (playerController.IsMoving && playerHealth != null)
                            playerHealth.TakeDamage(shakeDamageAmount);
                    }
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (playerController != null)
            playerController.SetSpeedMultiplier(1f);

        if (CameraShake.instance != null)
            CameraShake.instance.StopShake();
    }

    private IEnumerator FallingDebrisPhase()
    {
        debrisPhaseActive = true;

        // One respawning "slot" per debris type (x slotsPerPrefab)
        foreach (GameObject template in templates)
        {
            if (template == null) continue;
            for (int i = 0; i < Mathf.Max(1, slotsPerPrefab); i++)
                StartCoroutine(DebrisSlot(template));
        }

        float elapsed = 0f;
        while (elapsed < rocksDuration && isRunning)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Slots stop respawning; debris already in the air just finishes falling
        debrisPhaseActive = false;
    }

    // One slot = one debris that keeps coming back:
    // spawn above player -> wait until it's gone -> wait respawnDelay -> repeat
    private IEnumerator DebrisSlot(GameObject template)
    {
        if (maxStartStagger > 0f)
            yield return new WaitForSeconds(Random.Range(0f, maxStartStagger));

        while (isRunning && debrisPhaseActive)
        {
            GameObject debris = SpawnDebris(template);

            // Wait until it is destroyed, falls off the bottom of the camera, or times out
            float life = 0f;
            while (isRunning && debris != null && !IsBelowCamera(debris.transform.position) && life < debrisLifetime)
            {
                life += Time.deltaTime;
                yield return null;
            }

            if (debris != null) Destroy(debris);

            yield return new WaitForSeconds(respawnDelay);
        }
    }

    private GameObject SpawnDebris(GameObject template)
    {
        if (template == null || playerController == null) return null;

        // Read the player's position at the moment of spawning so debris
        // always drops where the player currently is.
        Vector3 playerPos = playerController.transform.position;
        float x = playerPos.x + Random.Range(-spawnXJitter, spawnXJitter);
        float y = playerPos.y + spawnHeight;

        GameObject debris = Instantiate(template, new Vector3(x, y, 0f), Quaternion.identity);
        debris.SetActive(true); // templates made from scene objects are inactive
        return debris;
    }

    private bool IsBelowCamera(Vector3 pos)
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        float bottom = cam.transform.position.y - cam.orthographicSize - offScreenMargin;
        return pos.y < bottom;
    }

    public void StopHazards()
    {
        isRunning = false;
        debrisPhaseActive = false;
        if (loopRoutine != null) StopCoroutine(loopRoutine);
        if (CameraShake.instance != null) CameraShake.instance.StopShake();
        if (playerController != null) playerController.SetSpeedMultiplier(1f);
    }
}