using UnityEngine;

public class SuitorBehavior : MonoBehaviour
{
    [Header("Settings")]
    public float moveDistance = 1f;         // How far to scoot
    public float moveDuration = 0.25f;      // How quickly to scoot
    public string playerTag = "Player";     // Tag of the PC

    private bool isMoving = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving || !other.CompareTag(playerTag))
            return;

        float chance = Random.value; // Returns 0 to 1

        if (chance <= 0.4f)
        {
            StartCoroutine(Scoot(Random.value < 0.5f ? -1 : 1)); // Left or right
        }
    }

    System.Collections.IEnumerator Scoot(int direction)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(moveDistance * direction, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
