using UnityEngine;

/// <summary>
/// Attach to the Bag overlay panel's button row (Home/Settings/Back/bag icon)
/// inside EACH gameplay scene now that Inventory is an overlay, not a scene.
/// Uses the existing SceneController singleton for Home/Settings, which still
/// leave the current scene. Back/bag icon just toggle this panel's visibility.
/// </summary>
public class InvSceneNav : MonoBehaviour
{
    [Tooltip("The whole Bag overlay panel (grid + description). Toggled on/off, never loaded as a scene.")]
    public GameObject bagPanel;

    [Tooltip("Scene name for the Settings button.")]
    public string settingsSceneName = "Settings";

    public void OpenBag()
    {
        if (bagPanel != null) bagPanel.SetActive(true);
    }

    public void CloseBag()
    {
        if (bagPanel != null) bagPanel.SetActive(false);
    }

    public void GoHome()
    {
        if (SceneController.instance == null)
        {
            Debug.LogWarning("SceneController instance not found!");
            return;
        }

        SceneController.instance.GoToHome();
    }

    public void OpenSettings()
    {
        if (SceneController.instance == null)
        {
            Debug.LogWarning("SceneController instance not found!");
            return;
        }

        SceneController.instance.LoadScene(settingsSceneName);
    }
}