using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;         // Positive = right, Negative = left
    public float moveDuration = 0.25f;      // Time it takes to scoot
    public string playerTag = "Cursor";     // The tag of your PC

    private bool isMoving = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving || !other.CompareTag(playerTag))
            return;

        float chance = Random.value; // 0 to 1

        if (chance <= 0.4f)
        {
            StartCoroutine(Scoot());
        }
    }

    System.Collections.IEnumerator Scoot()
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(moveDistance, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
