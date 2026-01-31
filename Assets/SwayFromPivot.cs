using UnityEngine;

public class SwayFromPivot : MonoBehaviour
{
    public float swayAngle = 15f;      // Max angle in degrees
    public float swaySpeed = 1f;       // Speed of sway
    public float offset = 0f;          // Phase offset if you want to stagger sways

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * swaySpeed + offset) * swayAngle;
        transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
    }
}
