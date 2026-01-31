using UnityEngine;

public class ResetCursorOnSceneLoad : MonoBehaviour
{
    void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;
            Debug.Log("Cursor reset for Main Menu.");
        }
    }
}