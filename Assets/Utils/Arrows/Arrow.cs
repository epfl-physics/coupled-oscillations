using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Arrow : MonoBehaviour
{
    [HideInInspector] public LineRenderer bodyLR;

    [Header("Properties")]
    public Vector3 components;
    public Color color = Color.black;
    [Min(0)] public float lineWidth = 0.2f;
    public int sortingOrder = 0;

    [Header("Head")]
    public LineRenderer headLR;
    [Range(0, 60)] public float headAngle = 45;

    private void Awake()
    {
        bodyLR = GetComponent<LineRenderer>();
        bodyLR.positionCount = 2;
        if (headLR) headLR.positionCount = 3;
    }

    public void Redraw()
    {
        // Draw the body
        bodyLR.SetPositions(new Vector3[] { Vector3.zero, components });

        float width = Mathf.Min(components.magnitude / 3, lineWidth);

        bodyLR.startWidth = width;
        bodyLR.endWidth = width;
        bodyLR.startColor = color;
        bodyLR.endColor = color;

        bodyLR.sortingOrder = sortingOrder;

        // Draw the head
        if (!headLR) return;

        Vector3 headPosition = components;
        // Direction along the arrow
        Vector3 e1 = components.normalized;
        // Direction orthogonal to the vector in the plane spanned by the arrow and the y-axis
        Vector3 e2 = (e1.x == 0) ? Vector3.right : Vector3.Cross(Vector3.Cross(e1, Vector3.up).normalized, e1);

        float angle = Mathf.Deg2Rad * headAngle;
        float headLength = Mathf.Min(components.magnitude, 2 * width);
        Vector3 headPoint1 = headPosition + headLength * (-Mathf.Cos(angle) * e1 + Mathf.Sin(angle) * e2);
        Vector3 headPoint2 = headPosition + headLength * (-Mathf.Cos(angle) * e1 - Mathf.Sin(angle) * e2);
        headLR.SetPositions(new Vector3[] { headPoint1, headPosition, headPoint2 });

        headLR.startWidth = width;
        headLR.endWidth = width;
        headLR.startColor = color;
        headLR.endColor = color;

        headLR.sortingOrder = sortingOrder;
    }

    public void SetComponents(Vector3 components, bool redraw = true)
    {
        this.components = components;
        if (redraw) Redraw();
    }
}
