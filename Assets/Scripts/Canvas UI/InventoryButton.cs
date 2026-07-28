using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public void OpenInventory()
    {
        Time.timeScale = 1f;

        if (SceneContoller.instance != null)
        {
            SceneContoller.instance.LoadScene("Inventory");
        }
        else
        {
            Debug.LogWarning("SceneContoller instance not found!");
        }
    }
}