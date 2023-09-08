using TMPro;
using UnityEngine;

public class CoupledOscillationsSlideController : Slides.SimulationSlideController
{
    private CoupledOscillationsSimulation sim;

    public override void InitializeSlide()
    {
        sim = simulation as CoupledOscillationsSimulation;
        // base.InitializeSlide();
    }
}
