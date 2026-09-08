using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [Header("Which story scene plays after this level")]
    [Tooltip("e.g. Story1, Story2, Story3... set this per-scene in the Inspector.")]
    [SerializeField] private string storySceneName = "Story1";

    [Header("Which PlayerPrefs key marks this level as completed")]
    [Tooltip("e.g. Level1Completed, Level2Completed... set this per-scene in the Inspector.")]
    [SerializeField] private string levelCompletedKey = "Level1Completed";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something entered the trigger: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player reached finish point!");

            // Mark this level as completed
            PlayerPrefs.SetInt(levelCompletedKey, 1);
            PlayerPrefs.Save();

            Debug.Log($"{levelCompletedKey} set! Loading {storySceneName}...");

            // Go to the story scene instead of jumping straight to the next level
            SceneController.instance.LoadScene(storySceneName);
        }
    }
}