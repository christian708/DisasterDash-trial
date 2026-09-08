using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent — works no matter which scene the player dies in. Call
/// ShowDeathScreen() from PlayerHealth.Die(). Shows the overlay, waits,
/// reloads the checkpoint's scene, and repositions the player there.
/// </summary>
public class DeathController : MonoBehaviour
{
    public static DeathController Instance { get; private set; }

    [Tooltip("The black background + 'You Died' text panel, child of this object's own Canvas. Starts hidden.")]
    public GameObject deathPanel;

    [Tooltip("Seconds to show the death screen before auto-respawning.")]
    public float respawnDelay = 2f;

    private bool isRespawning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    public void ShowDeathScreen()
    {
        if (isRespawning) return;
        isRespawning = true;

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        SceneManager.sceneLoaded += OnSceneLoaded;

        string sceneToLoad = (CheckpointManager.Instance != null && CheckpointManager.Instance.HasCheckpoint)
            ? CheckpointManager.Instance.CheckpointScene
            : SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(RepositionPlayerNextFrame());
    }

    private IEnumerator RepositionPlayerNextFrame()
    {
        // Wait a frame so other scripts' Start() (e.g. PlayerSpawn) run first,
        // so they don't override our position afterward.
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && CheckpointManager.Instance != null && CheckpointManager.Instance.HasCheckpoint)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            player.transform.position = CheckpointManager.Instance.CheckpointPosition;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        isRespawning = false;
    }
}