using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    [Tooltip("Reference to the InvSceneNav component on the Bag overlay panel in this scene.")]
    public InvSceneNav invSceneNav;

    public void OpenInventory()
    {
        Time.timeScale = 1f;

        if (invSceneNav != null)
        {
            invSceneNav.OpenBag();
        }
        else
        {
            Debug.LogWarning("InvSceneNav reference not set on InventoryButton!");
        }
    }
}