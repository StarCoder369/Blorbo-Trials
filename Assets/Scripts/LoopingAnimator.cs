using UnityEngine;
using UnityEngine.UI;

public class LoopingAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;
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

        if (spriteRenderer == null && image == null)
        {
            image = GetComponent<Image>();
        }
    }

    void Update()
    {
        if ((spriteRenderer == null && image == null) || frames == null || frames.Length == 0 || fps <= 0f) //Giant if statement
        {
            return;
        }

        frameTimer += Time.unscaledDeltaTime;
        float frameDuration = 1f / fps;

        while (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            currentFrame = (currentFrame + 1) % frames.Length; // Yay! Mod! Math! So Fun! /s

            if (frames[currentFrame] != null)
            {
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = frames[currentFrame];
                }
                else
                {
                    image.sprite = frames[currentFrame];
                }
            }
        }
    }
}