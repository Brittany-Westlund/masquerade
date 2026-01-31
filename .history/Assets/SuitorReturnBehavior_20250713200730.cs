using UnityEngine;
using System.Collections;

public class SuitorReturnBehavior : MonoBehaviour
{
    [Header("Return Settings")]
    public float returnDelay = 5f;         // Time after scooting to return
    public float returnMoveDistance = -1f; // Negative = return left, Positive = return right
    public float returnDuration = 0.25f;

    [Header("Facing Direction")]
    public bool startsFacingRight = true;  // Set this manually in the Inspector based on initial sprite

    private SuitorBehavior suitor;
    private bool isReturning = false;

    void Awake()
    {
        suitor = GetComponent<SuitorBehavior>();

        // Set initial facing direction based on Inspector setting
        Vector3 scale = transform.localScale;
        if ((startsFacingRight && scale.x < 0) || (!startsFacingRight && scale.x > 0))
        {
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    public void StartReturnTimer()
    {
        if (!isReturning)
            StartCoroutine(ReturnAfterDelay());
    }

    IEnumerator ReturnAfterDelay()
    {
        isReturning = true;
        yield return new WaitForSeconds(returnDelay);

        // Flip to face the return direction
        FaceDirection(returnMoveDistance);

        // Move back
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

        // Reset SuitorBehavior so it can scoot again
        if (suitor != null)
            suitor.enabled = true;
    }

    void FaceDirection(float direction)
    {
        if (direction == 0f) return;

        Vector3 scale = transform.localScale;
        bool shouldFaceRight = direction > 0f;
        if ((shouldFaceRight && scale.x < 0) || (!shouldFaceRight && scale.x > 0))
        {
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }
}
