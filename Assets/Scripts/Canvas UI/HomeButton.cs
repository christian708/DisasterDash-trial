using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void GoHome()
    {
        Time.timeScale = 1f;

        if (SceneController.instance != null)
        {
            SceneController.instance.GoToHome();
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }
}