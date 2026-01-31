using UnityEngine;
using System.Collections;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;              // Always positive
    public float moveDuration = 0.25f;

    [Header("Sprite Renderers")]
    public SpriteRenderer rootRenderer;            // Left-facing idle/swimming
    public SpriteRenderer swimmingRightRenderer;   // Right-facing swimming

    private bool isMoving = false;

    // This is now a public method that YOU call from other code
    public IEnumerator Scoot(float direction)
    {
        if (isMoving) yield break;
        isMoving = true;

        direction = Mathf.Sign(direction); // make sure it's -1 or 1
        float signedMove = direction * Mathf.Abs(moveDistance);

        // 🔁 Enable correct renderer based on direction
        if (direction > 0)
        {
            // Moving RIGHT
            if (rootRenderer) rootRenderer.enabled = false;
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = true;
        }
        else
        {
            // Moving LEFT
            if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
            if (rootRenderer) rootRenderer.enabled = true;
        }

        // 🏃 Move
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(signedMove, 0f, 0f);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // 💤 Return to idle state: always left-facing swimming
        if (swimmingRightRenderer) swimmingRightRenderer.enabled = false;
        if (rootRenderer) rootRenderer.enabled = true;

        isMoving = false;
    }
}
