using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Petal : MonoBehaviour
{
    [Header("Flutter Settings")]
    public float flutterSpeed = 1.5f;
    public float flutterFrequency = 2f;
    public float flutterAmplitude = 0.5f;
    public float fallSpeed = 1.5f;

    [Header("Audio")]
    public AudioClip flutterSound;
    public AudioClip waterHitSound;

    private AudioSource audioSource;
    private float startX;
    private float flutterTime;

    void Start()
    {
        startX = transform.position.x;
        audioSource = GetComponent<AudioSource>();
        if (flutterSound)
        {
            audioSource.clip = flutterSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        flutterTime += Time.deltaTime;
        float xOffset = Mathf.Sin(flutterTime * flutterFrequency) * flutterAmplitude;

        transform.position += new Vector3(xOffset, -fallSpeed, 0f) * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (flutterSound) audioSource.Stop();
            if (waterHitSound) audioSource.PlayOneShot(waterHitSound);

            // Optional: destroy after splash sound
            Destroy(gameObject, waterHitSound ? waterHitSound.length : 0.5f);
        }
    }
}
