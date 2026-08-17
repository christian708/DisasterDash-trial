using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    private IInteractable currentInteractable;
    private bool wasUsePressed = false;
    private bool wasPickUpPressed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log("Nearby interactable: " + other.gameObject.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    void Update()
    {
        if (currentInteractable == null)
        {
            wasUsePressed = false;
            wasPickUpPressed = false;
            return;
        }

        bool useDown = MobileButton.UsePressed || (Keyboard.current != null && Keyboard.current.eKey.isPressed);
        bool pickupDown = MobileButton.PickUpPressed || (Keyboard.current != null && Keyboard.current.fKey.isPressed);

        if (useDown && !wasUsePressed)
        {
            currentInteractable.OnUse();
        }

        if (pickupDown && !wasPickUpPressed)
        {
            currentInteractable.OnPickUp();
        }

        wasUsePressed = useDown;
        wasPickUpPressed = pickupDown;
    }
}