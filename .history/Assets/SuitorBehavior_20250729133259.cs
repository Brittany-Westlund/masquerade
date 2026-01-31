using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.25f;
    public string playerTag = "Cursor";

    [Header("Petal Settings")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Root Visual")]
    public SpriteRenderer rootRenderer; // This is the left-facing idle/swim visual on the parent object

    [Header("Child Visuals")]
    public GameObject standingVisual;
    public GameObject legupVisual;
    public GameObject lookingVisual;
    public GameObject swimmingRightVisual;

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

        bool movingRight = moveDistance > 0;

        // Disable root visual if any child will be active
        if (rootRenderer != null)
            rootRenderer.enabled = false;

        SetIdleVisualsActive(false);

        if (movingRight && swimmingRightVisual != null)
        {
            swimmingRightVisual.SetActive(true);
        }

        if (animator != null)
            animator.SetBool("IsSwimming", true);

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

        if (animator != null)
            animator.SetBool("IsSwimming", false);

        if (swimmingRightVisual != null)
            swimmingRightVisual.SetActive(false);

        // Reactivate idle visuals and root renderer
        SetIdleVisualsActive(true);

        if (rootRenderer != null)
            rootRenderer.enabled = true;

        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
        {
            returner.StartReturnTimer(moveDistance);
        }

        isMoving = false;
    }

    private void SetIdleVisualsActive(bool isActive)
    {
        if (standingVisual) standingVisual.SetActive(isActive);
        if (legupVisual) legupVisual.SetActive(isActive);
        if (lookingVisual) lookingVisual.SetActive(isActive);

        // If *any* child visuals are active, disable rootRenderer
        if (rootRenderer != null)
        {
            bool anyChildActive =
                (standingVisual && standingVisual.activeSelf) ||
                (legupVisual && legupVisual.activeSelf) ||
                (lookingVisual && lookingVisual.activeSelf) ||
                (swimmingRightVisual && swimmingRightVisual.activeSelf);

            rootRenderer.enabled = !anyChildActive;
        }
    }
}
