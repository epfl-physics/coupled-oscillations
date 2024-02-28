// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
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
