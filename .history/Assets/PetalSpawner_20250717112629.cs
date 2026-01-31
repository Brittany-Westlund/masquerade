using UnityEngine;
using System.Collections.Generic;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 3f;
    public int petalsPerWave = 4;
    public float spawnXRange = 8f;
    public float spawnYMin = 8f;
    public float spawnYMax = 12f;
    public float fallSpeed = 1f;
    public float petalLifetime = 12f;

    [Header("Drift Settings")]
    public float driftAmplitude = 0.5f;
    public float driftFrequency = 1f;

    [Header("Fall Duration Range")]
    public float fallTimeMin = 1f;
    public float fallTimeMax = 4f;

    private float timer = 0f;
    private List<PetalData> petals = new List<PetalData>();

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPetals();
            timer = 0f;
        }

        float now = Time.time;
        for (int i = petals.Count - 1; i >= 0; i--)
        {
            var data = petals[i];
            float elapsed = now - data.spawnTime;

            if (elapsed > petalLifetime)
            {
                Destroy(data.obj);
                petals.RemoveAt(i);
                continue;
            }

            Vector3 pos = data.obj.transform.position;

            // Fall for a random duration, then stop
            if (elapsed < data.fallDuration)
            {
                pos.y -= fallSpeed * Time.deltaTime;
            }

            // Gentle side-to-side drift
            float drift = Mathf.Sin(elapsed * driftFrequency) * driftAmplitude;
            pos.x = data.baseX + drift;

            data.obj.transform.position = pos;
        }
    }

    void SpawnPetals()
    {
        for (int i = 0; i < petalsPerWave; i++)
        {
            float spawnX = Random.Range(-spawnXRange, spawnXRange);
            float spawnY = Random.Range(spawnYMin, spawnYMax);
            float fallDuration = Random.Range(fallTimeMin, fallTimeMax);

            GameObject petal = Instantiate(petalPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);

            PetalData data = new PetalData
            {
                obj = petal,
                baseX = spawnX,
                spawnTime = Time.time,
                fallDuration = fallDuration
            };

            petals.Add(data);
        }
    }

    private class PetalData
    {
        public GameObject obj;
        public float baseX;
        public float spawnTime;
        public float fallDuration;
    }
}
