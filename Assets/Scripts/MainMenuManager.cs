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
        // Continue stays interactable either way - falls back to Character Select
        // if there's no save yet (see OnContinuePressed).
    }

    public void OnNewGamePressed()
    {
        // Reset JSON save data
        SaveData freshData = new SaveData();
        GameSession.LoadedData = freshData;
        SaveSystem.Save(freshData); // overwrite the file immediately so nothing lingers

        // Reset PlayerPrefs progress flags (level unlocks, completion, etc.)
        ResetProgressPlayerPrefs();

        // Reset inventory - all items back to locked/greyed-out
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetInventory();
        }

        Debug.Log("Level1Completed after reset: " + PlayerPrefs.GetInt("Level1Completed", 0));

        SceneManager.LoadScene(characterSelectScene);
    }

    public void OnContinuePressed()
    {
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