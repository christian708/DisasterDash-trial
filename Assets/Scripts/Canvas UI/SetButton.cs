using UnityEngine;

public class SetButton : MonoBehaviour
{
    public void OpenSettings()
    {
        Time.timeScale = 1f;

        Debug.Log("Settings opened");

        // If you have a Settings Scene:
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Settings");
        }
    }
}