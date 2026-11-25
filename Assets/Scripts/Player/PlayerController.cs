using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    private Vector2 moveInput;

    private bool wasUsingMobileControls = false;

    // Multiplier applied on top of moveSpeed by hazards like wind/flood.
    // 1 = normal speed, 0.5 = half speed, etc. Reset to 1 on trigger exit.
    private float speedMultiplier = 1f;

    // Constant horizontal push from wind (+ = right, - = left). Reset to 0 on exit.
    private float windPush = 0f;

    // Crouch (set automatically by CrouchZone)
    private bool isCrouching = false;
    private Vector2 normalColSize;
    private Vector2 normalColOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (col is BoxCollider2D box)
        {
            normalColSize = box.size;
            normalColOffset = box.offset;
        }

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
        float crouchMult = isCrouching ? crouchSpeedMultiplier : 1f;

        float inputX = moveInput.x;

        // Wind pushing left  -> block moving right (against the wind)
        // Wind pushing right -> block moving left
        if (windPush < 0f && inputX > 0f) inputX = 0f;
        else if (windPush > 0f && inputX < 0f) inputX = 0f;

        rb.linearVelocity = new Vector2(
            inputX * moveSpeed * speedMultiplier * crouchMult + windPush,
            rb.linearVelocity.y);

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

    private bool wasMoving = false;

    private void HandleAnimation()
    {
        if (animator == null)
            return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        animator.SetBool("IsRunning", isMoving);
        animator.SetBool("IsCrouching", isCrouching);

        // Only call AudioManager on state change, not every frame
        if (isMoving != wasMoving)
        {
            wasMoving = isMoving;
            if (isMoving)
                AudioManager.Instance?.PlayRunSound();
            else
                AudioManager.Instance?.StopRunSound();
        }
    }

    /// <summary>Whether the player currently has movement input.</summary>
    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    /// <summary>Whether the player is currently crouched (under a table).</summary>
    public bool IsCrouching => isCrouching;

    /// <summary>
    /// Called by CrouchZone when the player enters/exits a crouch area.
    /// Shrinks the collider and drives the IsCrouching animator bool.
    /// </summary>
    public void SetCrouch(bool crouch)
    {
        if (isCrouching == crouch) return;
        isCrouching = crouch;

        if (col is BoxCollider2D box)
        {
            if (crouch)
            {
                float newH = normalColSize.y * 0.5f;
                box.size = new Vector2(normalColSize.x, newH);
                box.offset = new Vector2(
                    normalColOffset.x,
                    normalColOffset.y - (normalColSize.y - newH) / 2f);
            }
            else
            {
                box.size = normalColSize;
                box.offset = normalColOffset;
            }
        }
    }

    /// <summary>
    /// Call from hazards like wind/flood to temporarily change speed.
    /// 1 = normal, 0.5 = half speed. Call again with 1 to reset (e.g. on trigger exit).
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// <summary>
    /// Sets moveSpeed directly to an absolute value. Kept for compatibility.
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    /// <summary>
    /// Constant horizontal push from wind. Negative = pushed left, 0 = no push.
    /// While non-zero, input against the wind direction is blocked.
    /// </summary>
    public void SetWindPush(float push)
    {
        windPush = push;
    }
}