using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MassiveObject))]
public class MassiveObjectEditor : Editor
{
    private float mass = 1;

    public override void OnInspectorGUI()
    {
        var massiveObject = target as MassiveObject;

        if (mass != massiveObject.mass)
        {
            massiveObject.SetMass(massiveObject.mass);
            mass = massiveObject.mass;
        }

        DrawDefaultInspector();
    }
}
