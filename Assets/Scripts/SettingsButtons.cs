using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButtons : MonoBehaviour
{
    [SerializeField] string mainMenuScene = "MainMenu";
    [SerializeField] string fallbackScene = "MainMenu"; // used if no previous scene is known

    static string lastScene = "";
    static string currentScene = "";

    // Auto-tracks the scene you came from, so no other scripts need editing
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != currentScene)
        {
            lastScene = currentScene;
            currentScene = scene.name;
        }
    }

    public void GoBack()
    {
        Load(string.IsNullOrEmpty(lastScene) ? fallbackScene : lastScene);
    }

    public void GoMainMenu()
    {
        Load(mainMenuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Load(string sceneName)
    {
        if (SceneController.instance != null)
            SceneController.instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName); // only when testing Settings in isolation
    }
}