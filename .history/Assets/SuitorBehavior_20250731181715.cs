using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;              // Always positive
    public float moveDuration = 0.25f;
    public string playerTag = "Cursor";

    [Header("Petal Settings")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Sprite Renderers")]
    public SpriteRenderer rootRenderer;            // ← idle swimming sprite (left-facing)
    public SpriteRenderer swimmingRightRenderer;   // ← right-facing swimming sprite (child)
    public SpriteRenderer standingRenderer;
    public SpriteRenderer legupRenderer;
    public SpriteRenderer lookingRenderer;

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

    public System.Collections.IEnumerator Scoot()
    {
        isMoving = true;

        // 🧭 Determine direction
        bool movingRight = Random.value > 0.5f;
        float actualMove = movingRight ? Mathf.Abs(moveDistance) : -Mathf.Abs(moveDistance);

        // 🧼 Disable both swimming renderers
        if (rootRenderer) rootRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;

        // 🎯 Enable the correct one for the scoot
        if (movingRight && swimmingRightRenderer != null)
        {
            swimmingRightRenderer.enabled = true;
        }
        else if (!movingRight && rootRenderer != null)
        {
            rootRenderer.enabled = true;
        }

        // 🏊 Movement
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(actualMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // 💤 Return to idle swimming pose (left-facing)
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (rootRenderer) rootRenderer.enabled = true;

        // ⏳ Optional return timer
        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
        {
            returner.StartReturnTimer(actualMove);
        }

        isMoving = false;
    }
}
