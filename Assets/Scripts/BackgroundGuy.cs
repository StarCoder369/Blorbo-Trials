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

    float startX;
    float wobbleOffset;
    float size;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        size = Random.Range(minSize, maxSize);
        startX = transform.position.x;
        wobbleOffset = Random.Range(0f, 100f);
        transform.localScale = new Vector2(size, size);
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        float wobble = Mathf.Sin(Time.time * wobbleSpeed + wobbleOffset) * wobbleAmount; // AHHHH!!! TRIG!!!!
        transform.position = new Vector3(startX + wobble, transform.position.y, transform.position.z);
    }
}