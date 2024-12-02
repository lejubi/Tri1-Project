using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;

    public Powerup powerup;
    public float moveSpeed;
    public float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask harmfulObjectLayer;
    [SerializeField] private float invincibilityDuration = 1f;
    private bool isGrounded;
    private bool playerIsFalling;
    private float currentFallTime;
    public float MaxFallTime = 3f;
    private float horizontalMove = 0f;
    private bool isJumping = false;
    public bool hasPowerup;
    private GameManager gameManager;
    public Color originalColor;

    private Powerup currentPowerup;
    private Coroutine powerupCoroutine;

    public enum PlayerState
    {
        Idle,
        Running,
        JumpingUp,
        JumpingDown
    }
    public PlayerState playerState;


    public bool immunity = false;
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
        CheckFalling();
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
    private void CheckWin()
    {
        gameManager.winScreen = true;
    }

    void UpdateAnimationState()
    {
        // updating animation of character based on action
        if (isGrounded)
        {
            if (Mathf.Abs(horizontalMove) > 0.1f)
            {
                playerState = PlayerState.Running;
            }
            else
            {
                playerState = PlayerState.Idle;
            }
        }
        else
        {
            playerState = rb.velocity.y > 0 ? PlayerState.JumpingUp : PlayerState.JumpingDown;
        }
        animator.SetBool("isRunning", playerState == PlayerState.Running);
        animator.SetBool("isIdle", playerState == PlayerState.Idle);
        animator.SetBool("isJumpUp", playerState == PlayerState.JumpingUp);
        animator.SetBool("isJumpDown", playerState == PlayerState.JumpingDown);

        if (horizontalMove > 0)
            transform.localScale = new Vector2(20, 20);
        else if (horizontalMove < 0)
            transform.localScale = new Vector2(-20, 20);
    }

    private void CheckFalling()
    {
        if (!isGrounded)
        {
            currentFallTime += Time.deltaTime;
        }
        else
        {
            currentFallTime = 0;
        }
        if (currentFallTime >= MaxFallTime)
        {
            gameManager.deathScreen = true;
        }
    }

    // powerup methods
    public void TakeDamage(int damage)
    {
        if (!immunity)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthUI();
            if (health <= 0)
            {
                gameManager.deathScreen = true;
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

        if (collider.CompareTag("WinPortal"))
        {
            Debug.Log("You Won!");
            gameManager.winScreen = true;
        }

        if (collider.CompareTag("Powerup"))
        {
            Debug.Log("player touching powerup " + collider.gameObject.name);

            hasPowerup = true;

            Powerup newPowerup = collider.GetComponent<PowerupDisplay>().powerup;

            // switch(collider.gameObject.name)
            // {
            //     case "Health_Powerup(Clone)":
            //         Debug.Log("in health case");
            //         powerup = Powerup.Health;
            //         immunity = false;
            //         moveSpeed = 5;
            //         jumpForce = 10;
            //         StartCoroutine(ApplyPowerupEffect());
            //         break;
            //     case "Immunity_Powerup(Clone)":
            //         Debug.Log("in debug case");
            //         powerup = Powerup.Immunity;
            //         moveSpeed = 5;
            //         jumpForce = 10;
            //         StartCoroutine(ApplyPowerupEffect());
            //         break;
            //     case "Jump_Powerup(Clone)":
            //         Debug.Log("in jump case");
            //         powerup = Powerup.Jump;
            //         immunity = false;
            //         moveSpeed = 5;
            //         StartCoroutine(ApplyPowerupEffect());
            //         break;
            //     case "Speed_Powerup(Clone)":
            //         Debug.Log("in speed case");
            //         powerup = Powerup.Speed;
            //         immunity = false;
            //         jumpForce = 10;
            //         StartCoroutine(ApplyPowerupEffect());
            //         break;
            // }

            if (newPowerup != null)
            {
                if (powerupCoroutine != null)
                {
                    StopCoroutine(powerupCoroutine);
                    if (currentPowerup != null)
                    {
                        currentPowerup.RemovePowerupEffect(this);
                    }
                }
                currentPowerup = newPowerup;
                powerupCoroutine = StartCoroutine(ApplyPowerupEffect(newPowerup));
                Destroy(collider.gameObject);
            }
            Debug.Log("destroyed powerup");
            Destroy(collider.gameObject);
        }
    }
    private IEnumerator InvincibilityFrames()
    {
        immunity = true;
        StartCoroutine(FlickerEffect());
        Invoke(nameof(ResetInvincibility), invincibilityDuration);
        yield return new WaitForSeconds(invincibilityDuration);
    }
    private void ResetInvincibility()
    {
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
        health += h;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
    }
    public void UpdateHealthUI()
    {
        HealthUI healthUI = FindObjectOfType<HealthUI>();
        if (healthUI != null)
        {
            healthUI.InitializeHearts();
        }
    }
    public void ApplyColorTint(Color color)
    {
        Color tintedColor = color;
        tintedColor.a = spriteRenderer.color.a;
        spriteRenderer.color = tintedColor;
    }

    private IEnumerator ApplyPowerupEffect(Powerup powerup)
    {
        powerup.ApplyPowerupEffect(this);
        Debug.Log("powerup applied");

        yield return new WaitForSeconds(5f);

        powerup.RemovePowerupEffect(this);
        Debug.Log("powerup removed");

        currentPowerup = null;
        hasPowerup = false;
    }

    // private void ResetPowerupEffectCoroutine()
    // {
    //     switch (powerup)
    //     {
    //         case Powerup.Jump:
    //             jumpForce = 10;
    //             spriteRenderer.color = originalColor;
    //             break;
    //         case Powerup.Speed:
    //             moveSpeed = 5;
    //             spriteRenderer.color = originalColor;
    //             break;
    //         case Powerup.Health:
    //             break;
    //         case Powerup.Immunity:
    //             immunity = false;
    //             spriteRenderer.color = originalColor;
    //             break;
    //     }

    //     Debug.Log("powerup reset");
    //     hasPowerup = false;
    // }
}