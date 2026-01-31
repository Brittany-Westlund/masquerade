using UnityEngine;
using System.Collections;

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
    public SpriteRenderer rootRenderer;            // ← idle/swimming left-facing
    public SpriteRenderer swimmingRightRenderer;   // ← active right-facing swimming

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

        // 🎲 Flip a coin for direction
        bool movingRight = Random.value > 0.5f;

        // 🧼 Fully disable both visuals before enabling the correct one
        if (rootRenderer != null) rootRenderer.enabled = false;
        if (swimmingRightRenderer != null) swimmingRightRenderer.enabled = false;

        // 🎨 Enable the correct sprite
        if (movingRight)
        {
            if (swimmingRightRenderer != null) swimmingRightRenderer.enabled = true;
        }
        else
        {
            if (rootRenderer != null) rootRenderer.enabled = true;
        }

        // 🏊 Perform movement in that direction
        float signedMove = movingRight ? moveDistance : -moveDistance;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // 💤 Back to idle swimming (left-facing)
        if (swimmingRightRenderer != null) swimmingRightRenderer.enabled = false;
        if (rootRenderer != null) rootRenderer.enabled = true;

        // ⏳ Optional: start return logic
        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
        {
            returner.StartReturnTimer(signedMove);
        }

        isMoving = false;
    }
}
