using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 30f;       // pixels per second
    [SerializeField] private float resetXPosition = -700f; // where it respawns (left edge)
    [SerializeField] private float despawnXPosition = 700f; // where it wraps (right edge)

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x += speed * Time.deltaTime;

        if (pos.x > despawnXPosition)
        {
            pos.x = resetXPosition;
        }

        rectTransform.anchoredPosition = pos;
    }
}