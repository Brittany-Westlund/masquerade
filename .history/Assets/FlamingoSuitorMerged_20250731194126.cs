using UnityEngine;
using System.Collections;

public class FlamingoSuitorMerged : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer idleRenderer;
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;
    public SpriteRenderer swimmingRightRenderer;

    [Header("Movement")]
    public float moveDistance = 1f;
    public float moveDuration = 0.25f;

    [Header("Star Drop Settings")]
    public GameObject starObject;
    public float starFallAmount = 5f;

    [Header("Petal Settings")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;
    private bool isMoving = false;

    void Start()
    {
        SetToIdleOnly();
    }

    void Update()
    {
        if (cursorIsNearby && !isMoving && Input.GetMouseButtonDown(0))
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

        TryScootFromCollision();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;

        cursorIsNearby = false;

        if (!legLifted && !isSleeping)
            SetToIdleOnly();
    }

    void TryScootFromCollision()
    {
        float scootChance = IsPetalNearby() ? 0.05f : 0.25f;
        if (Random.value <= scootChance)
        {
            float direction = Random.value > 0.5f ? 1f : -1f;
            StartCoroutine(Scoot(direction));
        }
    }

    IEnumerator RespondWithLegLift()
    {
        legLifted = true;

        float delay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(delay);

        SetToLegLiftOnly();

        yield return new WaitForSeconds(10f);

        if (Random.value <= 0.8f)
        {
            StartCoroutine(SwitchToSleeping());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            TryScootAfterLeg();
        }
    }

    void TryScootAfterLeg()
    {
        float direction = Random.value > 0.5f ? 1f : -1f;
        StartCoroutine(Scoot(direction));
    }

    IEnumerator SwitchToSleeping()
    {
        isSleeping = true;
        SetToSleepingOnly();

        if (starObject != null)
            StartCoroutine(FallStar());

        float sleepDuration = Random.Range(60f, 180f);
        yield return new WaitForSeconds(sleepDuration);

        isSleeping = false;
        SetToIdleOnly();
        TryScootAfterLeg();
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

    IEnumerator Scoot(float direction)
    {
        if (isMoving) yield break;
        isMoving = true;

        DisableAllSuitorRenderers();

        if (direction > 0)
            swimmingRightRenderer.enabled = true;
        else
            idleRenderer.enabled = true;

        float signedMove = Mathf.Sign(direction) * Mathf.Abs(moveDistance);
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        DisableAllSuitorRenderers();
        idleRenderer.enabled = true;

        isMoving = false;
    }

    void DisableAllSuitorRenderers()
    {
        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = false;
        if (swimmingRightRenderer != null) swimmingRightRenderer.enabled = false;
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, petalRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(petalTag))
                return true;
        }
        return false;
    }
}
