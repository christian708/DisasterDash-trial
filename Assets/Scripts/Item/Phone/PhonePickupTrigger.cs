using UnityEngine;

/// <summary>
/// Attach to the phone GameObject in the scene. Clicking it plays the story
/// slideshow, then automatically deactivates the assigned fire — no item
/// needed. This is a one-off scene trigger, separate from the inventory system.
/// </summary>
public class PhonePickupTrigger : MonoBehaviour
{
    [Tooltip("The slideshow controller, disabled by default in the scene.")]
    public StorySlideshowController slideshow;

    [Tooltip("The massive fire to deactivate automatically once the slideshow ends.")]
    public GameObject fireToDeactivate;

    private bool used;

    private void OnMouseDown()
    {
        TriggerPhone();
    }

    public void TriggerPhone()
    {
        if (used) return;
        used = true;

        gameObject.SetActive(false); // phone disappears once picked up

        if (slideshow != null)
        {
            slideshow.Open(fireToDeactivate);
        }
    }
}