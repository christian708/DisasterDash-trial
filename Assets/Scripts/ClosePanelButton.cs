using UnityEngine;
using UnityEngine.UI;

public class ClosePanelButton : MonoBehaviour
{
    [SerializeField] private GameObject panelToClose; // the Warning panel

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ClosePanel);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(ClosePanel);
    }

    public void ClosePanel()
    {
        panelToClose.SetActive(false);
    }
}