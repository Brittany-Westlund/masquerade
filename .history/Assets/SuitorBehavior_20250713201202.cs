using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;           // Positive = right, Negative = left
    public float moveDuration = 0.25f;        // Scoot duration
    public string playerTag = "Cursor";       // Tag for player character

    [Header("Petal Settings")]
    public string petalTag = "Petal";         // Tag for petal object
    public float petalRadius = 1.5f;          // Proximity to affect behavior

    private bool isMoving = false;
    private bool hasFlipped = false;

    void Update()
    {
        // Continually check for petal proximity
        if (!hasFlipped && IsPetalNearby())
        {
            float flipChance = 0.6f;
            float roll = Random.value;

            if (roll <= flipChance)
            {
                Flip();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving || !other.CompareTag(playerTag))
            return;

        float scootChance = hasFlipped ? 0.01f : (IsPetalNearby() ? 0.1f : 0.4f);
        float roll = Random.value;

        if (roll <= scootChance)
        {
            StartCoroutine(Scoot());
        }

        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
        {
            returner.StartReturnTimer(moveDistance);
        }
    }

    void Flip()
    {
        hasFlipped = true;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
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
