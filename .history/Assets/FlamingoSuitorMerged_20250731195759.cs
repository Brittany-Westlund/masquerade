using UnityEngine;
using System.Collections;

public class FlamingoSuitorMerged : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer idleRenderer;             // Left-facing idle/swimming
    public SpriteRenderer swimmingRightRenderer;    // Right-facing swimming
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;

    [Header("Star Drop Settings")]
    public GameObject starObject;
    public float starFallAmount = 5f;

    [Header("Scoot Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.25f;
    public float returnDelay = 5f;

    [Header("Petal Detection")]
    public LayerMask petalLayer;
    public float petalCheckRadius = 1.5f;

    private Vector3 originalPosition;
    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;
    private bool isMoving = false;

    void Start()
    {
        originalPosition = transform.position;
        SetToIdleOnly();
    }

    void Update()
    {
        if (cursorIsNearby && !isMoving)
        {
            float scootChance = IsPetalNearby() ? 0.05f : 0.25f;
            if (Random.value < scootChance)
            {
                TryToScootAway();
            }
        }

        if (cursorIsNearby && !legLifted && !isSleeping && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RespondWithLegLift());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;
        cursorIsNearby = true;

        if (!legLifted && !isSleeping)
            SetToLookingOnly();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;
        cursorIsNearby = false;

        if (!legLifted && !isSleeping)
            SetToIdleOnly();
    }

    IEnumerator RespondWithLegLift()
    {
        legLifted = true;

        yield return new WaitForSeconds(Random.Range(1f, 2f));
        SetToLegLiftOnly();

        yield return new WaitForSeconds(10f);

        if (Random.value <= 0.8f)
        {
            StartCoroutine(SwitchToSleeping());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            TryToScootAway();
        }
    }

    IEnumerator SwitchToSleeping()
    {
        isSleeping = true;
        SetToSleepingOnly();

        if (starObject != null)
            StartCoroutine(FallStar());

        yield return new WaitForSeconds(Random.Range(60f, 180f));

        isSleeping = false;
        SetToIdleOnly();
        TryToScootAway();
    }

    IEnumerator FallStar()
    {
        Vector3 start = starObject.transform.position;
        Vector3 end = start - new Vector3(0f, starFallAmount, 0f);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            starObject.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        starObject.transform.position = end;
    }

    public void TryToScootAway()
    {
        float dir = Random.value > 0.5f ? 1f : -1f;
        StartCoroutine(ScootAndReturn(dir));
    }

    IEnumerator ScootAndReturn(float direction)
    {
        if (isMoving) yield break;
        isMoving = true;

        DisableAllSuitorRenderers();
        if (direction > 0)
            swimmingRightRenderer.enabled = true;
        else
            idleRenderer.enabled = true;

        Vector3 start = transform.position;
        Vector3 scootTarget = start + new Vector3(Mathf.Sign(direction) * Mathf.Abs(moveDistance), 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, scootTarget, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = scootTarget;
        yield return new WaitForSeconds(returnDelay);

        DisableAllSuitorRenderers();
        if (direction < 0)
            swimmingRightRenderer.enabled = true; // Returning right
        else
            idleRenderer.enabled = true;          // Returning left

        elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        DisableAllSuitorRenderers();
        idleRenderer.enabled = true;
        isMoving = false;
    }

    void DisableAllSuitorRenderers()
    {
        idleRenderer.enabled = false;
        swimmingRightRenderer.enabled = false;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = false;
    }

    void SetToIdleOnly()
    {
        DisableAllSuitorRenderers();
        idleRenderer.enabled = true;
        legLifted = false;
    }

    void SetToLookingOnly()
    {
        DisableAllSuitorRenderers();
        lookingRenderer.enabled = true;
    }

    void SetToLegLiftOnly()
    {
        DisableAllSuitorRenderers();
        legUpRenderer.enabled = true;
    }

    void SetToSleepingOnly()
    {
        DisableAllSuitorRenderers();
        sleepingRenderer.enabled = true;
    }

    bool IsPetalNearby()
    {
        return Physics2D.OverlapCircle(transform.position, petalCheckRadius, petalLayer);
    }
}
