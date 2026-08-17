using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public void OpenInventory()
    {
        Time.timeScale = 1f;

        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("Inventory");
        }
        else
        {
            Debug.LogWarning("SceneController instance not found!");
        }
    }
}