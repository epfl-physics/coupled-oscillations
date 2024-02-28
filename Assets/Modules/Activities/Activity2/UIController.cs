// ----------------------------------------------------------------------------------------------------------
// Author: Austin Peel
//
// © All rights reserved. ECOLE POLYTECHNIQUE FEDERALE DE LAUSANNE, Switzerland, Section de Physique, 2024.
// See the LICENSE.md file for more details.
// ----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private float amplitude = 1f;
    [SerializeField, Min(0)] private int numDecimalDigits = 1;
    [SerializeField] private bool pauseOnSliderChange = false;

    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private Toggle periodToggle;

    private int currentMode = 1;

    private void Start()
    {
        if (toggleGroup && sim) sim.EnterNormalMode(true, amplitude);
        if (periodToggle) HandlePeriodToggleChange(periodToggle.isOn);
    }

    public void HandleModeToggle()
    {
        if (toggleGroup)
        {
            Toggle toggle = toggleGroup.GetFirstActiveToggle();
            if (!toggle.name.Contains(currentMode.ToString()))
            {
                currentMode = currentMode == 1 ? 2 : 1;
                if (sim) sim.EnterNormalMode(currentMode == 1, amplitude);
                if (periodToggle) HandlePeriodToggleChange(periodToggle.isOn);
            }
        }
    }

    public void HandleMassChange(float mass)
    {
        if (sim)
        {
            sim.SetMass(RoundToDecimalPlace(mass, numDecimalDigits), pauseOnSliderChange);
            sim.UpdateMassPositions();
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }

        if (periodToggle) HandlePeriodToggleChange(periodToggle.isOn);
    }

    public void HandleK1Change(float k1)
    {
        if (sim)
        {
            sim.SetK1(RoundToDecimalPlace(k1, numDecimalDigits), pauseOnSliderChange);
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }

        if (periodToggle) HandlePeriodToggleChange(periodToggle.isOn);
    }

    public void HandleK2Change(float k2)
    {
        if (sim)
        {
            sim.SetK2(RoundToDecimalPlace(k2, numDecimalDigits), pauseOnSliderChange);
            if (toggleGroup) sim.EnterNormalMode(currentMode == 1, amplitude);
        }

        if (periodToggle) HandlePeriodToggleChange(periodToggle.isOn);
    }

    private float RoundToDecimalPlace(float value, int decimalPlace)
    {
        float factor = Mathf.Pow(10f, decimalPlace);
        return Mathf.Round(factor * value) / factor;
    }

    public void HandlePeriodToggleChange(bool stopAfterOnePeriod)
    {
        if (!timer) return;

        if (sim && stopAfterOnePeriod)
        {
            float[] periods = sim.GetNormalModePeriods();
            periods[0] = Mathf.Clamp(periods[0], 0, 99.99f);
            periods[1] = Mathf.Clamp(periods[1], 0, 99.99f);
            timer.SetMaxTime(currentMode == 1 ? periods[0] : periods[1]);
        }
        else
        {
            timer.SetMaxTime(99.99f);
        }
    }
}
