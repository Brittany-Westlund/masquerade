using UnityEngine;

public class ReedSway : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1f;                // How close the cursor needs to be
    public float maxAngle = 15f;             // Max rotation angle in degrees
    public float swaySpeed = 5f;             // How quickly it responds
    public float returnSpeed = 2f;           // How quickly it settles

    private Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 toMouse = mouseWorld - transform.position;
        float distance = toMouse.magnitude;

        if (distance < radius)
        {
            float angle = Mathf.Clamp(-toMouse.x, -1f, 1f) * maxAngle;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * swaySpeed);
        }
        else
        {
            // Return to upright
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * returnSpeed);
        }
    }
}
