using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public SpriteRenderer spriteRenderer;

    [Header("Movement Settings")]
    public float runThreshold = 0.1f;
    public float fallThreshold = -0.1f;
    public bool flipSprite = true;
    public bool startFacingRight = true;

    [Header("Idle Animation")]
    public Sprite[] idleFrames;
    public float idleFPS = 6f;

    [Header("Run Animation")]
    public Sprite[] runFrames;
    public float runFPS = 12f;

    [Header("Jump Animation")]
    public Sprite[] jumpFrames;
    public float jumpFPS = 12f;

    [Header("Fall Animation")]
    public Sprite[] fallFrames;
    public float fallFPS = 10f;

    [Header("Land Animation")]
    public Sprite[] landFrames;
    public float landFPS = 12f;

    [Header("Death Animation")]
    public Sprite[] deathFrames;
    public float deathFPS = 12f;

    public bool HasDeathAnimation => deathFrames != null && deathFrames.Length > 0;
    public float DeathAnimationDuration => HasDeathAnimation ? deathFrames.Length / Mathf.Max(deathFPS, 0.01f) : 0f;

    enum AnimationState
    {
        None,
        Idle,
        Run,
        Jump,
        Fall,
        Land,
        Death
    }

    AnimationState currentState = AnimationState.None;

    Sprite[] currentFrames;
    float currentFPS;
    float frameTimer;
    int currentFrame;
    bool loopAnimation;
    bool wasGrounded;
    bool wasFalling;
    bool isDead;
    bool facingRight;

    void Awake()
    {
        if (player == null)
        {
            player = GetComponent<Player>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        facingRight = startFacingRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }

        if (player != null)
        {
            wasGrounded = player.IsGrounded;
        }
    }

    void Update()
    {
        if (player == null || spriteRenderer == null)
        {
            return;
        }

        if (isDead)
        {
            UpdateAnimation();
            return;
        }

        bool grounded = player.IsGrounded;
        float verticalVelocity = player.VerticalVelocity;

        if (flipSprite && Mathf.Abs(player.moveInput) > 0.01f)
        {
            facingRight = player.moveInput > 0f;
            spriteRenderer.flipX = !facingRight;
        }

        bool falling = !grounded && verticalVelocity < fallThreshold;
        bool landed = grounded && !wasGrounded && wasFalling;

        if (landed && HasFrames(landFrames))
        {
            PlayAnimation(AnimationState.Land, landFrames, landFPS, false);
        }
        else if (!grounded && verticalVelocity > 0f)
        {
            PlayAnimation(AnimationState.Jump, jumpFrames, jumpFPS, false);
        }
        else if (falling)
        {
            PlayAnimation(AnimationState.Fall, fallFrames, fallFPS, true);
        }
        else if (grounded && Mathf.Abs(player.moveInput) > 0.01f)
        {
            PlayAnimation(AnimationState.Run, runFrames, runFPS, true);
        }
        else if (grounded)
        {
            PlayAnimation(AnimationState.Idle, idleFrames, idleFPS, true);
        }

        wasFalling = falling;
        wasGrounded = grounded;

        UpdateAnimation();
    }

    bool HasFrames(Sprite[] frames)
    {
        return frames != null && frames.Length > 0;
    }

    void PlayAnimation(AnimationState state, Sprite[] frames, float fps, bool loop)
    {
        if (!HasFrames(frames))
        {
            return;
        }

        if (currentState == state)
        {
            return;
        }

        currentState = state;
        currentFrames = frames;
        currentFPS = Mathf.Max(fps, 0.01f);
        currentFrame = 0;
        frameTimer = 0f;
        loopAnimation = loop;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = currentFrames[0];
        }
    }

    void UpdateAnimation()
    {
        if (!HasFrames(currentFrames) || currentFPS <= 0f || spriteRenderer == null)
        {
            return;
        }

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / currentFPS;

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;

            if (currentFrame < currentFrames.Length - 1)
            {
                currentFrame++;
            }
            else if (loopAnimation)
            {
                currentFrame = 0;
            }
            else
            {
                frameTimer = 0f;
                return;
            }

            spriteRenderer.sprite = currentFrames[currentFrame];
        }
    }

    public void PlayDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (HasDeathAnimation)
        {
            PlayAnimation(AnimationState.Death, deathFrames, deathFPS, false);
        }
    }

    public void ResetAnimation()
    {
        isDead = false;
        currentState = AnimationState.None;
        currentFrames = null;
        currentFrame = 0;
        frameTimer = 0f;
        wasFalling = false;

        if (player != null)
        {
            wasGrounded = player.IsGrounded;
        }
    }
}