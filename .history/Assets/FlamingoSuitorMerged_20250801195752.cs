using UnityEngine;
using System.Collections;

public class FlamingoSuitorMerged : MonoBehaviour
{
    [Header("Main Renderers")]
    public SpriteRenderer idleRenderer;
    public SpriteRenderer lookingRenderer;
    public SpriteRenderer legUpRenderer;
    public SpriteRenderer sleepingRenderer;
    public SpriteRenderer swimmingRightRenderer;

    [Header("Settings")]
    public GameObject starObject;
    public float starFallAmount = 5f;
    public float moveDistance = 1f;
    public float moveDuration = 0.25f;
    public float returnDelay = 6f;
    public float returnDuration = 0.25f;
    public float scootChanceIdle = 0.25f;
    public float scootChanceNearPetal = 0.05f;
    public string petalTag = "Petal";
    public float petalCheckRadius = 1.5f;

    [Header("Optional Environment Object")]
    public GameObject environmentObject;
    public float environmentFadeDuration = 1f;

    [Header("Sound Effects")]
    public AudioClip environmentFadeSound;
    public AudioClip starFallSound;
    public AudioClip scootSound;

    private AudioSource audioSource;

    private bool scootOverrideActive = false;

    private bool cursorIsNearby = false;
    private bool legLifted = false;
    private bool isSleeping = false;
    private bool isMoving = false;
    private bool starFallen = false;

    private Vector3 originalPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetToIdleOnly();
        originalPosition = transform.position;
    }

    void Update()
    {
        if (cursorIsNearby && !legLifted && !isSleeping && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(RespondWithLegLift());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;
        cursorIsNearby = true;

        if (!legLifted && !isSleeping)
            SetToLookingOnly();

        if (!legLifted && !isSleeping && !isMoving)
        {
            float chance = IsPetalNearby() ? scootChanceNearPetal : scootChanceIdle;
            if (Random.value < chance)
            {
                StartCoroutine(Scoot(Random.Range(0, 2) == 0 ? -1f : 1f));
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Cursor")) return;
        cursorIsNearby = false;

        if (!legLifted && !isSleeping)
            SetToIdleOnly();
    }

    bool IsPetalNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, petalCheckRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(petalTag)) return true;
        }
        return false;
    }

    IEnumerator RespondWithLegLift()
    {
        legLifted = true;
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        SetToLegLiftOnly();
        yield return new WaitForSeconds(3f);

        if (Random.value <= 0.8f)
        {
            StartCoroutine(SwitchToSleeping());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            TryScootRandomly();
        }
    }

    IEnumerator SwitchToSleeping()
    {
        isSleeping = true;
        SetToSleepingOnly();

        if (!starFallen && starObject != null)
        {
            starFallen = true;
            StartCoroutine(FallStar());
        }

        yield return new WaitForSeconds(Random.Range(60f, 90f));

        isSleeping = false;
        SetToIdleOnly();
        TryScootRandomly();
    }

    IEnumerator FallStar()
    {
        if (starFallSound != null) PlaySound(starFallSound);
        Vector3 start = starObject.transform.position;
        Vector3 end = start - new Vector3(0f, starFallAmount, 0f);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            starObject.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        starObject.transform.position = end;
    }

    void SetToIdleOnly()
    {
        if (scootOverrideActive) return; // ❌ block idleRenderer while scooting
        EnableOnly(idleRenderer);
        legLifted = false;
    }


    void SetToLookingOnly()
    {
        if (scootOverrideActive) return;
        EnableOnly(lookingRenderer);
    }
    void SetToLegLiftOnly()
    {
        EnableOnly(legUpRenderer);
        if (environmentObject != null)
        {
            PlaySound(environmentFadeSound);
            StartCoroutine(FadeInEnvironment());
        }
    }

    IEnumerator FadeInEnvironment()
    {
        SpriteRenderer envRenderer = environmentObject.GetComponent<SpriteRenderer>();
        if (envRenderer == null) yield break;

        Color color = envRenderer.color;
        color.a = 0f;
        envRenderer.color = color;
        envRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < environmentFadeDuration)
        {
            float t = elapsed / environmentFadeDuration;
            color.a = Mathf.Lerp(0f, 1f, t);
            envRenderer.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        envRenderer.color = color;
    }

    void SetToSleepingOnly() => EnableOnly(sleepingRenderer);

    void EnableOnly(SpriteRenderer target)
    {
        idleRenderer.enabled = false;
        lookingRenderer.enabled = false;
        legUpRenderer.enabled = false;
        sleepingRenderer.enabled = false;
        swimmingRightRenderer.enabled = false;

        if (target != null) target.enabled = true;
    }

    void TryScootRandomly()
    {
        float dir = Random.value > 0.5f ? 1f : -1f;
        StartCoroutine(Scoot(dir));
    }

    IEnumerator Scoot(float direction)
    {
        if (scootSound != null) PlaySound(scootSound);
        if (isMoving) yield break;
        isMoving = true;
        scootOverrideActive = true;

        float signedMove = Mathf.Sign(direction) * Mathf.Abs(moveDistance);
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(signedMove, 0f, 0f);

        Debug.Log($"[Scoot] Direction: {direction} | Using: {(direction > 0 ? "swimmingRightRenderer" : "idleRenderer")}");

        EnableOnly(direction > 0 ? swimmingRightRenderer : idleRenderer);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        yield return new WaitForSeconds(returnDelay);

        Vector3 returnStart = transform.position;
        Vector3 returnEnd = originalPosition;
        float returnMove = returnEnd.x - returnStart.x;

        EnableOnly(returnMove > 0 ? swimmingRightRenderer : idleRenderer);

        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(returnStart, returnEnd, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;

        scootOverrideActive = false; // ✅ only allow idle visuals again now
        SetToIdleOnly();
        isMoving = false;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
