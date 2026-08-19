using UnityEngine;
using UnityEngine.UI;

public class BlinkClickIcon : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    private Image icon;

    void Awake()
    {
        icon = GetComponent<Image>();
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f);
        Color c = icon.color;
        c.a = alpha;
        icon.color = c;
    }
}