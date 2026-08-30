using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TEMPORARY DIAGNOSTIC - attach to any GameObject (e.g. Main Camera) to test
/// if a click hits any Collider2D directly, bypassing EventSystem entirely.
/// Remove this script once the EX_0 issue is resolved.
/// </summary>
public class ClickRaycastDiagnostic : MonoBehaviour
{
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        Debug.Log($"[ClickRaycastDiagnostic] Script is alive. mainCam = {(mainCam != null ? mainCam.name : "NULL")}");
    }

    private float logTimer = 0f;

    private void Update()
    {
        // Heartbeat log every 2 seconds so we know Update is truly running and Mouse.current is valid
        logTimer += Time.deltaTime;
        if (logTimer > 2f)
        {
            logTimer = 0f;
            Debug.Log($"[ClickRaycastDiagnostic] Heartbeat - Mouse.current = {(Mouse.current != null ? "OK" : "NULL")}");
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCam.transform.position.z));

            Collider2D hit = Physics2D.OverlapPoint(worldPoint);

            if (hit != null)
            {
                Debug.Log($"[ClickRaycastDiagnostic] Direct hit: {hit.gameObject.name} at world {worldPoint}");
            }
            else
            {
                Debug.Log($"[ClickRaycastDiagnostic] No collider at world {worldPoint}");
            }
        }
    }
}