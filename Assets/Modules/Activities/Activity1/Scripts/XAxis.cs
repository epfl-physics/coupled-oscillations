// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

public class XAxis : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private Transform x1;
    [SerializeField] private Transform x2;
    [SerializeField] private Transform xCM;
    [SerializeField] private Transform xR;

    private void Update()
    {
        if (!sim) return;

        UpdateXMarkers();
    }

    private void UpdateXMarkers()
    {
        double[] x = sim.GetX1X2(true);

        if (x1) x1.localPosition = (float)x[0] * Vector3.right;
        if (x2) x2.localPosition = (float)x[1] * Vector3.right;
        if (xCM) xCM.localPosition = 0.5f * (float)(x[0] + x[1]) * Vector3.right;
        if (xR) xR.localPosition = (float)(x[1] - x[0] - 2 * sim.x0) * Vector3.right;
    }
}
