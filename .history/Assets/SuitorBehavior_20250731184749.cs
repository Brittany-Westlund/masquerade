using UnityEngine;
using System.Collections;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1f;
    public float moveDuration = 0.25f;

    [Header("Sprite Renderers")]
    public SpriteRenderer rootRenderer;            // Idle / Left-facing
    public SpriteRenderer swimmingRightRenderer;   // Right-facing

    private bool isMoving = false;

    public IEnumerator Scoot(float direction)
    {
        if (isMoving) yield break;
        isMoving = true;

        direction = Mathf.Sign(direction); // Normalize to -1 or 1
        float signedMove = direction * Mathf.Abs(moveDistance);

        // Disable both, then enable correct one
        DisableAllSuitorRenderers();

        if (direction > 0 && swimmingRightRenderer)
            swimmingRightRenderer.enabled = true;
        else if (rootRenderer)
            rootRenderer.enabled = true;

        // Move
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

        // Return to idle (always left-facing rootRenderer)
        DisableAllSuitorRenderers();
        if (rootRenderer) rootRenderer.enabled = true;

        isMoving = false;
    }

    public void ScootLeft()
    {
        StartCoroutine(Scoot(-1f));
    }

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
