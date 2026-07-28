using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneContoller : MonoBehaviour
{
    public static SceneContoller instance;

    [SerializeField] private Animator transitionAnim;
    [SerializeField] private float transitionTime = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 🔥 Re-find transition animator every scene
        GameObject transitionObj = GameObject.Find("Scene Transition");

        if (transitionObj != null)
        {
            transitionAnim = transitionObj.GetComponent<Animator>();
            transitionAnim.SetTrigger("Start");
        }
        else
        {
            Debug.LogWarning("TransitionPanel not found in scene!");
        }
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadLevelCoroutine(nextIndex));
        }
        else
        {
            Debug.Log("No more scenes to load!");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void GoToHome()
    {
        StartCoroutine(LoadSceneCoroutine("LevelMenu"));
    }

    private IEnumerator LoadLevelCoroutine(int sceneIndex)
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("End");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("End");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}