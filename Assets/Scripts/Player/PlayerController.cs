using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
        public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

    private bool wasUsingMobileControls = false;

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
        rb.linearVelocity = moveInput * moveSpeed;

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

        // TEMP DEBUG - remove once confirmed
        if (isMoving)
        {
            Debug.Log($"[PlayerController] Setting IsRunning=true on animator: {animator.gameObject.name}");
        }
    }

        public float GetSpeed() => moveSpeed;

        public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}