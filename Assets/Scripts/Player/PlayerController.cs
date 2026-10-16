using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;

    private bool wasUsingMobileControls = false;

    // Multiplier applied on top of moveSpeed by hazards like wind/flood.
    // 1 = normal speed, 0.5 = half speed, etc. Reset to 1 on trigger exit.
    private float speedMultiplier = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        FindActiveCharacter();
    }

    private void FindActiveCharacter()
    {
        Animator[] animators = GetComponentsInChildren<Animator>(true);
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);

        animator = null;
        spriteRenderer = null;

        foreach (Animator a in animators)
        {
            if (a.gameObject.activeInHierarchy)
            {
                animator = a;
                break;
            }
        }

        foreach (SpriteRenderer s in sprites)
        {
            if (s.gameObject.activeInHierarchy)
            {
                spriteRenderer = s;
                break;
            }
        }
    }

    // Keyboard / Player Input
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        if (animator == null || !animator.gameObject.activeInHierarchy)
        {
            FindActiveCharacter();
        }

        // MOBILE CONTROLS
        bool mobileMoving =
            MobileButton.MoveLeft ||
            MobileButton.MoveRight;

        if (MobileButton.MoveLeft)
        {
            moveInput = Vector2.left;
            wasUsingMobileControls = true;
        }
        else if (MobileButton.MoveRight)
        {
            moveInput = Vector2.right;
            wasUsingMobileControls = true;
        }
        else if (wasUsingMobileControls)
        {
            // Mobile button was released
            moveInput = Vector2.zero;
            wasUsingMobileControls = false;
        }

        HandleAnimation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * speedMultiplier, rb.linearVelocity.y);

        if (spriteRenderer != null)
        {
            if (moveInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void HandleAnimation()
    {
        if (animator == null)
            return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        animator.SetBool("IsRunning", isMoving);
    }

    /// <summary>Whether the player currently has movement input.</summary>
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    /// <summary>
    /// Call from hazards like WindGust/FloodOverlay to temporarily change speed.
    /// 1 = normal, 0.5 = half speed. Call again with 1 to reset (e.g. on trigger exit).
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// <summary>
    /// Sets moveSpeed directly to an absolute value (used by Wind.cs).
    /// Note: this is a separate mechanism from SetSpeedMultiplier - avoid
    /// combining wind and flood/other multiplier-based hazards at the same
    /// time on the same player, since they'd fight over the same speed value.
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}