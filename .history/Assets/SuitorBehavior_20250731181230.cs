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
    public SpriteRenderer rootRenderer;            // Left-facing swim (on the root)
    public SpriteRenderer swimmingRightRenderer;   // Right-facing swimmer (child)
    public SpriteRenderer standingRenderer;
    public SpriteRenderer legupRenderer;
    public SpriteRenderer lookingRenderer;

    [Header("Animation (Optional)")]
    public Animator animator;

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

        // Determine direction
        bool movingRight = Random.value > 0.5f;
        float actualMove = movingRight ? Mathf.Abs(moveDistance) : -Mathf.Abs(moveDistance);

        // Enable only the appropriate swimming sprite
        SetAllRenderersDisabled();

        if (movingRight)
        {
            if (swimmingRightRenderer != null)
                swimmingRightRenderer.enabled = true;
        }
        else
        {
            if (rootRenderer != null)
                rootRenderer.enabled = true;
        }

        if (animator != null)
            animator.SetBool("IsSwimming", true);

        // Move only while in swimming state
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

        if (animator != null)
            animator.SetBool("IsSwimming", false);

        // Return to idle state (e.g. standing)
        SetAllRenderersDisabled();
        if (rootRenderer != null)
            rootRenderer.enabled = true;

        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
            returner.StartReturnTimer(actualMove);

        isMoving = false;
    }

    private void SetAllRenderersDisabled()
    {
        if (rootRenderer) rootRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (standingRenderer) standingRenderer.enabled = false;
        if (legupRenderer) legupRenderer.enabled = false;
        if (lookingRenderer) lookingRenderer.enabled = false;
    }
}
