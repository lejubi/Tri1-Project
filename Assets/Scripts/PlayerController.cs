using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool hasPowerup;
    private GameManager gameManager;
    public Color originalColor;


    enum Powerup {
        Jump,
        Speed,
        Health,
        Immunity
    }

    private bool immunity = false;
    public int health;
    public int maxHealth = 3;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        UpdateHealthUI();
        // to prevent rotation of player
        rb.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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
    private void Win()
    {
        gameManager.winScreen=true;
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
                gameManager.deathScreen=true;
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

        if (collider.CompareTag("Powerup"))
        {
            Debug.Log("player touching powerup " + collider.gameObject.name);

            hasPowerup = true;

            switch(collider.gameObject.name)
            {
                case "Health_Powerup(Clone)":
                    Debug.Log("in health case");
                    powerup = Powerup.Health;
                    immunity = false;
                    moveSpeed = 5;
                    jumpForce = 10;
                    StartCoroutine(ApplyPowerupEffect());
                    break;
                case "Immunity_Powerup(Clone)":
                    Debug.Log("in debug case");
                    powerup = Powerup.Immunity;
                    moveSpeed = 5;
                    jumpForce = 10;
                    StartCoroutine(ApplyPowerupEffect());
                    break;
                case "Jump_Powerup(Clone)":
                    Debug.Log("in jump case");
                    powerup = Powerup.Jump;
                    immunity = false;
                    moveSpeed = 5;
                    StartCoroutine(ApplyPowerupEffect());
                    break;
                case "Speed_Powerup(Clone)":
                    Debug.Log("in speed case");
                    powerup = Powerup.Speed;
                    immunity = false;
                    jumpForce = 10;
                    StartCoroutine(ApplyPowerupEffect());
                    break;
            }

            Destroy(collider.gameObject);
            Debug.Log("destroyed powerup " + collider.gameObject.name);
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
    private void ApplyColorTint(Color color)
    {
        spriteRenderer.color = color;
    }

    private IEnumerator ApplyPowerupEffect()
    {
        
        switch(powerup)
        {
            case Powerup.Jump:
                jumpForce *= (float)1.4;
                ApplyColorTint(Color.green);
                break;
            case Powerup.Speed:
                moveSpeed *= (float)1.4;
                ApplyColorTint(Color.red);
                break;
            case Powerup.Health:
                addHealth(1);
                spriteRenderer.color = originalColor;
                break;
            case Powerup.Immunity:
                immunity = true;
                ApplyColorTint(Color.blue);
                break;
        }

        Debug.Log("powerup applied");
        Invoke(nameof(ResetPowerupEffectCoroutine), 5f);

        yield return null;
    }

    private void ResetPowerupEffectCoroutine()
    {
        StartCoroutine(ResetPowerupEffect());
    }

    private IEnumerator ResetPowerupEffect()
    {
        switch(powerup)
        {
            case Powerup.Jump:
                jumpForce = 10;
                spriteRenderer.color = originalColor;
                break;
            case Powerup.Speed:
                moveSpeed = 5;
                spriteRenderer.color = originalColor;
                break;
            case Powerup.Health:
                break;
            case Powerup.Immunity:
                immunity = false;
                spriteRenderer.color = originalColor;
                break;
        }

        Debug.Log("powerup reset");
        hasPowerup = false;

        yield return null;
    }
}