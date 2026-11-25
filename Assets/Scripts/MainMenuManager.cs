using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;

    [Header("Scene Names (must match Build Settings exactly)")]
    [SerializeField] private string characterSelectScene = "Charac_Select";
    [SerializeField] private string levelMenuScene = "LevelMenu";

    private void Start()
    {
        RefreshContinueButton();
    }

    /// <summary>
    /// Continue is available if there is a save file OR any level has been completed.
    /// Without either, the player has no progress to continue from.
    /// </summary>
    private void RefreshContinueButton()
    {
        if (continueButton == null) return;

        bool hasSave = SaveSystem.SaveExists();

        bool hasProgress = false;
        for (int i = 1; i <= 7; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i + "Completed", 0) == 1)
            {
                hasProgress = true;
                break;
            }
        }

        continueButton.interactable = hasSave || hasProgress;
    }

    public void OnNewGamePressed()
    {
        // Reset JSON save data
        SaveData freshData = new SaveData();
        GameSession.LoadedData = freshData;
        SaveSystem.Save(freshData); // overwrite the file immediately so nothing lingers

        // Reset PlayerPrefs progress flags (level unlocks, completion, etc.)
        ResetProgressPlayerPrefs();

        // Reset collected stars
        StarManager.ResetAll();

        // Reset carried-over health
        HealthCarry.Reset();

        // Reset inventory - all items back to locked/greyed-out
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetInventory();
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] InventoryManager.Instance is NULL - inventory not reset!");
        }

        SceneManager.LoadScene(characterSelectScene);
    }

    public void OnContinuePressed()
    {
        HealthCarry.Reset();

        if (SaveSystem.SaveExists())
        {
            SaveData data = SaveSystem.Load();
            GameSession.LoadedData = data;
            SceneManager.LoadScene(levelMenuScene);
        }
        else
        {
            SaveData freshData = new SaveData();
            GameSession.LoadedData = freshData;
            SceneManager.LoadScene(characterSelectScene);
        }
    }

    private void ResetProgressPlayerPrefs()
    {
        // Add every progress-related PlayerPrefs key here.
        // These are the ones currently used by LevelMenuManager:
        PlayerPrefs.DeleteKey("Level1Completed");
        PlayerPrefs.DeleteKey("Level2Completed");
        PlayerPrefs.DeleteKey("Level3Completed");
        PlayerPrefs.DeleteKey("Level4Completed");
        PlayerPrefs.DeleteKey("Level5Completed");
        PlayerPrefs.DeleteKey("Level6Completed");
        PlayerPrefs.DeleteKey("Level7Completed");
        PlayerPrefs.Save();
    }
}