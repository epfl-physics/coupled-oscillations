// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;

[CreateAssetMenu(fileName = "New Simulation State", menuName = "Simulation State", order = 50)]
public class CoupledOscillationsSimulationState : ScriptableObject
{
    public double[] x;
    public double[] xdot;
}
