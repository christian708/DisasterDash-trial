using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LandslideBlock : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1.5f;
    [SerializeField] float shakeAmount = 0.05f;
    public UnityEvent onRemoved;

    bool removing;

    public void RemoveLandslide()
    {
        if (removing) return;
        removing = true;
        StartCoroutine(RemoveRoutine());
    }

    IEnumerator RemoveRoutine()
    {
        // stop blocking immediately
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeDuration);

            foreach (var r in renderers)
            {
                var col = r.color;
                col.a = a;
                r.color = col;
            }

            transform.position = startPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }

        transform.position = startPos;
        onRemoved?.Invoke();
        gameObject.SetActive(false);
    }
}