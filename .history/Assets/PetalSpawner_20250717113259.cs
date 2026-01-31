using UnityEngine;
using System.Collections.Generic;

public class PetalSpawner : MonoBehaviour
{
    [Header("Petal Settings")]
    public GameObject petalPrefab;
    public float spawnInterval = 3f;
    public int petalsPerWave = 3;
    public float spawnXRange = 8f;
    public float spawnYMin = 8f;
    public float spawnYMax = 12f;
    public float fallSpeed = 1.5f;
    public float petalLifetime = 12f;

    [Header("Flutter Settings")]
    public float flutterFrequency = 2f;
    public float flutterAmplitude = 0.5f;

    [Header("Fall Duration Range")]
    public float fallTimeMin = 1f;
    public float fallTimeMax = 4f;

    [Header("Audio")]
    public AudioClip flutterSound;
    public AudioClip waterHitSound;

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
            PetalData data = petals[i];
            float elapsed = now - data.spawnTime;

            if (elapsed > petalLifetime)
            {
                Destroy(data.obj);
                petals.RemoveAt(i);
                continue;
            }

            if (data.hasLanded) continue;

            // Fall and flutter for a limited time
            if (elapsed < data.fallDuration)
            {
                Vector3 pos = data.obj.transform.position;
                pos.y -= fallSpeed * Time.deltaTime;

                float flutter = Mathf.Sin(elapsed * flutterFrequency) * flutterAmplitude;
                pos.x = data.baseX + flutter;

                data.obj.transform.position = pos;
            }
            else
            {
                data.hasLanded = true;

                if (data.audioSource && flutterSound) data.audioSource.Stop();
                if (data.audioSource && waterHitSound) data.audioSource.PlayOneShot(waterHitSound);
            }
        }
    }

    void SpawnPetals()
    {
        for (int i = 0; i < petalsPerWave; i++)
        {
            float x = Random.Range(-spawnXRange, spawnXRange);
            float y = Random.Range(spawnYMin, spawnYMax);
            float fallDuration = Random.Range(fallTimeMin, fallTimeMax);

            GameObject petal = Instantiate(petalPrefab, new Vector3(x, y, 0f), Quaternion.identity);

            AudioSource audioSource = petal.GetComponent<AudioSource>();
            if (audioSource && flutterSound)
            {
                audioSource.clip = flutterSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            petals.Add(new PetalData
            {
                obj = petal,
                baseX = x,
                spawnTime = Time.time,
                fallDuration = fallDuration,
                audioSource = audioSource,
                hasLanded = false
            });
        }
    }

    private class PetalData
    {
        public GameObject obj;
        public float baseX;
        public float spawnTime;
        public float fallDuration;
        public AudioSource audioSource;
        public bool hasLanded;
    }
}
