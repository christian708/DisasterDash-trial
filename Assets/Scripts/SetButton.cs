using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSettings : MonoBehaviour
{
    [SerializeField] string settingsScene = "Settings";

    public void Open()
    {
        if (SceneController.instance != null)
            SceneController.instance.LoadScene(settingsScene);
        else
            SceneManager.LoadScene(settingsScene);
    }
}