using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public enum Type { Simple, ZigZag, Coiled }
    public Type type = Type.Simple;
    [Min(0)] public int numCoils = 3;
    [SerializeField] private Color color = Color.blue;
    public float k = 1f;  // [N / m]
    public float height = 1f;  // [m]

    [Header("Endpoints")]
    public Vector3 point1 = Vector3.left;
    public Vector3 point2 = Vector3.right;

    public void SetEndpoints(Vector3 point1, Vector3 point2, bool redraw = true)
    {
        this.point1 = point1;
        this.point2 = point2;
        if (redraw) Redraw();
    }

    public void Redraw()
    {
        if (lineRenderer == null) { return; }

        float width = 0.1f * Mathf.Log10(1 + Mathf.Max(1, k));
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        switch (type)
        {
            case Type.Simple:
                lineRenderer.positionCount = 2;
                lineRenderer.numCornerVertices = 0;
                lineRenderer.SetPositions(new Vector3[] { point1, point2 });
                break;
            case Type.ZigZag:
                int numPositions = 4 + 2 * numCoils;
                int numSegments = numPositions - 2;
                lineRenderer.positionCount = numPositions;
                lineRenderer.numCornerVertices = 5;
                Vector3 displacement = point2 - point1;
                Vector3 xHat = displacement.normalized;
                Vector3 yHat = Vector3.Cross(Vector3.Cross(xHat, Vector3.up), xHat);
                float delta = displacement.magnitude / numSegments;
                Vector3[] positions = new Vector3[numPositions];
                positions[0] = point1;
                positions[1] = point1 + 1 * delta * xHat;
                for (int i = 0; i < numSegments; i++)
                {
                    float sign = (i % 2) == 0 ? 1 : -1;
                    positions[2 + i] = point1 + (1.5f + i) * delta * xHat + sign * height * yHat;
                }
                positions[numPositions - 2] = point2 - delta * xHat;
                positions[numPositions - 1] = point2;
                lineRenderer.SetPositions(positions);
                break;
            default:
                break;
        }
    }
}
