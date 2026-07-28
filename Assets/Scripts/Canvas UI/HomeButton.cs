using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void GoHome()
    {
        Time.timeScale = 1f;

        if (SceneContoller.instance != null)
        {
            SceneContoller.instance.GoToHome();
        }
        else
        {
            Debug.LogWarning("SceneContoller instance not found!");
        }
    }
}