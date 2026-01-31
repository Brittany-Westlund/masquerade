using UnityEngine;

public class LilypadWiggle : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1f;               // Distance to detect cursor
    public float pushStrength = 0.2f;       // How far it moves
    public float returnSpeed = 2f;          // How fast it returns to original position

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 toMouse = mouseWorld - transform.position;
        float distance = toMouse.magnitude;

        if (distance < radius)
        {
            // Push away from mouse
            Vector3 pushDir = -toMouse.normalized;
            transform.position = Vector3.Lerp(transform.position, originalPosition + pushDir * pushStrength, Time.deltaTime * 10f);
        }
        else
        {
            // Smoothly return to resting position
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);
        }
    }
}
