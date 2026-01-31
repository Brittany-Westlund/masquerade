using UnityEngine;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 4f;         // Time between petal waves
    public int petalsPerWave = 1;            // Number of petals to spawn each time
    public float spawnXRange = 8f;           // How far left/right from center they can spawn
    public float spawnYMin = 5f;             // Lowest Y value (e.g., just above camera)
    public float spawnYMax = 8f;             // Highest Y value (optional variation)

    private float timer = 0f;

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
            float randomX = Random.Range(-spawnXRange, spawnXRange);
            float randomY = Random.Range(spawnYMin, spawnYMax);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0f);
            Instantiate(petalPrefab, spawnPos, Quaternion.identity);
        }
    }
}
