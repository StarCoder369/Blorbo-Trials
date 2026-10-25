using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject spawningObject;

    public float spawnCooldown;

    float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            Spawn();
            spawnTimer = spawnCooldown;
        }
    }

    public void Spawn()
    {
        Instantiate(spawningObject, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, Camera.main.transform);
    }
}