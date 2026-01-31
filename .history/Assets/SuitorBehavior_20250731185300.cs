using UnityEngine;
using System.Collections;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Scoot Settings")]
    public float moveDistance = 1f; // Always positive
    public float moveDuration = 0.25f;
    public string playerTag = "Cursor";

    [Header("Visuals")]
    public SpriteRenderer rootRenderer;            // Left-facing (idle & swim)
    public SpriteRenderer swimmingRightRenderer;   // Right-facing swim
    public SpriteRenderer standingRenderer;
    public SpriteRenderer legupRenderer;
    public SpriteRenderer lookingRenderer;

    private bool isMoving = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag) || isMoving) return;

        float scootChance = 0.4f;
        if (Random.value <= scootChance)
        {
            bool goRight = Random.value > 0.5f;
            StartCoroutine(Scoot(goRight ? 1f : -1f));
        }
    }

    IEnumerator Scoot(float direction)
    {
        isMoving = true;

        // Clean slate: disable all visuals
        DisableAllSuitorRenderers();

        // Choose facing
        if (direction > 0)
        {
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = true;
        }
        else
        {
            if (rootRenderer) rootRenderer.enabled = true;
        }

        // Move
        float signedMove = direction * Mathf.Abs(moveDistance);
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

        // Restore idle left-facing
        DisableAllSuitorRenderers();
        if (rootRenderer) rootRenderer.enabled = true;

        isMoving = false;
    }

    void DisableAllSuitorRenderers()
    {
        if (rootRenderer) rootRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (standingRenderer) standingRenderer.enabled = false;
        if (legupRenderer) legupRenderer.enabled = false;
        if (lookingRenderer) lookingRenderer.enabled = false;
    }
}
