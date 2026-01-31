using UnityEngine;

public class PetalMovement : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1.5f;               // Distance within which the cursor affects the petal
    public float pushStrength = 1.5f;         // How strong the push force is
    public float maxDistance = 3f;            // Max distance from original position
    public float returnSpeed = 1.5f;          // Speed when returning to origin

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

        Vector3 currentOffset = transform.position - originalPosition;

        if (distance < radius)
        {
            // Calculate push direction away from cursor
            Vector3 pushDir = -toMouse.normalized;

            // Apply push while staying within max distance
            Vector3 targetOffset = currentOffset + pushDir * pushStrength * Time.deltaTime;
            if (targetOffset.magnitude > maxDistance)
                targetOffset = targetOffset.normalized * maxDistance;

            transform.position = originalPosition + targetOffset;
        }
        else
        {
            // Smoothly return to original position
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);
        }
    }
}
