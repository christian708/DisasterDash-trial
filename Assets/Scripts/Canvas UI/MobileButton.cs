using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Left,
        Right
    }

    public ButtonType buttonType;

    public static bool MoveLeft;
    public static bool MoveRight;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonType == ButtonType.Left)
            MoveLeft = true;

        if (buttonType == ButtonType.Right)
            MoveRight = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buttonType == ButtonType.Left)
            MoveLeft = false;

        if (buttonType == ButtonType.Right)
            MoveRight = false;
    }
}