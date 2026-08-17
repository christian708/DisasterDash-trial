using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Left,
        Right,
        Use,     // A button
        PickUp   // B button
    }

    public ButtonType buttonType;

    public static bool MoveLeft;
    public static bool MoveRight;
    public static bool UsePressed;
    public static bool PickUpPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetState(true);

        // Debug feedback
        if (buttonType == ButtonType.Use)
            Debug.Log("A Button Pressed (Use)");

        if (buttonType == ButtonType.PickUp)
            Debug.Log("B Button Pressed (PickUp)");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetState(false);
    }

    private void SetState(bool isPressed)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                MoveLeft = isPressed;
                break;
            case ButtonType.Right:
                MoveRight = isPressed;
                break;
            case ButtonType.Use:
                UsePressed = isPressed;
                break;
            case ButtonType.PickUp:
                PickUpPressed = isPressed;
                break;
        }
    }

    private void OnDisable()
    {
        SetState(false);
    }
}