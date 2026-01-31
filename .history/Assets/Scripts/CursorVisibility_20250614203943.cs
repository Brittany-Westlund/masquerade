using UnityEngine;

public class ResetCursorOnSceneLoad : MonoBehaviour
{
    void Awake()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
        Debug.Log("Cursor reset to default and made visible in Awake.");
    }
}