using UnityEngine;

/// <summary>
/// Attach to the story slideshow canvas (disabled by default in the scene).
/// Advances through panels on tap; deactivates the assigned fire when finished.
/// </summary>
public class StorySlideshowController : MonoBehaviour
{
    [Tooltip("Story panels in order. Each is a full-screen Image/GameObject shown one at a time.")]
    public GameObject[] panels;

    private int currentIndex;
    private GameObject fireToDeactivateOnFinish;

    /// <summary>
    /// Call this to open the slideshow from panel 0, passing the fire to
    /// auto-deactivate once the slideshow finishes.
    /// </summary>
    public void Open(GameObject fireToDeactivate)
    {
        fireToDeactivateOnFinish = fireToDeactivate;
        currentIndex = 0;
        gameObject.SetActive(true);
        ShowCurrentPanel();
    }

    /// <summary>
    /// Wire a full-screen invisible Button (or EventTrigger PointerClick)
    /// covering the canvas to call this on every tap.
    /// </summary>
    public void OnTapAdvance()
    {
        currentIndex++;

        if (currentIndex >= panels.Length)
        {
            FinishSlideshow();
        }
        else
        {
            ShowCurrentPanel();
        }
    }

    private void ShowCurrentPanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                panels[i].SetActive(i == currentIndex);
            }
        }
    }

    private void FinishSlideshow()
    {
        gameObject.SetActive(false);

        if (fireToDeactivateOnFinish != null)
        {
            fireToDeactivateOnFinish.SetActive(false);
        }
    }
}