using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Input Things")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference activateAction;
    public InputActionReference resetAction;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 35f;
    public float deceleration = 2f;
    public float maxHorizontalSpeed = 25f;

    [Header("Jump Fields")]
    public float jumpForce = 14f;
    public float gravityMultiplier = 2.5f;
    public float fallGravityMultiplier = 3.5f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("Activate Fields")]
    public float activateCooldown = 0.1f;
    public float activateSpeedMult = 1.3f;

    [Header("Ground Check Stuff")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Footsteps")]
    public AudioSource footstepSound;
    public float minFootstepSpeed = 2f;
    public float maxFootstepSpeed = 25f;
    public float maxFootstepInterval = 0.45f;
    public float minFootstepInterval = 0.15f;

    [Header("Other Things")]
    public float levelSwitchVelocityMult = 0.5f;
    public float wallBounceMult = 0.5f;
    public float minWallBounceSpeed = 5f;
    public float wallBounceControlLock = 0.12f;
    public GameObject explodeParticle;
    public LevelManager levelManager;

    public AudioSource jumpSound;
    public AudioSource dieSound;

    Rigidbody2D rb;
    PlayerAnimator playerAnimator;
    public float moveInput;
    float coyoteTimer;
    float jumpBufferTimer;
    float activateTimer;
    float wallBounceTimer;
    float footstepTimer;
    bool grounded;
    bool isDying;

    public bool IsGrounded => grounded;
    public float HorizontalVelocity => rb != null ? rb.linearVelocity.x : 0f;
    public float VerticalVelocity => rb != null ? rb.linearVelocity.y : 0f;

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJump;
        }

        if (activateAction != null)
        {
            activateAction.action.Enable();
            activateAction.action.performed += OnActivate;
        }

        if (resetAction != null)
        {
            resetAction.action.Enable();
            resetAction.action.performed += OnReset;
        }
    }

    void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJump;
            jumpAction.action.Disable();
        }

        if (activateAction != null)
        {
            activateAction.action.performed -= OnActivate;
            activateAction.action.Disable();
        }

        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        transform.position = levelManager.playerSpawnPoints[levelManager.currentLevel].position;
    }

    void Update()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.action.ReadValue<Vector2>().x;
        }

        if (grounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (activateTimer > 0f)
        {
            activateTimer -= Time.deltaTime;
        }

        UpdateFootsteps();
    }

    void OnReset(InputAction.CallbackContext context)
    {
        transform.position = levelManager.playerSpawnPoints[levelManager.currentLevel].position;
        rb.linearVelocity = Vector2.zero;
    }

    void UpdateFootsteps()
    {
        if (isDying || !grounded || Mathf.Abs(moveInput) < 0.01f || Mathf.Abs(rb.linearVelocity.x) < minFootstepSpeed)
        {
            footstepTimer = 0f;
            return;
        }

        float speedPercent = Mathf.InverseLerp(minFootstepSpeed, Mathf.Max(maxFootstepSpeed, minFootstepSpeed + 0.01f), Mathf.Abs(rb.linearVelocity.x));
        float interval = Mathf.Lerp(maxFootstepInterval, minFootstepInterval, speedPercent);

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (footstepSound != null)
            {
                footstepSound.Play();
            }

            footstepTimer = interval;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (!isDying)
        {
            jumpBufferTimer = jumpBufferTime;

            if (jumpSound != null)
            {
                jumpSound.Play();
            }
            StartTimer();
        }
    }

    void OnActivate(InputAction.CallbackContext context)
    {
        if (isDying || activateTimer > 0f || rb == null)
        {
            return;
        }

        float direction = rb.linearVelocity.x == 0f ? (moveInput == 0f ? 1f : Mathf.Sign(moveInput)) : -Mathf.Sign(rb.linearVelocity.x);
        float speed = Mathf.Abs(rb.linearVelocity.x) * activateSpeedMult;

        rb.linearVelocityX = direction * Mathf.Min(speed, maxHorizontalSpeed);
        activateTimer = activateCooldown;
    }

    void FixedUpdate()
    {
        if (rb == null || isDying)
        {
            return;
        }

        grounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (wallBounceTimer > 0f)
        {
            wallBounceTimer -= Time.fixedDeltaTime;
        }
        else
        {
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                rb.AddForce(Vector2.right * moveInput * acceleration, ForceMode2D.Force);
            }
            else
            {
                rb.AddForce(Vector2.right * -rb.linearVelocity.x * deceleration, ForceMode2D.Force);
            }
        }

        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocityY = jumpForce;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = gravityMultiplier * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityMultiplier;
        }

        if (moveInput != 0)
        {
            StartTimer();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDying)
        {
            return;
        }

        if (collision.CompareTag("CamBound"))
        {
            CameraBound camBound = collision.GetComponent<CameraBound>();

            if (camBound == null || !camBound.boundsEnabled)
            {
                return;
            }

            if (levelManager.currentLevel + 1 >= levelManager.cameras.Count)
            {
                levelManager.CompleteGame();
                return;
            }

            if (levelManager.selectedCam != levelManager.cameras[levelManager.currentLevel + 1])
            {
                levelManager.DisableAllCameras();
                levelManager.currentLevel++;
                levelManager.EnableCamera(levelManager.cameras[levelManager.currentLevel]);
                transform.position = levelManager.playerSpawnPoints[levelManager.currentLevel].position;
                rb.linearVelocityX *= levelSwitchVelocityMult;
            }
        }

        if (collision.CompareTag("KillObj"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        footstepTimer = 0f;

        if (explodeParticle != null)
        {
            GameObject instantiatedExplosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);
            Destroy(instantiatedExplosion, 2f);
        }

        if (playerAnimator != null)
        {
            playerAnimator.PlayDeath();
        }

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (levelManager != null)
        {
            levelManager.AddDeath();
        }

        if (dieSound != null)
        {
            dieSound.Play();
        }

        Invoke(nameof(Respawn), HasDeathAnimation() ? GetDeathAnimationDuration() : 0f);
    }

    bool HasDeathAnimation()
    {
        return playerAnimator != null && playerAnimator.HasDeathAnimation;
    }

    float GetDeathAnimationDuration()
    {
        return playerAnimator != null ? playerAnimator.DeathAnimationDuration : 0f;
    }

    void Respawn()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.playerSpawnPoints != null && LevelManager.Instance.currentLevel >= 0 && LevelManager.Instance.currentLevel < LevelManager.Instance.playerSpawnPoints.Count && LevelManager.Instance.playerSpawnPoints[LevelManager.Instance.currentLevel] != null)
        {
            transform.position = LevelManager.Instance.playerSpawnPoints[LevelManager.Instance.currentLevel].position;
        }

        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        grounded = false;
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
        wallBounceTimer = 0f;
        footstepTimer = 0f;
        isDying = false;

        if (playerAnimator != null)
        {
            playerAnimator.ResetAnimation();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDying || collision == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Environment"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                float impactSpeed = Mathf.Abs(collision.relativeVelocity.x);

                if (impactSpeed >= minWallBounceSpeed)
                {
                    rb.linearVelocityX = contact.normal.x * impactSpeed * wallBounceMult;
                    wallBounceTimer = wallBounceControlLock;
                }
            }
        }
    }

    void StartTimer()
    {
        if (!levelManager.timerStarted && !levelManager.gameEnded)
        {
            levelManager.timerStarted = true;
        }
    }
}