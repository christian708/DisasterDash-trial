using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something entered the trigger: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player reached finish point!");

            // Save that Level 1 has been completed
            PlayerPrefs.SetInt("Level1Completed", 1);
            PlayerPrefs.Save();

            Debug.Log("Level 1 completed! Level 2 unlocked.");

            // Continue to the next scene
            SceneController.instance.LoadNextLevel();
        }
    }
}