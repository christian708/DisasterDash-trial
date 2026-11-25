using UnityEngine;

/// <summary>
/// Add this to a Branch or RockHazard scene object so it only starts falling
/// when the player walks within range. The debris stays frozen at its placed
/// position until proximity is detected, then it releases and falls normally.
///
/// Setup:
///   1. Place the Branch / RockHazard GameObject where you want it in the level.
///   2. Add this component to the same GameObject.
///   3. Set Trigger Radius to how close the player needs to be (world units).
///   4. The debris will hang in place and begin falling the moment the player
///      enters that radius.
/// </summary>
public class DebrisProximityTrigger : MonoBehaviour
{
    [Tooltip("How close (world units) the player must be before this debris starts falling.")]
    [SerializeField] private float triggerRadius = 5f;

    [Tooltip("Show a Gizmo circle in the editor so you can see the trigger range.")]
    [SerializeField] private bool showGizmo = true;

    private Transform playerTransform;
    private bool triggered = false;

    // Cache references to the falling scripts so we can freeze/unfreeze them.
    private Branch branch;
    private RockHazard rockHazard;

    private void Awake()
    {
        branch = GetComponent<Branch>();
        rockHazard = GetComponent<RockHazard>();

        // Freeze whichever script is present until the player is nearby.
        SetFalling(false);
    }

    private void Start()
    {
        // Find the player once at start.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (triggered) return;
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= triggerRadius)
        {
            triggered = true;
            SetFalling(true);
        }
    }

    /// <summary>Enables or disables the falling behaviour on Branch / RockHazard.</summary>
    private void SetFalling(bool falling)
    {
        if (branch != null) branch.enabled = falling;
        if (rockHazard != null) rockHazard.enabled = falling;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
