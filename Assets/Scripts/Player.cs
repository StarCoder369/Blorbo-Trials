
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Input Things")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference activateAction;

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

    [Header("Other Things")]
    public float levelSwitchVelocityMult = 0.5f;
    public float wallBounceMult = 0.5f;
    public float minWallBounceSpeed = 5f;
    public float wallBounceControlLock = 0.12f;
    public GameObject explodeParticle;

    Rigidbody2D rb;
    float moveInput;
    float coyoteTimer;
    float jumpBufferTimer;
    float activateTimer;
    float wallBounceTimer;
    bool grounded;

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        activateAction.action.Enable();
        jumpAction.action.performed += OnJump;
        activateAction.action.performed += OnActivate;
    }

    void OnDisable()
    {
        jumpAction.action.performed -= OnJump;
        activateAction.action.performed -= OnActivate;
        moveAction.action.Disable();
        jumpAction.action.Disable();
        activateAction.action.Disable();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>().x;

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
    }

    void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    void OnActivate(InputAction.CallbackContext context)
    {
        if (activateTimer <= 0f)
        {
            float direction = rb.linearVelocity.x == 0f ? (moveInput == 0f ? 1f : Mathf.Sign(moveInput)) : -Mathf.Sign(rb.linearVelocity.x);
            float speed = Mathf.Abs(rb.linearVelocity.x) * activateSpeedMult;
            rb.linearVelocityX = direction * Mathf.Min(speed, maxHorizontalSpeed);
            activateTimer = activateCooldown;
        }
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

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
        if (collision.CompareTag("CamBound"))
        {
            CameraBound camBound = collision.GetComponent<CameraBound>();

            if (!camBound.boundsEnabled)
            {
                return;
            }

            if (LevelManager.Instance.selectedCam != LevelManager.Instance.cameras[LevelManager.Instance.currentLevel + 1])
            {
                LevelManager.Instance.DisableAllCameras();
                LevelManager.Instance.currentLevel++;
                LevelManager.Instance.EnableCamera(LevelManager.Instance.cameras[LevelManager.Instance.currentLevel]);
                transform.position = LevelManager.Instance.playerSpawnPoints[LevelManager.Instance.currentLevel].position;
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
        GameObject instantiatedExplosion = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        Destroy(instantiatedExplosion, 2f);
        transform.position = LevelManager.Instance.playerSpawnPoints[LevelManager.Instance.currentLevel].position;
        rb.linearVelocityX = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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
}
