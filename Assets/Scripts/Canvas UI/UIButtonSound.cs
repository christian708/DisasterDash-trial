using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to any UI Button to play the click sound defined in AudioManager.
/// No setup needed beyond attaching — it hooks into the button's OnClick automatically.
/// </summary>
[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();
    }
}
