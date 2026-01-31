using UnityEngine;
using System.Collections;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;              // Always positive in Inspector
    public float moveDuration = 0.25f;

    [Header("Sprite Renderers")]
    public SpriteRenderer rootRenderer;            // Left-facing idle/swimming
    public SpriteRenderer swimmingRightRenderer;   // Right-facing swimming

    private bool isMoving = false;

    /// <summary>
    /// Starts scooting left (-1) or right (+1)
    /// </summary>
    public IEnumerator Scoot(float direction)
    {
        if (isMoving) yield break;
        isMoving = true;

        direction = Mathf.Sign(direction); // Ensure it's -1 or +1
        float signedMove = direction * Mathf.Abs(moveDistance);

        // 🔒 Disable all renderers first
        DisableAllSuitorRenderers();

        // 🎯 Show the correct one
        if (direction > 0)
        {
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = true;
        }
        else
        {
            if (rootRenderer) rootRenderer.enabled = true;
        }

        // 🏃 Move
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // 💤 Back to idle = left-facing swimming (rootRenderer)
        DisableAllSuitorRenderers();
        if (rootRenderer) rootRenderer.enabled = true;

        isMoving = false;
    }

    /// <summary>
    /// Helper to scoot left
    /// </summary>
    public void ScootLeft()
    {
        StartCoroutine(Scoot(-1f));
    }

    /// <summary>
    /// Helper to scoot right
    /// </summary>
    public void ScootRight()
    {
        StartCoroutine(Scoot(1f));
    }

    private void DisableAllSuitorRenderers()
    {
        if (rootRenderer) rootRenderer.enabled = false;
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
    }

}
