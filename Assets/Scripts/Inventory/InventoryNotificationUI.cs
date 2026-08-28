using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Put this on a UI object that has a CanvasGroup + TMP_Text, starting at alpha 0.
/// Call InventoryNotificationUI.Instance.ShowMessage(...) to fade it in, hold, fade out.
/// </summary>
public class InventoryNotificationUI : MonoBehaviour
{
    public static InventoryNotificationUI Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showDuration = 1.5f;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine activeRoutine;

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        yield return new WaitForSeconds(showDuration);
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}