using UnityEngine;

/// <summary>
/// Attach to the "?" hint icon in the scene. Clicking it shows a popup image;
/// the popup's own X button (wired to ClosePopup in the Inspector) closes the
/// popup and removes the "?" icon so it can't be reopened.
/// </summary>
public class HintIconPopup : MonoBehaviour
{
    [Tooltip("The popup panel (image + X button), disabled by default in the scene.")]
    public GameObject popupPanel;

    private void OnMouseDown()
    {
        ShowPopup();
    }

    public void ShowPopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Wire the popup's X button OnClick() to this method.
    /// </summary>
    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        gameObject.SetActive(false); // remove the "?" icon permanently
    }
}