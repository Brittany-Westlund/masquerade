using UnityEngine;
using System.Collections;

public class CustomCursorManager : MonoBehaviour
{
    [Header("Cursor Sprites")]
    public Sprite defaultSprite;
    public Sprite clickedSprite;

    [Header("Idle Settings")]
    public GameObject idleObject;
    public float idleDelay = 1f;
    public float fadeSpeed = 2f;

    [Header("Cursor Object")]
    public GameObject cursorObject;

    private SpriteRenderer cursorRenderer;
    private SpriteRenderer idleRenderer;

    private Vector3 lastMousePosition;
    private float lastMoveTime;
    private bool isIdle = false;

    void Awake()
    {
        // 👇 Set invisible texture before first frame
        Texture2D invisible = new Texture2D(1, 1);
        invisible.SetPixel(0, 0, new Color(0, 0, 0, 0));
        invisible.Apply();
        Cursor.SetCursor(invisible, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true; // Must remain true when using SetCursor
    }

    void Start()
    {
        if (cursorObject != null)
            cursorRenderer = cursorObject.GetComponent<SpriteRenderer>();

        if (cursorRenderer != null && defaultSprite != null)
            cursorRenderer.sprite = defaultSprite;

        if (idleObject != null)
        {
            idleRenderer = idleObject.GetComponent<SpriteRenderer>();
            SetAlpha(idleRenderer, 0f); // Start hidden
        }

        lastMousePosition = Input.mousePosition;
        lastMoveTime = Time.time;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (cursorObject != null)
            cursorObject.transform.position = worldPos;

        // Click feedback
        if (Input.GetMouseButtonDown(0) && cursorRenderer != null && clickedSprite != null)
            cursorRenderer.sprite = clickedSprite;
        else if (Input.GetMouseButtonUp(0) && cursorRenderer != null && defaultSprite != null)
            cursorRenderer.sprite = defaultSprite;

        // Idle logic
        if (mousePos != lastMousePosition)
        {
            lastMousePosition = mousePos;
            lastMoveTime = Time.time;

            if (isIdle)
            {
                isIdle = false;
                if (idleRenderer != null) SetAlpha(idleRenderer, 0f);
                if (cursorObject != null) cursorObject.SetActive(true);
            }
        }
        else if (!isIdle && Time.time - lastMoveTime >= idleDelay)
        {
            isIdle = true;
            if (cursorObject != null) cursorObject.SetActive(false);
        }

        // Fade in idle object
        if (idleRenderer != null)
        {
            float targetAlpha = isIdle ? 1f : 0f;
            float currentAlpha = idleRenderer.color.a;
            float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            SetAlpha(idleRenderer, newAlpha);
        }
    }

    void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color c = renderer.color;
        c.a = alpha;
        renderer.color = c;
    }
}
