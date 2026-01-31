using UnityEngine;
using System.Collections;

public class FlamingoSuitorMerged : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer idleRenderer;               // Left-facing swimming (default)
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;
    public SpriteRenderer swimmingRightRenderer;      // Right-facing swimming

    [Header("Movement")]
    public float scootDistance = 1f;
    public float scootDuration = 0.25f;
    public float returnDelay = 6f;
    public float returnDuration = 0.25f;

    [Header("Cursor & Petal Settings")]
    public string cursorTag = "Cursor";
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Star Drop Settings")]
    public GameObject starObject;
    public float starFallAmount = 5f;

    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;
    private bool isMoving = false;

    private Vector3 startPos;
    private Coroutine returnCoroutine;

    void Start()
    {
        SetToIdleOnly();
        startPos = transform.position;
    }

    void Update()
    {
        if (cursorIsNearby && !isMoving && !isSleeping && !legLifted)
        {
            TryScootFromIdle();

            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(RespondWithLegLift());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(cursorTag))
        {
            cursorIsNearby = true;
            if (!legLifted && !isSleeping)
                SetToLookingOnly();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(cursorTag))
        {
            cursorIsNearby = false;
            if (!legLifted && !isSleeping)
                SetToIdleOnly();
        }
    }

    void TryScootFromIdle()
    {
        float chance = IsPetalNearby() ? 0.05f : 0.25f;
        if (Random.value < chance)
        {
            float dir = Random.value > 0.5f ? 1f : -1f;
            StartCoroutine(Scoot(dir));
        }
    }

    bool IsPetalNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, petalRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(petalTag)) return true;
        }
        return false;
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
            float dir = Random.value > 0.5f ? 1f : -1f;
            StartCoroutine(Scoot(dir));
        }
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

        float dir = Random.value > 0.5f ? 1f : -1f;
        StartCoroutine(Scoot(dir));
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

        float move = Mathf.Sign(direction) * Mathf.Abs(scootDistance);
        Vector3 end = transform.position + new Vector3(move, 0f, 0f);

        DisableAllRenderers();
        if (move > 0f)
            swimmingRightRenderer.enabled = true;
        else
            idleRenderer.enabled = true;

        float elapsed = 0f;
        Vector3 start = transform.position;
        while (elapsed < scootDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / scootDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        DisableAllRenderers();
        idleRenderer.enabled = true;

        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnAfterDelay(-move));

        isMoving = false;
    }

    IEnumerator ReturnAfterDelay(float returnMove)
    {
        yield return new WaitForSeconds(returnDelay);

        DisableAllRenderers();
        if (returnMove > 0f)
            swimmingRightRenderer.enabled = true;
        else
            idleRenderer.enabled = true;

        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(returnMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        DisableAllRenderers();
        idleRenderer.enabled = true;
    }

    void DisableAllRenderers()
    {
        if (idleRenderer) idleRenderer.enabled = false;
        if (lookingRenderer) lookingRenderer.enabled = false;
        if (legUpRenderer) legUpRenderer.enabled = false;
        if (sleepingRenderer) sleepingRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
    }

    public void SetToIdleOnly()
    {
        DisableAllRenderers();
        if (idleRenderer) idleRenderer.enabled = true;
        legLifted = false;
    }

    public void SetToLookingOnly()
    {
        DisableAllRenderers();
        if (lookingRenderer) lookingRenderer.enabled = true;
    }

    public void SetToLegLiftOnly()
    {
        DisableAllRenderers();
        if (legUpRenderer) legUpRenderer.enabled = true;
    }

    public void SetToSleepingOnly()
    {
        DisableAllRenderers();
        if (sleepingRenderer) sleepingRenderer.enabled = true;
    }
}
