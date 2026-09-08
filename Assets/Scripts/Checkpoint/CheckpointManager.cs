using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public string CheckpointScene { get; private set; }
    public Vector3 CheckpointPosition { get; private set; }
    public bool HasCheckpoint { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckpoint(string sceneName, Vector3 position)
    {
        CheckpointScene = sceneName;
        CheckpointPosition = position;
        HasCheckpoint = true;
        Debug.Log($"[CheckpointManager] Checkpoint set: scene='{sceneName}', position={position}");
    }
}