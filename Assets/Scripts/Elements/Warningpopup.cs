using System.Collections;
using UnityEngine;

/// <summary>
/// Blinks a warning image on and off (like a warning light) for a set duration, then hides it.
/// Attach anywhere (e.g. the Canvas or an empty manager object) and assign the warning
/// Image's GameObject in the Inspector.
/// </summary>
public class Warningpopup : MonoBehaviour
{
    [SerializeField] private GameObject warningImage; // the warning Image GameObject inside your Canvas
    [SerializeField] private float blinkInterval = 0.3f; // seconds between each show/hide toggle

    private void Awake()
    {
        if (warningImage != null) warningImage.SetActive(false);
    }

    /// <summary>
    /// Blinks the warning image on/off for 'duration' seconds, then hides it and returns.
    /// Call with: yield return StartCoroutine(warningPopup.Show(2f));
    /// </summary>
    public IEnumerator Show(float duration)
    {
        if (warningImage == null) yield break;

        float elapsed = 0f;
        bool visible = false;

        while (elapsed < duration)
        {
            visible = !visible;
            warningImage.SetActive(visible);

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        warningImage.SetActive(false);
    }
}