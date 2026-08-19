using UnityEngine;

/// <summary>
/// Use this on the Main Camera for OUTDOOR / SIDE-SCROLLING scenes only
/// (e.g. Fire Stage, school exterior, etc.) where the player moves and
/// the camera needs to follow while staying clamped to the background.
///
/// Do NOT combine with "Camera Auto Fit" on the same camera - this script
/// already handles fitting by reading the Background sprite's bounds.
///
/// For static INTERIOR scenes (kitchen, etc.) where the whole room fits
/// on screen and the camera doesn't need to scroll, use "Camera Auto Fit"
/// instead, not this script.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollowOutdoor : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 2f);

    [Header("Fit Settings")]
    [Tooltip("Drag this scene's Background SpriteRenderer here. Used for clamping bounds only.")]
    [SerializeField] private SpriteRenderer background;
    [Tooltip("Use a fixed manual ortho size instead of fitting the full background width. Turn this ON when Background is a large combined/multi-scene strip (camera should only ever show a slice of it, not the whole thing).")]
    [SerializeField] private bool useFixedSize = true;
    [Tooltip("Orthographic size to use when useFixedSize is true. Tune this to match how 'zoomed in' the camera should feel on this level's art.")]
    [SerializeField] private float fixedOrthoSize = 3.5f;
    [Tooltip("Only used when useFixedSize is false - fits the full background WIDTH on screen (good for single-scene-per-image backgrounds).")]
    [SerializeField] private bool fitWidth = true;

    [Header("Bounds Clamp")]
    [SerializeField] private bool clampToBackground = true;

    [Header("Auto-Find (optional convenience)")]
    [Tooltip("If Target/Background aren't assigned, try to find them automatically by tag/name.")]
    [SerializeField] private bool autoFindIfMissing = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string backgroundObjectName = "Background";

    private Camera cam;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        if (autoFindIfMissing)
        {
            if (target == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
                if (playerObj != null) target = playerObj.transform;
            }

            if (background == null)
            {
                GameObject bgObj = GameObject.Find(backgroundObjectName);
                if (bgObj != null) background = bgObj.GetComponent<SpriteRenderer>();
            }
        }
    }

    private void Start()
    {
        FitCamera();
    }

    private void FitCamera()
    {
        if (useFixedSize)
        {
            // Manual fixed zoom - correct choice when Background is a large
            // combined/multi-scene strip that should never be fully shown at once.
            cam.orthographicSize = fixedOrthoSize;
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            return;
        }

        if (background == null)
        {
            Debug.LogWarning("[CameraFollowOutdoor] Background not assigned - camera fit will be skipped.", this);
            return;
        }

        float screenAspect = (float)Screen.width / Screen.height;
        Bounds b = background.bounds;
        float halfWidthUnits = b.extents.x;
        float halfHeightUnits = b.extents.y;

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
        // Re-fit if resolution/orientation changes at runtime (device rotation, simulator swap, etc.)
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