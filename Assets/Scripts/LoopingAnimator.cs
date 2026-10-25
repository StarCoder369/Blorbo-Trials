using UnityEngine;

public class LoopingAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] frames;
    public float fps = 12f;

    float frameTimer;
    int currentFrame;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (spriteRenderer == null || frames == null || frames.Length == 0 || fps <= 0f)
        {
            return;
        }

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / fps;

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            currentFrame = (currentFrame + 1) % frames.Length; //Yay! Mod! Math! So Fun!                 /s

            if (frames[currentFrame] != null)
            {
                spriteRenderer.sprite = frames[currentFrame];
            }
        }
    }
}