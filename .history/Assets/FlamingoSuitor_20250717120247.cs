using UnityEngine;
using System.Collections;

public class FlamingoSuitor : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer idleRenderer;
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer standingRenderer;
    public SpriteRenderer legUpRenderer;

    [Header("Looking Behavior")]
    public float lookFrequencyMin = 3f;
    public float lookFrequencyMax = 6f;
    public float lookDuration = 1f;

    private bool cursorIsNearby = false;
    private bool flamingoWelcomed = false;
    private CustomCursorManager cursorManager;
    private Coroutine lookingRoutine;

    private void Start()
    {
        cursorManager = FindObjectOfType<CustomCursorManager>();
        ResetToIdle();
        lookingRoutine = StartCoroutine(RandomLookingRoutine());
    }

    private void ResetToIdle()
    {
        idleRenderer.enabled = true;
        lookingRenderer.enabled = false;
        standingRenderer.enabled = false;
        legUpRenderer.enabled = false;
    }

    IEnumerator RandomLookingRoutine()
    {
        while (!flamingoWelcomed)
        {
            yield return new WaitForSeconds(Random.Range(lookFrequencyMin, lookFrequencyMax));

            if (!cursorIsNearby)
            {
                idleRenderer.enabled = false;
                lookingRenderer.enabled = true;
                yield return new WaitForSeconds(lookDuration);
                lookingRenderer.enabled = false;
                idleRenderer.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (flamingoWelcomed || !other.CompareTag("Cursor")) return;

        cursorIsNearby = true;

        StopLooking(); // Stop glance behavior
        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        standingRenderer.enabled = true;

        if (cursorManager != null)
            cursorManager.SetToStanding();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (flamingoWelcomed || !other.CompareTag("Cursor")) return;

        cursorIsNearby = false;
        ResetToIdle();

        if (cursorManager != null)
            cursorManager.SetToDefault();

        // Resume glance behavior
        if (lookingRoutine != null)
            StopCoroutine(lookingRoutine);
        lookingRoutine = StartCoroutine(RandomLookingRoutine());
    }

    private void StopLooking()
    {
        if (lookingRoutine != null)
            StopCoroutine(lookingRoutine);

        idleRenderer.enabled = true;
        lookingRenderer.enabled = false;
    }

    private void Update()
    {
        if (cursorIsNearby && !flamingoWelcomed && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(WelcomingDecisionRoutine());
        }
    }

    IEnumerator WelcomingDecisionRoutine()
    {
        flamingoWelcomed = true;
        StopAllCoroutines();

        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        standingRenderer.enabled = true;
        legUpRenderer.enabled = false;

        if (cursorManager != null)
            cursorManager.SetToDefault();

        // Small random pause before decision
        float delay = Random.Range(0.4f, 1.2f);
        yield return new WaitForSeconds(delay);

        float roll = Random.value;

        if (roll <= 0.8f)
        {
            // Accept – leg up
            standingRenderer.enabled = false;
            legUpRenderer.enabled = true;

            // Make sure the flamingo doesn't revert visual later
            flamingoWelcomed = true;
            cursorIsNearby = false; // To prevent Update() or other triggers from interfering
        }
        else
        {
            // Decline – scoot away
            SuitorBehavior suitor = GetComponent<SuitorBehavior>();
            if (suitor != null)
            {
                StartCoroutine(suitor.Scoot());
            }

            // Revert to idle visuals (optional)
            ResetToIdle();
            flamingoWelcomed = false; // Optional: allow retry
        }
    }

    public void SetToIdleOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = true;
        if (lookingRenderer != null) lookingRenderer.enabled = false;
        if (standingRenderer != null) standingRenderer.enabled = false;
        if (legUpRenderer != null) legUpRenderer.enabled = false;
    }
}
