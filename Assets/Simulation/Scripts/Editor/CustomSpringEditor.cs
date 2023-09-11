using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomSpring)), CanEditMultipleObjects]
public class CustomSpringEditor : Editor
{
    private CustomSpring spring;

    private void OnEnable()
    {
        spring = target as CustomSpring;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields
        DrawDefaultInspector();

        // Check if the any fields have been changed
        if (GUI.changed)
        {
            spring.Redraw();
        }
    }
}
