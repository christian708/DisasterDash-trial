using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowFinishPoint : MonoBehaviour
{
    [SerializeField] private string targetScene = "Level1_Fire";

    [Header("Star Rating")]
    [Tooltip("The level number used by StarManager (matches the LevelMenu button number).")]
    [SerializeField] private int levelNumber = 1;
    [Tooltip("PlayerPrefs key that marks this level as completed.")]
    [SerializeField] private string levelCompletedKey = "";
    [Tooltip("Max hearts the player can have. Must match PlayerHealth.maxHearts.")]
    [SerializeField] private int maxHearts = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance?.PlaySFX("finish");

            if (!string.IsNullOrEmpty(levelCompletedKey))
            {
                PlayerPrefs.SetInt(levelCompletedKey, 1);
                PlayerPrefs.Save();
            }

            AwardStars();

            SceneController.instance.LoadScene(targetScene);
        }
    }

    private void AwardStars()
    {
        if (levelNumber <= 0) return;

        int heartsLeft = (HealthCarry.savedHealth < 0) ? maxHearts : HealthCarry.savedHealth;
        heartsLeft = Mathf.Clamp(heartsLeft, 0, maxHearts);

        int starsEarned;
        if (heartsLeft >= maxHearts)        starsEarned = 3;
        else if (heartsLeft == maxHearts - 1) starsEarned = 2;
        else if (heartsLeft > 0)            starsEarned = 1;
        else                                starsEarned = 0;

        for (int i = 1; i <= StarManager.StarsPerLevel; i++)
        {
            string key = "Level" + levelNumber + "_Star" + i;
            if (i <= starsEarned) PlayerPrefs.SetInt(key, 1);
            else                  PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }
}