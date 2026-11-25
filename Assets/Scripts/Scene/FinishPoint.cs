using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [Header("Which story scene plays after this level")]
    [Tooltip("e.g. Story1, Story2, Story3... set this per-scene in the Inspector.")]
    [SerializeField] private string storySceneName = "Story1";

    [Header("Which PlayerPrefs key marks this level as completed")]
    [Tooltip("e.g. Level1Completed, Level2Completed... set this per-scene in the Inspector.")]
    [SerializeField] private string levelCompletedKey = "Level1Completed";

    [Header("Star Rating")]
    [Tooltip("The level number used by StarManager (1 = Fire, 2 = Equake, etc.)")]
    [SerializeField] private int levelNumber = 1;
    [Tooltip("Max hearts the player can have. Must match PlayerHealth.maxHearts.")]
    [SerializeField] private int maxHearts = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Something entered the trigger: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player reached finish point!");

            AudioManager.Instance?.PlaySFX("finish");

            // Mark this level as completed
            PlayerPrefs.SetInt(levelCompletedKey, 1);
            PlayerPrefs.Save();

            // Calculate and save star rating based on remaining health
            AwardStars();

            Debug.Log($"{levelCompletedKey} set! Loading {storySceneName}...");

            // Go to the story scene instead of jumping straight to the next level
            SceneController.instance.LoadScene(storySceneName);
        }
    }

    /// <summary>
    /// Awards 1–3 stars based on how much health the player has left.
    /// 3 hearts = 3 stars, 2 hearts = 2 stars, 1 heart = 1 star.
    /// Saves the exact result — this run always overwrites the previous rating.
    /// </summary>
    private void AwardStars()
    {
        // HealthCarry.savedHealth is -1 when full health was never touched,
        // so treat -1 as full hearts.
        int heartsLeft = (HealthCarry.savedHealth < 0) ? maxHearts : HealthCarry.savedHealth;

        // Clamp to valid range just in case
        heartsLeft = Mathf.Clamp(heartsLeft, 0, maxHearts);

        // Map health to star count
        // Full health        → 3 stars
        // One heart lost     → 2 stars
        // Two+ hearts lost   → 1 star
        // Dead (0 hearts)    → 0 stars
        int starsEarned;
        if (heartsLeft >= maxHearts)
            starsEarned = 3;
        else if (heartsLeft == maxHearts - 1)
            starsEarned = 2;
        else if (heartsLeft > 0)
            starsEarned = 1;
        else
            starsEarned = 0;

        Debug.Log($"Level {levelNumber} finished — hearts: {heartsLeft}/{maxHearts} → {starsEarned} star(s)");

        // Overwrite all 3 star slots to exactly match this run's result.
        // Star i is filled if i <= starsEarned, cleared otherwise.
        for (int i = 1; i <= StarManager.StarsPerLevel; i++)
        {
            string key = "Level" + levelNumber + "_Star" + i;
            if (i <= starsEarned)
                PlayerPrefs.SetInt(key, 1);
            else
                PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }
}