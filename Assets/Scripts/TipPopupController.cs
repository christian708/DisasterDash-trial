using UnityEngine;

public class TipPopupController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject popupPanel; // the UI Image object with the tips baked in

    void Awake()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); // hidden by default
        }
    }

    public void Show()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    public void Hide()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
}