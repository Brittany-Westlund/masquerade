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

        if (!legLifted) // If not already lifting leg, return to idle
            SetToIdleOnly();
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
        float delay = Random.Range(1f, 2f);
        yield return new WaitForSeconds(delay);
        SetToLegLiftOnly();
    }

    void SetToIdleOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = true;
        if (lookingRenderer != null) lookingRenderer.enabled = false;
        if (legUpRenderer != null) legUpRenderer.enabled = false;
    }

    void SetToLookingOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = false;
        if (lookingRenderer != null) lookingRenderer.enabled = true;
        if (legUpRenderer != null) legUpRenderer.enabled = false;
    }

    void SetToLegLiftOnly()
    {
        if (idleRenderer != null) idleRenderer.enabled = false;
        if (lookingRenderer != null) lookingRenderer.enabled = false;
        if (legUpRenderer != null) legUpRenderer.enabled = true;
    }
}
