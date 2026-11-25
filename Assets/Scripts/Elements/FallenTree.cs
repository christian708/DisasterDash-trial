using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fallen tree that blocks the path. PlayerHeldItem calls Chop(playerAnimator).
/// Player plays the chop animation, then the tree fades away and stops blocking.
/// </summary>
public class FallenTree : MonoBehaviour
{
    [Header("Blocking")]
    [Tooltip("Solid (non-trigger) colliders that stop the player.")]
    public Collider2D[] blockers;

    [Header("Player chop animation")]
    public string playerChopTrigger = "Chop";
    [Tooltip("Seconds the player's chop clip takes. Match it to the clip length.")]
    public float chopDuration = 1f;
    [Tooltip("Optional: seconds into the chop when the tree starts fading (the hit moment). 0 = wait for the full chop.")]
    public float fadeStartTime = 0f;

    [Header("Fade")]
    public float fadeDuration = 0.8f;

    public UnityEvent onCleared;

    private bool chopped;

    public void Chop(Animator playerAnimator)
    {
        if (chopped) return;
        chopped = true;
        StartCoroutine(ChopRoutine(playerAnimator));
    }

    private IEnumerator ChopRoutine(Animator playerAnimator)
    {
        // freeze the player's walking during the chop
        var pc = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (pc != null) pc.SetSpeedMultiplier(0f);

        if (playerAnimator != null)
            playerAnimator.SetTrigger(playerChopTrigger);

        float wait = fadeStartTime > 0f ? fadeStartTime : chopDuration;
        yield return new WaitForSeconds(wait);

        // path clears as the fade begins
        foreach (var c in blockers)
            if (c != null) c.enabled = false;

        // if the fade starts mid-chop, let the rest of the chop finish too
        float remainingChop = Mathf.Max(0f, chopDuration - wait);

        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        float f = 0f;
        while (f < fadeDuration)
        {
            f += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(f / fadeDuration);
            foreach (var sr in srs)
            {
                if (sr == null) continue;
                Color col = sr.color;
                col.a = a;
                sr.color = col;
            }
            yield return null;
        }

        if (remainingChop > fadeDuration)
            yield return new WaitForSeconds(remainingChop - fadeDuration);

        if (pc != null) pc.SetSpeedMultiplier(1f);

        onCleared?.Invoke();
        gameObject.SetActive(false);
    }
}