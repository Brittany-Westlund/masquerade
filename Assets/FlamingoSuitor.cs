using UnityEngine;
using System.Collections;

public class FlamingoSuitor : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer idleRenderer;
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;

    [Header("Star Drop Settings")]
    public GameObject starObject;
    public float starFallAmount = 5f;

    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;

    void Start()
    {
        SetToIdleOnly();
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

    void Update()
    {
        if (cursorIsNearby && !legLifted && !isSleeping && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RespondWithLegLift());
        }
    }

    IEnumerator RespondWithLegLift()
    {
        legLifted = true;

        float delay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(delay);

        SetToLegLiftOnly();

        yield return new WaitForSeconds(10f); // Stay in leg lift for 10 seconds

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

        float sleepDuration = Random.Range(60f, 180f);
        yield return new WaitForSeconds(sleepDuration);

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

    public void SetToIdleOnly()
    {
        idleRenderer.enabled = true;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = false;

        legLifted = false;
    }

    public void SetToLookingOnly()
    {
        idleRenderer.enabled = false;
        lookingRenderer.enabled = true;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = false;
    }

    public void SetToLegLiftOnly()
    {
        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = true;
        sleepingRenderer.enabled = false;
    }

    public void SetToSleepingOnly()
    {
        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = true;
    }

    public void TryToScootAway()
    {
        SuitorBehavior suitor = GetComponent<SuitorBehavior>();
        if (suitor != null)
        {
           
        }
    }
}
