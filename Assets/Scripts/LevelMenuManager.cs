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

        // Level 2 is unlocked when Level 1 is completed
        bool level1Completed = PlayerPrefs.GetInt("Level1Completed", 0) == 1;
        SetLevelState(level2Button, level1Completed);

        // Keep Levels 3-7 locked for now
        SetLevelState(level3Button, false);
        SetLevelState(level4Button, false);
        SetLevelState(level5Button, false);
        SetLevelState(level6Button, false);
        SetLevelState(level7Button, false);
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
}