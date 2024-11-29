using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class PlayerController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField] private Powerup powerup;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask harmfulObjectLayer;
    [SerializeField] private float invincibilityDuration = 1f;
    private bool isGrounded;
    private float horizontalMove = 0f;
    private bool isJumping = false;
    private bool immunity = false;
    public int health;
    public int maxHealth = 3;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        UpdateHealthUI();
        // to prevent rotation of player
        rb.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveSpeed += powerup.speed;
        jumpForce += powerup.jump;
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
            if (rb.velocity.y > 0)
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
            transform.localScale = new Vector2(20, 20);
        else if (horizontalMove < 0)
            transform.localScale = new Vector2(-20, 20);
    }

        // powerup methods
    public void TakeDamage(int damage)
    {
        if(!immunity)
        {
            health-=damage;
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthUI();
            if (health <= 0)
            {
                Debug.Log("Player has died!");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("HarmfulObject"))
        {
            if (immunity == false)
            {
                TakeDamage(1);
                StartCoroutine(InvincibilityFrames());
            }
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        immunity = true;
        StartCoroutine(FlickerEffect());
        yield return new WaitForSeconds(invincibilityDuration);
        immunity = false;
        spriteRenderer.enabled = true;
    }
    private IEnumerator FlickerEffect()
{
    float flickerInterval = 0.1f; 
    while (immunity)
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
        yield return new WaitForSeconds(flickerInterval);
    }
    spriteRenderer.enabled = true; 
}
    public void addHealth(int h)
    {
        health+=h;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }
    public void UpdateHealthUI()
    {
        HealthUI healthUI = FindObjectOfType<HealthUI>();
        if(healthUI!=null)
        {
            healthUI.InitializeHearts();
        }
    }

    public void changeSpeed(int speed)
    {
        moveSpeed += speed;
    }


    public void setImmunityTrue()
    {
        immunity = true;
    }

    public void setImmunityFalse()
    {
        immunity = false;
    }

    public void changeJumpForce(int force)
    {
        jumpForce += force;
    }
}