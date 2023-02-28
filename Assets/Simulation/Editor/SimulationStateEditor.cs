using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimulationState))]
public class SimulationStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var simState = target as SimulationState;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset"))
        {
            simState.x = new double[4] { -1, 0, 0, 0 };
            simState.xdot = new double[4] { 0, 0, 7, -2 };
        }
    }
}
