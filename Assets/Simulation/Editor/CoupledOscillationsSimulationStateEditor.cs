// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CoupledOscillationsSimulationState))]
public class CoupledOscillationsSimulationStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var simState = target as CoupledOscillationsSimulationState;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset"))
        {
            simState.x = new double[4] { -1, 0, 0, 0 };
            simState.xdot = new double[4] { 0, 0, 7, -2 };
        }
    }
}
