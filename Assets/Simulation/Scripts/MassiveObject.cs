using UnityEngine;

public class MassiveObject : MonoBehaviour
{
    [Min(0)] public float mass = 1f;
    public float HalfScale { get { return 0.5f * transform.localScale.x; } }

    [Header("Interactions")]
    [SerializeField] private bool interactable;
    [SerializeField] private Texture2D hoverCursor = null;
    [SerializeField] private Vector2 hotspot = default;

    public void SetMass(float mass)
    {
        transform.localScale = (1 + Mathf.Log10(Mathf.Max(0.1f, Mathf.Pow(mass, 0.33f)))) * Vector3.one;
        this.mass = mass;
    }

    private void OnMouseEnter()
    {
        if (!interactable) return;

        // Display the cursor while hovering
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hotspot, CursorMode.Auto);
        }
    }

    private void OnMouseExit()
    {
        if (!interactable) return;

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

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }
}
