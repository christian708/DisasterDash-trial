using UnityEngine;
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

    private void Start()
    {
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
    }

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

    public void OpenLevel1()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level0_Fire_Interior");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel2()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level2_Earthquake_Interior");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel3()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level5_Typhoon");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel4()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level6_Flood_In");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel5()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level8_LandslideIn");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel6()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level10_VolIn");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }

    public void OpenLevel7()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Level12_Mixed");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }
}