using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input Things")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference activateAction;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 70f;
    public float deceleration = 90f;

    [Header("Jump Fields")]
    public float jumpForce = 14f;
    public float gravityMultiplier = 2.5f;
    public float fallGravityMultiplier = 3.5f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("Ground Check Stuff")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    float moveInput;
    float coyoteTimer;
    float jumpBufferTimer;
    bool grounded;

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
    }

    void OnDisable()
    {
        jumpAction.action.performed -= OnJump;
        moveAction.action.Disable();
        jumpAction.action.Disable();
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
    }

    void OnJump(InputAction.CallbackContext context)
    {
        jumpBufferTimer = jumpBufferTime;
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float targetSpeed = moveInput * moveSpeed;
        float rate = Mathf.Abs(moveInput) > 0.01f ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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

            if (CameraManager.Instance.selectedCam != camBound.cameraObj)
            {
                CameraManager.Instance.DisableAllCameras();
                CameraManager.Instance.EnableCamera(camBound.cameraObj);

                if (camBound.playerPos != null)
                {
                    transform.position = camBound.playerPos.position;
                }
            }
            else if (camBound.secondaryCameraObj != null)
            {
                CameraManager.Instance.DisableAllCameras();
                CameraManager.Instance.EnableCamera(camBound.secondaryCameraObj);

                if (camBound.secondaryPlayerPos != null)
                {
                    transform.position = camBound.secondaryPlayerPos.position;
                }
            }
        }
    }
}