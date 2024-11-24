using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private bool isGrounded;
    private float horizontalMove = 0f;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        // to prevent rotation of player
        rb.freezeRotation = true; 
    }

    void Update()
    {
        // checking if player is on the ground and if they are trying to move or jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            isJumping = true;
        }

        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMove, rb.velocity.y);

        if (isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = false;
        }
    }

    void UpdateAnimationState()
    {
        // updating animation of character based on action
        if (isGrounded)
        {
            if (Mathf.Abs(horizontalMove) > 0.1f)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isIdle", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isIdle", true);
            }
            animator.SetBool("isJumpUp", false);
            animator.SetBool("isJumpDown", false);
        }
        else
        {
            if (rb.velocity.y > 0 )
            {
                animator.SetBool("isJumpUp", true);
                animator.SetBool("isJumpDown", false);
            }
            else
            {
                animator.SetBool("isJumpUp", false);
                animator.SetBool("isJumpDown", true);
            }
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", false);
        }

        if (horizontalMove > 0)
            transform.localScale = new Vector3(15, 15, 15);
        else if (horizontalMove < 0)
            transform.localScale = new Vector3(-15, 15, 15);
    }
}