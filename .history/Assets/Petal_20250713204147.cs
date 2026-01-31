using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Petal : MonoBehaviour
{
    [Header("Flutter Settings")]
    public float flutterSpeed = 1.5f;
    public float flutterFrequency = 2f;
    public float flutterAmplitude = 0.5f;
    public float fallSpeed = 1.5f;

    [Header("Water Contact Settings")]
    public float waterYThreshold = -3.5f; // adjust based on your camera

    [Header("Audio")]
    public AudioClip flutterSound;
    public AudioClip waterHitSound;

    private AudioSource audioSource;
    private float flutterTime;
    private bool hasLanded = false;

    void Start()
    {
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
        if (hasLanded) return;

        // Flutter down motion
        flutterTime += Time.deltaTime;
        float xOffset = Mathf.Sin(flutterTime * flutterFrequency) * flutterAmplitude;
        transform.position += new Vector3(xOffset, -fallSpeed, 0f) * Time.deltaTime;

        // Check for water landing
        if (transform.position.y <= waterYThreshold)
        {
            LandOnWater();
        }
    }

    void LandOnWater()
    {
        hasLanded = true;

        if (flutterSound) audioSource.Stop();
        if (waterHitSound) audioSource.PlayOneShot(waterHitSound);

        // Optional: fade out or destroy
        Destroy(gameObject, waterHitSound ? waterHitSound.length : 0.5f);
    }
}
