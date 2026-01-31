using UnityEngine;

public class PetalMovement : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1.5f;               // Cursor influence radius
    public float pushStrength = 1.5f;         // How strong the push is
    public float maxSpeed = 2f;               // Max speed of movement

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 toMouse = mouseWorld - transform.position;
        float distance = toMouse.magnitude;

        if (distance < radius)
        {
            // Push away from the cursor
            Vector3 pushDir = -toMouse.normalized;
            velocity += pushDir * pushStrength * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }

        // Apply velocity (with a little dampening to feel floaty)
        transform.position += velocity * Time.deltaTime;
        velocity *= 0.98f; // Dampening factor
    }
}
