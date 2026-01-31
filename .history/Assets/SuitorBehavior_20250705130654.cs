using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;           // Positive = right, Negative = left
    public float moveDuration = 0.25f;        // Scoot duration
    public string playerTag = "Cursor";       // Tag for player character

    [Header("Petal Settings")]
    public string petalTag = "Petal";         // Tag for petal object
    public float petalRadius = 1.5f;          // How close the petal needs to be to reduce scoot chance

    private bool isMoving = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving || !other.CompareTag(playerTag))
            return;

        float scootChance = IsPetalNearby() ? 0.1f : 0.4f;
        float roll = Random.value;

        if (roll <= scootChance)
        {
            StartCoroutine(Scoot());
        }
    }

    bool IsPetalNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, petalRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(petalTag))
                return true;
        }
        return false;
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
