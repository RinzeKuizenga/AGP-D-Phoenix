using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTextureDefault;
    [SerializeField] private Texture2D scrubTexture;

    [SerializeField] private Vector2 clickPosition = Vector2.zero;

    public static CursorController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.Auto);
    }

    public void SetToMode(ModeOfCursor modeOfCursor)
    {
        switch (modeOfCursor)
        {
            case ModeOfCursor.Default:
                Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.Auto);
                break;
            case ModeOfCursor.Scrub:
                Cursor.SetCursor(scrubTexture, clickPosition, CursorMode.Auto);
                break;
        }
    }

    public enum ModeOfCursor
    {
        Default, 
        Scrub
    }
}
