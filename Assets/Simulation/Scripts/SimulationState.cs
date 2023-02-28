using UnityEngine;

[CreateAssetMenu(fileName = "New Simulation State", menuName = "Simulation State", order = 50)]
public class SimulationState : ScriptableObject
{
    public double[] x;
    public double[] xdot;
}
