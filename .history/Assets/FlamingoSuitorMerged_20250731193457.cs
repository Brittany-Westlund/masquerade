using UnityEngine;
using System.Collections;

public class FlamingoSuitorMerged : MonoBehaviour
{
    [Header("Suitor Renderers")]
    public SpriteRenderer idleRenderer;              // Left-facing swimming
    public SpriteRenderer swimmingRightRenderer;     // Right-facing swimming
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;
    
    [Header("Petal Scoot Modifier")]
    public string petalTag = "Petal";
    public float petalRadius = 1.5f;

    [Header("Settings")]
    public float scootDistance = 1f;
    public float scootDuration = 0.25f;
    public float starFallAmount = 5f;
    public GameObject starObject;
    public float returnDelay = 5f;

    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;
    private bool isMoving = false;

    private Vector3 originalPosition;
    private Coroutine returnCoroutine;

    void Start()
    {
        originalPosition = transform.position;
        SetToIdleOnly();
    }

    void Update()
    {
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
        {
            SetToLookingOnly();

            float scootChance = IsPetalNearby() ? 0.05f : 0.25f;
            if (Random.value <= scootChance)
            {
                float direction = Random.value > 0.5f ? 1f : -1f;
                TryToScootAway(direction);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
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

    public void TryToScootAway(float direction = 0f)
    {
        if (direction == 0f)
            direction = Random.value > 0.5f ? 1f : -1f;

        SuitorBehavior suitor = GetComponent<SuitorBehavior>();
        if (suitor != null)
        {
            if (direction > 0)
                suitor.ScootRight();
            else
                suitor.ScootLeft();
        }
    }

    IEnumerator Scoot(float direction)
    {
        isMoving = true;

        DisableAllRenderers();

        direction = Mathf.Sign(direction);
        float signedMove = direction * Mathf.Abs(scootDistance);

        if (direction > 0)
        {
            swimmingRightRenderer.enabled = true;
        }
        else
        {
            idleRenderer.enabled = true; // Left-facing swimming
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < scootDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / scootDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        yield return new WaitForSeconds(returnDelay);
        StartCoroutine(ReturnToOriginalPosition(-signedMove));
    }
    private bool IsPetalNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, petalRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(petalTag))
                return true;
        }
        return false;
    }

    IEnumerator ReturnToOriginalPosition(float returnDistance)
    {
        DisableAllRenderers();

        if (returnDistance > 0)
            swimmingRightRenderer.enabled = true;
        else
            idleRenderer.enabled = true;

        Vector3 start = transform.position;
        Vector3 end = originalPosition;
        float elapsed = 0f;

        while (elapsed < scootDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / scootDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        DisableAllRenderers();
        idleRenderer.enabled = true;
        isMoving = false;
    }

    void DisableAllRenderers()
    {
        if (idleRenderer) idleRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (lookingRenderer) lookingRenderer.enabled = false;
        if (legUpRenderer) legUpRenderer.enabled = false;
        if (sleepingRenderer) sleepingRenderer.enabled = false;
    }

    public void SetToIdleOnly()
    {
        DisableAllRenderers();
        idleRenderer.enabled = true;
        legLifted = false;
    }

    public void SetToLookingOnly()
    {
        DisableAllRenderers();
        lookingRenderer.enabled = true;
    }

    public void SetToLegLiftOnly()
    {
        DisableAllRenderers();
        legUpRenderer.enabled = true;
    }

    public void SetToSleepingOnly()
    {
        DisableAllRenderers();
        sleepingRenderer.enabled = true;
    }
}
