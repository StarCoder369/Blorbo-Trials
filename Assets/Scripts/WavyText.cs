using UnityEngine;
using TMPro;

// Used a video to figure out how to do this. Then did my own stuff to it.
public class WavyText : MonoBehaviour
{
    public TMP_Text text;
    public float speed = 4f;
    public float height = 10f;
    public float waveOffset = 0.3f;
    public float timeSteps = 12f;
    public float movementSteps = 5f;

    Vector3[][] originalVertices;
    string previousText;

    void Start()
    {
        SaveOriginalVertices();
    }

    void SaveOriginalVertices()
    {
        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo.Length][];

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        previousText = text.text;
    }

    void Update()
    {
        if (text.text != previousText)
        {
            SaveOriginalVertices();
        }

        text.ForceMeshUpdate();

        TMP_TextInfo textInfo = text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];

            if (!character.isVisible)
            {
                continue;
            }

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Vector3[] original = originalVertices[materialIndex];

            // Math. Uses floor to make it like jittery.
            float steppedTime = Mathf.Floor(Time.unscaledTime * timeSteps) / timeSteps;
            float offsetY = Mathf.Sin(steppedTime * speed - i * waveOffset) * height;

            if (movementSteps > 0f)
            {
                offsetY = Mathf.Round(offsetY / movementSteps) * movementSteps;
            }

            Vector3 offset = new Vector3(0f, offsetY, 0f);

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] = original[vertexIndex + j] + offset;
            }
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}