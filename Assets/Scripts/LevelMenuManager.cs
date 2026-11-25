using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Button level4Button;
    [SerializeField] private Button level5Button;
    [SerializeField] private Button level6Button;
    [SerializeField] private Button level7Button;

    [Header("Button Images")]
    [SerializeField] private Sprite greenButton;
    [SerializeField] private Sprite greyButton;

    [Header("Star Sprites")]
    [Tooltip("Sprite used for a star the player has earned.")]
    [SerializeField] private Sprite starFilled;
    [Tooltip("Sprite used for a star the player has not yet earned.")]
    [SerializeField] private Sprite starEmpty;

    [Header("Level 1 Stars")]
    [SerializeField] private Image level1Star1;
    [SerializeField] private Image level1Star2;
    [SerializeField] private Image level1Star3;

    [Header("Level 2 Stars")]
    [SerializeField] private Image level2Star1;
    [SerializeField] private Image level2Star2;
    [SerializeField] private Image level2Star3;

    [Header("Level 3 Stars")]
    [SerializeField] private Image level3Star1;
    [SerializeField] private Image level3Star2;
    [SerializeField] private Image level3Star3;

    [Header("Level 4 Stars")]
    [SerializeField] private Image level4Star1;
    [SerializeField] private Image level4Star2;
    [SerializeField] private Image level4Star3;

    [Header("Level 5 Stars")]
    [SerializeField] private Image level5Star1;
    [SerializeField] private Image level5Star2;
    [SerializeField] private Image level5Star3;

    [Header("Level 6 Stars")]
    [SerializeField] private Image level6Star1;
    [SerializeField] private Image level6Star2;
    [SerializeField] private Image level6Star3;

    [Header("Level 7 Stars")]
    [SerializeField] private Image level7Star1;
    [SerializeField] private Image level7Star2;
    [SerializeField] private Image level7Star3;

    private void OnEnable()
    {
        // Back at the menu = next level starts with full hearts
        HealthCarry.Reset();

        // Restore default BGM when returning to the level menu
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDefaultBGM();

        // Level 1 is always unlocked
        SetLevelState(level1Button, true);

        // Each subsequent level unlocks once the previous one's FinishPoint
        // has set its "LevelXCompleted" PlayerPrefs flag.
        SetLevelState(level2Button, PlayerPrefs.GetInt("Level1Completed", 0) == 1);
        SetLevelState(level3Button, PlayerPrefs.GetInt("Level2Completed", 0) == 1);
        SetLevelState(level4Button, PlayerPrefs.GetInt("Level3Completed", 0) == 1);
        SetLevelState(level5Button, PlayerPrefs.GetInt("Level4Completed", 0) == 1);
        SetLevelState(level6Button, PlayerPrefs.GetInt("Level5Completed", 0) == 1);
        SetLevelState(level7Button, PlayerPrefs.GetInt("Level6Completed", 0) == 1);

        // Show earned stars for each level
        RefreshAllStars();
    }

    // ── Star display ──────────────────────────────────────────────────────────

    /// <summary>Reads saved star counts and updates every star Image.</summary>
    private void RefreshAllStars()
    {
        SetLevelStars(1, level1Star1, level1Star2, level1Star3);
        SetLevelStars(2, level2Star1, level2Star2, level2Star3);
        SetLevelStars(3, level3Star1, level3Star2, level3Star3);
        SetLevelStars(4, level4Star1, level4Star2, level4Star3);
        SetLevelStars(5, level5Star1, level5Star2, level5Star3);
        SetLevelStars(6, level6Star1, level6Star2, level6Star3);
        SetLevelStars(7, level7Star1, level7Star2, level7Star3);
    }

    /// <summary>
    /// Fills the three star Images for <paramref name="levelIndex"/> based on
    /// how many stars the player has earned (read from StarManager).
    /// </summary>
    private void SetLevelStars(int levelIndex, Image star1, Image star2, Image star3)
    {
        // Guard – nothing to do if sprites or images are missing
        if (starFilled == null || starEmpty == null) return;

        int earned = StarManager.GetLevelStars(levelIndex);

        SetStar(star1, earned >= 1);
        SetStar(star2, earned >= 2);
        SetStar(star3, earned >= 3);
    }

    /// <summary>Assigns the filled or empty sprite to a single star Image.</summary>
    private void SetStar(Image starImage, bool filled)
    {
        if (starImage == null) return;
        starImage.sprite = filled ? starFilled : starEmpty;
    }

    // ── Button state ──────────────────────────────────────────────────────────

    private void SetLevelState(Button button, bool unlocked)
    {
        if (button == null)
            return;

        button.interactable = unlocked;

        Image image = button.GetComponent<Image>();

        if (image != null)
        {
            image.sprite = unlocked ? greenButton : greyButton;
        }
    }

    // ── Scene loading ─────────────────────────────────────────────────────────

    private void LoadLevel(string sceneName)
    {
        HealthCarry.Reset();

        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
            SceneManager.LoadScene(sceneName);
        }
    }

    public void OpenLevel1() { LoadLevel("Fire"); }
    public void OpenLevel2() { LoadLevel("Equake"); }
    public void OpenLevel3() { LoadLevel("Tp"); }
    public void OpenLevel4() { LoadLevel("Fl"); }
    public void OpenLevel5() { LoadLevel("LS"); }
    public void OpenLevel6() { LoadLevel("VOL"); }
    public void OpenLevel7() { LoadLevel("Main"); }
    public void OpenMiniGame() { LoadLevel("MiniGamee"); }
}