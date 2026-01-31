using UnityEngine;

public class PetalNudge : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;             // How fast the petal glides
    public float returnSpeed = 1f;           // Speed returning to original spot
    public float threshold = 0.05f;          // Distance to consider arrival

    private Vector3 originalPosition;
    private Vector3? targetPosition = null;
    private bool returning = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (targetPosition.HasValue)
        {
            // Move toward the target
            transform.position = Vector3.Lerp(transform.position, targetPosition.Value, Time.deltaTime * moveSpeed);

            // Stop if close enough
            if (Vector3.Distance(transform.position, targetPosition.Value) < threshold)
            {
                transform.position = targetPosition.Value;
                targetPosition = null;
                returning = false;
            }
        }
        else if (returning)
        {
            // Return to original
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);

            if (Vector3.Distance(transform.position, originalPosition) < threshold)
            {
                transform.position = originalPosition;
                returning = false;
            }
        }
    }

    public void NudgeTo(Vector3 newPosition)
    {
        targetPosition = newPosition;
        returning = false;
    }

    public void ReturnToOriginal()
    {
        targetPosition = null;
        returning = true;
    }
}
