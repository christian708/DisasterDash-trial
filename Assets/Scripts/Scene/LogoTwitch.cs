using UnityEngine;

public class LogoTwitch : MonoBehaviour
{
    [Header("Twitch Settings")]
    [SerializeField] private float twitchIntensity = 1.5f;  // was 5 — much smaller shift
    [SerializeField] private float twitchSpeed = 0.25f;      // was 0.05 — twitches less often
    [SerializeField] private bool twitchRotation = true;
    [SerializeField] private float rotationIntensity = 0.5f; // was 2 — subtler wobble

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Quaternion originalRotation;
    private float timer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= twitchSpeed)
        {
            timer = 0f;

            float offsetX = Random.Range(-twitchIntensity, twitchIntensity);
            float offsetY = Random.Range(-twitchIntensity, twitchIntensity);
            rectTransform.anchoredPosition = originalPosition + new Vector2(offsetX, offsetY);

            if (twitchRotation)
            {
                float randomZ = Random.Range(-rotationIntensity, rotationIntensity);
                rectTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, randomZ);
            }
        }
    }
}