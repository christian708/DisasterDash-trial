using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed = 5f;

    // Keyboard/Gamepad input
    private Vector2 moveInput;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Called by the Player Input component (Keyboard/Gamepad)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        // Mobile Controls
        if (MobileButton.MoveLeft)
        {
            moveInput = Vector2.left;
        }
        else if (MobileButton.MoveRight)
        {
            moveInput = Vector2.right;
        }
        else
        {
            // Stop moving when no mobile button is pressed
            moveInput = Vector2.zero;
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

        // Flip sprite
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void HandleAnimation()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsRunning", isMoving);
    }
}