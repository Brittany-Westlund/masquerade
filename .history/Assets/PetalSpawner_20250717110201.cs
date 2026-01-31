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
    public float driftAmplitude = 0.5f;
    public float driftFrequency = 1f;
    public float petalLifetime = 10f;
    public float stopYMin = 0f;   // Petals will stop at a random height within this range
    public float stopYMax = 6f;
    public float fallSpeed = 1f;

    private float timer = 0f;
    private List<PetalData> activePetals = new List<PetalData>();

    void Update()
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
            float stopY = Random.Range(stopYMin, stopYMax);

            Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

            GameObject petal = Instantiate(petalPrefab, spawnPos, Quaternion.identity);
            PetalData data = new PetalData
            {
                obj = petal,
                startTime = Time.time,
                startPos = spawnPos,
                stopY = stopY
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

            Vector3 pos = data.obj.transform.position;

            // Only move down if above stopY
            if (pos.y > data.stopY)
            {
                pos.y -= fallSpeed * Time.deltaTime;
                pos.y = Mathf.Max(pos.y, data.stopY); // Clamp to stopY
            }

            // Apply side-to-side drift
            float drift = Mathf.Sin(elapsed * driftFrequency) * driftAmplitude;
            pos.x = data.startPos.x + drift;

            data.obj.transform.position = pos;
        }
    }

    private class PetalData
    {
        public GameObject obj;
        public float startTime;
        public Vector3 startPos;
        public float stopY;
    }
}
