using UnityEngine;
using System.Collections.Generic;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 4f;
    public int petalsPerWave = 3;
    public float spawnXRange = 8f;
    public float spawnYMin = 7f;
    public float spawnYMax = 12f;
    public float stopYMin = -4f; // change to match your full screen height
    public float stopYMax = 4.5f;
    public float fallSpeed = 0.75f;
    public float petalLifetime = 10f;

    [Header("Drift Settings")]
    public float driftAmplitude = 0.5f;
    public float driftFrequency = 1f;

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

            // Get current position
            Vector3 pos = data.obj.transform.position;

            // Fall toward stopY
            if (pos.y > data.stopY)
            {
                pos.y -= fallSpeed * Time.deltaTime;
                if (pos.y < data.stopY) pos.y = data.stopY;
            }

            // Side-to-side drift
            float driftX = Mathf.Sin(elapsed * driftFrequency) * driftAmplitude;
            pos.x = data.startPos.x + driftX;

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
