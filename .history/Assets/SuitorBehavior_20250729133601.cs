using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;              // Positive value only, direction is decided in code
    public float moveDuration = 0.25f;
    public string playerTag = "Cursor";

    [Header("Petal Settings")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Root Visual")]
    public SpriteRenderer rootRenderer;          // The renderer on the parent GameObject (left-facing flamingo)

    [Header("Child Visuals (inside this object)")]
    public GameObject standingVisual;
    public GameObject legupVisual;
    public GameObject lookingVisual;
    public GameObject swimmingRightVisual;       // Right-facing swimming child object

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

        // 🎯 Decide movement direction randomly
        bool movingRight = Random.value > 0.5f;
        float actualMove = movingRight ? Mathf.Abs(moveDistance) : -Mathf.Abs(moveDistance);

        // 🎨 Hide idle visuals
        SetIdleVisualsActive(false);

        // 🔁 Show swimming visual for correct direction
        if (rootRenderer != null)
            rootRenderer.enabled = !movingRight; // Disable root visual when moving right

        if (swimmingRightVisual != null)
            swimmingRightVisual.SetActive(movingRight);

        // 🏊 Trigger animation
        if (animator != null)
            animator.SetBool("IsSwimming", true);

        // 🚶 Move
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

        // 🧘 Restore state
        if (animator != null)
            animator.SetBool("IsSwimming", false);

        if (swimmingRightVisual != null)
            swimmingRightVisual.SetActive(false);

        SetIdleVisualsActive(true);

        if (rootRenderer != null)
            rootRenderer.enabled = true;

        // 🕰️ Start return timer
        SuitorReturnBehavior returner = GetComponent<SuitorReturnBehavior>();
        if (returner != null)
            returner.StartReturnTimer(actualMove);

        isMoving = false;
    }

    private void SetIdleVisualsActive(bool isActive)
    {
        if (standingVisual) standingVisual.SetActive(isActive);
        if (legupVisual) legupVisual.SetActive(isActive);
        if (lookingVisual) lookingVisual.SetActive(isActive);

        // Toggle rootRenderer based on child visual state
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
