using UnityEngine;

// Template class for a physics simulation that can be controlled by SimulationSlideController
public abstract class Simulation : MonoBehaviour
{
    [HideInInspector] public bool paused = false;

    public virtual void Pause()
    {
        paused = true;
    }

    public virtual void Resume()
    {
        paused = false;
    }

    public virtual void TogglePlayPause()
    {
        //paused = !paused;
        if (paused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
}
