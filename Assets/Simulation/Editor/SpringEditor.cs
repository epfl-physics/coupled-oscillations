using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spring))]
public class SpringEditor : Editor
{
    private Spring.Type type = default;
    private int numCoils = 0;
    private float k = 0;
    private Vector3 point1 = Vector3.zero;
    private Vector3 point2 = Vector3.zero;
    private float height = 1;

    public override void OnInspectorGUI()
    {
        var spring = target as Spring;

        if (type != spring.type)
        {
            spring.Redraw();
            type = spring.type;
        }

        if (numCoils != spring.numCoils)
        {
            spring.Redraw();
            numCoils = spring.numCoils;
        }

        if (k != spring.k)
        {
            spring.Redraw();
            k = spring.k;
        }

        if (point1 != spring.point1 || point2 != spring.point2)
        {
            spring.Redraw();
            point1 = spring.point1;
            point2 = spring.point2;
        }

        if (height != spring.height)
        {
            spring.Redraw();
            height = spring.height;
        }

        DrawDefaultInspector();
    }
}
