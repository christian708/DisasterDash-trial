using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2f);

    [Header("Auto-Fit Settings")]
    [SerializeField] private SpriteRenderer background; // drag this scene's Background here
    [SerializeField] private bool fitWidth = true;        // true = always show full background width
    [SerializeField] private float manualHalfWidth = 5.7f; // fallback only if no background assigned

    [Header("Bounds Clamp")]
    [SerializeField] private bool clampToBackground = true;

    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void Start()
    {
        FitCamera();
    }

    private void FitCamera()
    {
        float screenAspect = (float)Screen.width / Screen.height;

        float halfWidthUnits;
        float halfHeightUnits;

        if (background != null)
        {
            // Pull actual world-space size straight from the sprite — no PPU math needed
            Bounds b = background.bounds;
            halfWidthUnits = b.extents.x;
            halfHeightUnits = b.extents.y;
        }
        else
        {
            halfWidthUnits = manualHalfWidth;
            halfHeightUnits = manualHalfWidth * (Screen.height / (float)Screen.width); // rough fallback
        }

        if (fitWidth)
        {
            cam.orthographicSize = halfWidthUnits / screenAspect;
        }
        else
        {
            float refAspect = halfWidthUnits / halfHeightUnits;
            cam.orthographicSize = (screenAspect >= refAspect)
                ? halfHeightUnits
                : halfHeightUnits * (refAspect / screenAspect);
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            FitCamera();
        }

        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (clampToBackground && background != null)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            Bounds bgBounds = background.bounds;

            float minX = bgBounds.min.x + camHalfWidth;
            float maxX = bgBounds.max.x - camHalfWidth;
            float minY = bgBounds.min.y + camHalfHeight;
            float maxY = bgBounds.max.y - camHalfHeight;

            smoothed.x = (minX <= maxX) ? Mathf.Clamp(smoothed.x, minX, maxX) : bgBounds.center.x;
            smoothed.y = (minY <= maxY) ? Mathf.Clamp(smoothed.y, minY, maxY) : bgBounds.center.y;
        }

        transform.position = smoothed;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null && Application.isPlaying) FitCamera();
    }
#endif
}