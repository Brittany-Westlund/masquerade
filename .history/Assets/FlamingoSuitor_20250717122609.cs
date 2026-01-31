using UnityEngine;
using System.Collections;

public class FlamingoSuitor : MonoBehaviour
{
    [Header("Sprites")]
    public SpriteRenderer idleRenderer;
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;

    private bool cursorIsNearby = false;
    private bool legLifted = false;

    void Start()
    {
        SetToIdleOnly();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;

        cursorIsNearby = true;
        SetToLookingOnly();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;

        cursorIsNearby = false;

        // Only reset if they haven't lifted their leg yet
        if (!legLifted)
        {
            SetToIdleOnly();
        }

        // If leg has been lifted, do nothing — stay in leg pose forever!
    }

    void Update()
    {
        if (cursorIsNearby && !legLifted && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RespondWithLegLift());
        }
    }

    IEnumerator RespondWithLegLift()
    {
        legLifted = true;

        // Delay for realism
        float delay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(delay);

        SetToLegLiftOnly();

        // Optional: 20% chance to scoot away after lifting leg
        if (Random.value <= 0.2f)
        {
            TryToScootAway();
        }

        // ❌ Do NOT switch back to idle or standing afterward
    }

    public void SetToIdleOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = true;
        if (lookingRenderer != null) lookingRenderer.enabled = false;
        if (legUpRenderer != null) legUpRenderer.enabled = false;
    }

    public void SetToLookingOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = false;
        if (lookingRenderer != null) lookingRenderer.enabled = true;
        if (legUpRenderer != null) legUpRenderer.enabled = false;
    }

    public void SetToLegLiftOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = false;
        if (lookingRenderer != null) lookingRenderer.enabled = false;
        if (legUpRenderer != null) legUpRenderer.enabled = true;
    }


    public void TryToScootAway()
    {
        SuitorBehavior suitor = GetComponent<SuitorBehavior>();
        if (suitor != null)
        {
            StartCoroutine(suitor.Scoot());
        }
    }

}
