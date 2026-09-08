using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to a trigger zone. When the player touches it, this becomes the
/// point they respawn at if they die anywhere in the game afterward.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetCheckpoint(SceneManager.GetActiveScene().name, transform.position);
        }
    }
}