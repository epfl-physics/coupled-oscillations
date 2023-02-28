using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D hoverCursor = null;
    [SerializeField] private Vector2 hotspot = default;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display the cursor while hovering
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hotspot, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RestoreDefault();
    }

    private void RestoreDefault()
    {
        // Restore the default cursor
        if (hoverCursor != null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnDisable()
    {
        RestoreDefault();
    }
}
