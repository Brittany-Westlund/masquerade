using UnityEngine;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 2f;
    public int petalsPerWave = 3;
    public float spawnRadius = 5f;

    [Header("Spawn Area")]
    public float topY = 6f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPetals();
            timer = 0f;
        }
    }

    void SpawnPetals()
    {
        for (int i = 0; i < petalsPerWave; i++)
        {
            float x = Random.Range(-spawnRadius, spawnRadius);
            Vector3 spawnPos = new Vector3(x, topY, 0f);
            Instantiate(petalPrefab, spawnPos, Quaternion.identity);
        }
    }
}
