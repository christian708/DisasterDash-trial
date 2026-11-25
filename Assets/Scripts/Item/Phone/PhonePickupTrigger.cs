using UnityEngine;

/// <summary>
/// Attach to the phone GameObject. When the player is near it and presses B (Pick),
/// the phone disappears, the story slideshow plays, then the fire is deactivated.
/// One-off scene trigger, separate from the inventory system.
/// </summary>
public class PhonePickupTrigger : MonoBehaviour, IInteractable
{
    [Tooltip("The slideshow controller, disabled by default in the scene.")]
    public StorySlideshowController slideshow;

    [Tooltip("The massive fire to deactivate automatically once the slideshow ends.")]
    public GameObject fireToDeactivate;

    private bool used;

    // B button
    public void OnPickUp()
    {
        TriggerPhone();
    }

    // A button: phone can't be "used", so do nothing
    public void OnUse() { }

    public void TriggerPhone()
    {
        if (used) return;
        used = true;

        gameObject.SetActive(false); // phone disappears after pickup

        if (slideshow != null)
        {
            slideshow.Open(fireToDeactivate);
        }
    }
}