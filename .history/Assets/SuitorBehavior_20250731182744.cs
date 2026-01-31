using UnityEngine;
using System.Collections;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;              // Always positive in Inspector
    public float moveDuration = 0.25f;
    public string playerTag = "Cursor";

    [Header("Petal Settings")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Sprite Renderers")]
    public SpriteRenderer rootRenderer;            // ← left-facing idle & swimming
    public SpriteRenderer swimmingRightRenderer;   // ← right-facing swimming

    private bool isMoving = false;
    private bool hasFlipped = false;

    void Update()
    {
        if (!hasFlipped && IsPetalNearby())
        {
            float flipChance = 0.6f;
            if (Random.value <= flipChance)
            {
                hasFlipped = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving || !other.CompareTag(playerTag)) return;

        float scootChance = hasFlipped ? 0.01f : (IsPetalNearby() ? 0.1f : 0.4f);
        if (Random.value <= scootChance)
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

    public IEnumerator Scoot()
    {
        isMoving = true;

        // Decide direction
        bool movingRight = Random.value > 0.5f;
        float signedMove = movingRight ? moveDistance : -moveDistance;

        // 🔄 Set visuals based on direction
        if (movingRight)
        {
            if (rootRenderer) rootRenderer.enabled = false;
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = true;
        }
        else
        {
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
            if (rootRenderer) rootRenderer.enabled = true;
        }

        // 🏃 Move in the decided direction
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // After scoot, return to idle state = left-facing swimming
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (rootRenderer) rootRenderer.enabled = true;

        // Optional return timer
        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
        {
            returner.StartReturnTimer(signedMove);
        }

        isMoving = false;
    }
}
