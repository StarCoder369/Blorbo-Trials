using UnityEngine;

public class BackgroundGuy : MonoBehaviour
{
    public Sprite[] sprites;
    public float speed = 1f;
    public float rotationSpeed = 20f;

    public float wobbleAmount = 0.5f;
    public float wobbleSpeed = 1f;

    public float minSize = 0.75f;
    public float maxSize = 1.25f;

    public float minOpacity = 0.1f;
    public float maxOpacity = 0.5f;
    public float fadeInDuration = 1f;

    float startX;
    float wobbleOffset;
    float size;
    float opacity;
    float fadeTimer;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        // Behold the wall of text!
        opacity = Random.Range(minOpacity, maxOpacity);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        Color tempColor = spriteRenderer.color;
        tempColor.a = 0f;
        spriteRenderer.color = tempColor;
        size = Random.Range(minSize, maxSize);
        startX = transform.localPosition.x;
        wobbleOffset = Random.Range(0f, 100f);
        transform.localScale = new Vector2(size, size);
        Destroy(gameObject, 15f);
    }

    void Update()
    {
        transform.localPosition += Vector3.down * speed * Time.deltaTime;

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 5f);

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        float wobble = Mathf.Sin(Time.time * wobbleSpeed + wobbleOffset) * wobbleAmount;
        transform.localPosition = new Vector3(startX + wobble, transform.localPosition.y, 5f);

        if (fadeTimer < fadeInDuration && fadeInDuration > 0f)
        {
            fadeTimer += Time.deltaTime;
            Color tempColor = spriteRenderer.color;
            tempColor.a = Mathf.Lerp(0f, opacity, Mathf.Clamp01(fadeTimer / fadeInDuration));
            spriteRenderer.color = tempColor;
        }
    }
}