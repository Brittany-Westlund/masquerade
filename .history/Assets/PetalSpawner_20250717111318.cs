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
    public float stopYMin = -2f;
    public float stopYMax = 4.5f;
    public float fallSpeed = 1f;
    public float driftAmplitude = 0.5f;
    public float driftFrequency = 1f;
    public float petalLifetime = 15f;

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
            float elapsed = now - data.startTime;

            if (elapsed > petalLifetime)
            {
                Destroy(data.obj);
                petals.RemoveAt(i);
                continue;
            }

            Vector3 pos = data.obj.transform.position;

            // Move toward stopY and clamp when reached
            if (pos.y > data.stopY)
            {
                pos.y -= fallSpeed * Time.deltaTime;
                if (pos.y < data.stopY)
                    pos.y = data.stopY;
            }

            // Side-to-side drift
            float drift = Mathf.Sin(elapsed * driftFrequency) * driftAmplitude;
            pos.x = data.baseX + drift;

            data.obj.transform.position = pos;
        }
    }

    void SpawnPetals()
    {
        for (int i = 0; i < petalsPerWave; i++)
        {
            float x = Random.Range(-spawnXRange, spawnXRange);
            float y = Random.Range(spawnYMin, spawnYMax);
            float stopY = Random.Range(stopYMin, stopYMax);

            GameObject petal = Instantiate(petalPrefab, new Vector3(x, y, 0f), Quaternion.identity);
            petals.Add(new PetalData
            {
                obj = petal,
                baseX = x,
                stopY = stopY,
                startTime = Time.time
            });
        }
    }

    class PetalData
    {
        public GameObject obj;
        public float baseX;
        public float stopY;
        public float startTime;
    }
}
