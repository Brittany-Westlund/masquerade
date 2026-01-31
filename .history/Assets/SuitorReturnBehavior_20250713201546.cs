using UnityEngine;
using System.Collections;

public class SuitorReturnBehavior : MonoBehaviour
{
    [Header("Return Settings")]
    public float returnDelay = 5f;
    public float returnDuration = 0.25f;

    [Header("Facing Direction")]
    public bool startsFacingRight = true;

    private bool isReturning = false;
    private Vector3 initialScale;
    private Coroutine returnCoroutine;

    void Awake()
    {
        initialScale = transform.localScale;
        ApplyInitialFacing();
    }

    void ApplyInitialFacing()
    {
        if ((startsFacingRight && transform.localScale.x < 0) || (!startsFacingRight && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    public void StartReturnTimer(float lastMoveDistance)
    {
        if (isReturning || Mathf.Approximately(lastMoveDistance, 0f))
        {
            Debug.Log("[SuitorReturn] Not starting return: already returning or no movement.");
            return;
        }

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        Debug.Log($"[SuitorReturn] Starting return timer with delay {returnDelay}s...");
        returnCoroutine = StartCoroutine(ReturnAfterDelay(-lastMoveDistance));
    }

    IEnumerator ReturnAfterDelay(float returnMoveDistance)
    {
        isReturning = true;

        yield return new WaitForSeconds(returnDelay);

        Debug.Log("[SuitorReturn] Executing return movement.");

        FaceDirection(returnMoveDistance);

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(returnMoveDistance, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isReturning = false;
        returnCoroutine = null;
    }

    void FaceDirection(float direction)
    {
        if (Mathf.Approximately(direction, 0f)) return;

        Vector3 scale = transform.localScale;
        bool shouldFaceRight = direction > 0;
        bool currentlyFacingRight = scale.x > 0;

        if (shouldFaceRight != currentlyFacingRight)
        {
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }
}
