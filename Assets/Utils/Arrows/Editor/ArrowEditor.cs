using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Arrow))]
public class ArrowEditor : Editor
{
    private Arrow arrow;
    private Vector3 components;
    private Color color;
    private float lineWidth;
    private int sortingOrder;
    private float headAngle;
    // private float maxHeadSize;

    private void OnEnable()
    {
        arrow = target as Arrow;
        arrow.bodyLR = arrow.GetComponent<LineRenderer>();
    }

    private void OnDisable()
    {
        arrow = null;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!arrow) return;

        bool needToRedraw = false;
        needToRedraw |= Check<Vector3>(ref components, arrow.components);
        needToRedraw |= Check<Color>(ref color, arrow.color);
        needToRedraw |= Check<float>(ref lineWidth, arrow.lineWidth);
        needToRedraw |= Check<int>(ref sortingOrder, arrow.sortingOrder);

        if (arrow.headLR)
        {
            needToRedraw |= Check<float>(ref headAngle, arrow.headAngle);
            // needToRedraw |= Check<float>(ref maxHeadSize, arrow.maxHeadSize);
        }

        if (needToRedraw) arrow.Redraw();

        // base.OnInspectorGUI();
    }

    private bool Check<T>(ref T first, T second)
    {
        // Return true if first != second and we therefore need to redraw
        bool result = false;
        if (!first.Equals(second))
        {
            first = second;
            result = true;
        }
        return result;
    }
}
