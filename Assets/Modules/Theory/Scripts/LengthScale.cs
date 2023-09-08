using UnityEngine;

public class LengthScale : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform bar;
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;
    [SerializeField] private Transform label;

    [Header("Properties")]
    [SerializeField] private float xPositionLeft = -1;
    [SerializeField] private float xPositionRight = 1;
    [SerializeField] private Color color = Color.black;
    [SerializeField, Range(0.01f, 0.2f)] private float thickness = 0.1f;
    [SerializeField] private float height = 0.4f;
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private bool leftEdgeIgnoreThickness = false;
    [SerializeField] private bool rightEdgeIgnoreThickness = false;

    private void OnValidate()
    {
        if (!Application.isPlaying) Redraw();
    }

    public void Redraw()
    {
        float length = xPositionRight - xPositionLeft;
        if (bar)
        {
            Vector3 position = bar.localPosition;
            position.x = xPositionLeft + 0.5f * length;
            bar.localPosition = position;
            Vector3 scale = Vector3.one;
            scale.x = length - thickness;
            scale.y = thickness;
            bar.localScale = scale;
            var sr = bar.GetComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }

        if (leftEdge)
        {
            Vector3 position = leftEdge.localPosition;
            position.x = xPositionLeft;
            leftEdge.localPosition = position;

            if (!leftEdgeIgnoreThickness)
            {
                Vector3 scale = Vector3.one;
                scale.x = thickness;
                scale.y = height;
                leftEdge.localScale = scale;
            }

            var sr = leftEdge.GetComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }

        if (rightEdge)
        {
            Vector3 position = rightEdge.localPosition;
            position.x = xPositionRight;
            rightEdge.localPosition = position;

            if (!rightEdgeIgnoreThickness)
            {
                Vector3 scale = Vector3.one;
                scale.x = thickness;
                scale.y = height;
                rightEdge.localScale = scale;
            }

            var sr = rightEdge.GetComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }

        if (label)
        {
            Vector3 position = label.localPosition;
            position.x = xPositionLeft + 0.5f * length;
            label.localPosition = position;
            var sr = label.GetComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = sortingOrder;
        }
    }

    public void SetXPositionLeft(float value, bool redraw = true)
    {
        xPositionLeft = value;
        if (redraw) Redraw();
    }

    public void SetXPositionRight(float value, bool redraw = true)
    {
        xPositionRight = value;
        if (redraw) Redraw();
    }

    public void SetXPositions(float[] values, bool redraw = true)
    {
        xPositionLeft = values[0];
        xPositionRight = values[1];
        if (redraw) Redraw();
    }

    public float GetXPositionLeft()
    {
        return xPositionLeft;
    }

    public float GetXPositionRight()
    {
        return xPositionRight;
    }
}
