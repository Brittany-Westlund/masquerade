using UnityEngine;
using System.Collections.Generic;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 4f;
    public int petalsPerWave = 1;
    public float spawnXRange = 8f;
    public float spawnYMin = 7f;
    public float spawnYMax = 12f;

    [Header("Float Settings")]
    public float fallSpeed = 0.5f;
    public float driftAmplitude = 0.5f;
    public float driftFrequency = 1f;
    public float petalLifetime = 10f;

    private float timer = 0f;
    private List<PetalData> activePetals = new List<PetalData>();

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPetals();
            timer = 0f;
        }

        UpdatePetals();
    }

    void SpawnPetals()
    {
        for (int i = 0; i < petalsPerWave; i++)
        {
            float randomX = Random.Range(-spawnXRange, spawnXRange);
            float randomY = Random.Range(spawnYMin, spawnYMax);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

            GameObject petal = Instantiate(petalPrefab, spawnPos, Quaternion.identity);
            PetalData data = new PetalData
            {
                obj = petal,
                startTime = Time.time,
                startPos = spawnPos
            };
            activePetals.Add(data);
        }
    }

    void UpdatePetals()
    {
        float currentTime = Time.time;
        for (int i = activePetals.Count - 1; i >= 0; i--)
        {
            PetalData data = activePetals[i];
            float elapsed = currentTime - data.startTime;

            if (elapsed >= petalLifetime)
            {
                Destroy(data.obj);
                activePetals.RemoveAt(i);
                continue;
            }

            float newY = data.startPos.y - fallSpeed * elapsed;
            float newX = data.startPos.x + Mathf.Sin(elapsed * driftFrequency) * driftAmplitude;
            data.obj.transform.position = new Vector3(newX, newY, 0f);
        }
    }

    private class PetalData
    {
        public GameObject obj;
        public float startTime;
        public Vector3 startPos;
    }
}
