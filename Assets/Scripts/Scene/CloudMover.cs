using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 30f; // pixels per second

    [Header("Edge Buffer")]
    [Tooltip("Extra pixels beyond the canvas edge before wrapping, so the cloud fully disappears before resetting (avoids popping into the middle).")]
    [SerializeField] private float edgeBuffer = 200f;

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private float startY; // wherever you drag the cloud to, its height is preserved on every loop

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y;
    }

    void Update()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x += speed * Time.deltaTime;

        float halfCanvasWidth = canvasRect.rect.width / 2f;
        float despawnX = halfCanvasWidth + edgeBuffer;
        float resetX = -halfCanvasWidth - edgeBuffer;

        if (pos.x > despawnX)
        {
            pos.x = resetX;
            pos.y = startY; // keep the same height it was originally placed at
        }

        rectTransform.anchoredPosition = pos;
    }
}