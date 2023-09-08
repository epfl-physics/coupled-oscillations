using UnityEngine;

public class TheoryController : MonoBehaviour
{
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private Transform x1;
    [SerializeField] private Transform x2;
    [SerializeField] private Transform xCM;
    [SerializeField] private Transform xR;

    [Header("References")]
    [SerializeField] private LengthScale lsLeftRef;
    [SerializeField] private LengthScale lsCenterRef;
    [SerializeField] private LengthScale lsRightRef;

    [Header("Length Scales")]
    [SerializeField] private LengthScale lsLeft;
    [SerializeField] private LengthScale lsCenter;
    [SerializeField] private LengthScale lsRight;

    [Header("Vectors")]
    [SerializeField] private float scaleFactor = 1;
    [SerializeField] private Arrow force11;  // Force of spring 1 on mass 1
    [SerializeField] private Arrow force21;  // Force of spring 2 on mass 1
    [SerializeField] private Arrow force22;
    [SerializeField] private Arrow force32;

    [Header("Graphs")]
    [SerializeField] private DynamicGraph graph;
    [SerializeField, Min(0)] private float graphDeltaTime = 0.1f;
    [SerializeField] private Color x1Color = Color.black;
    [SerializeField] private Color x2Color = Color.black;
    [SerializeField] private Color cmColor = Color.black;
    [SerializeField] private Color xRColor = Color.black;
    private float graphTime;
    private float elapsedTime;

    private void Start()
    {
        if (sim)
        {
            SetReferenceLengthScales();
            UpdateVectors();
            sim.Pause();

            if (graph)
            {
                graph.CreateLine(xRColor, "XR");
                graph.CreateLine(x1Color, "X1");
                graph.CreateLine(x2Color, "X2");
                graph.CreateLine(cmColor, "CM");
            }
        }
    }

    private void Update()
    {
        if (!sim) return;

        // TODO don't update if sim is paused and user isn't dragging one of the masses
        UpdateXMarkers();
        UpdateLengthScales();
        UpdateVectors();
        UpdateGraph();
    }

    private void UpdateXMarkers()
    {
        double[] x = sim.GetX1X2(true);

        if (x1) x1.localPosition = (float)x[0] * Vector3.right;
        if (x2) x2.localPosition = (float)x[1] * Vector3.right;
        if (xCM) xCM.localPosition = 0.5f * (float)(x[0] + x[1]) * Vector3.right;
        if (xR) xR.localPosition = (float)(x[1] - x[0] - 2 * sim.x0) * Vector3.right;
    }

    private void SetReferenceLengthScales()
    {
        if (lsLeftRef)
        {
            float[] endpoints = sim.GetSpring1Endpoints();
            lsLeftRef.SetXPositions(endpoints);
        }
        if (lsCenterRef) lsCenterRef.SetXPositions(sim.GetSpring2Endpoints());
        if (lsRightRef) lsRightRef.SetXPositions(sim.GetSpring3Endpoints());
    }

    private void UpdateLengthScales()
    {
        if (lsLeft) lsLeft.SetXPositions(sim.GetSpring1Endpoints());
        if (lsCenter) lsCenter.SetXPositions(sim.GetSpring2Endpoints());
        if (lsRight) lsRight.SetXPositions(sim.GetSpring3Endpoints());
    }

    private void UpdateVectors()
    {
        if (force11)
        {
            float endpoint = sim.GetSpring1Endpoints()[1];
            Vector3 position = force11.transform.position;
            position.x = endpoint;
            force11.transform.position = position;
            float force = -scaleFactor * sim.GetK1() * (endpoint - lsLeftRef.GetXPositionRight());
            force11.SetComponents(force * Vector3.right);
        }

        if (force21)
        {
            float[] endpoints = sim.GetSpring2Endpoints();
            Vector3 position = force21.transform.position;
            position.x = endpoints[0];
            force21.transform.position = position;
            float delta = (endpoints[1] - endpoints[0]) - (lsCenterRef.GetXPositionRight() - lsCenterRef.GetXPositionLeft());
            float force = scaleFactor * sim.GetK2() * delta;
            force21.SetComponents(force * Vector3.right);
        }

        if (force22)
        {
            float[] endpoints = sim.GetSpring2Endpoints();
            Vector3 position = force22.transform.position;
            position.x = endpoints[1];
            force22.transform.position = position;
            float delta = (endpoints[1] - endpoints[0]) - (lsCenterRef.GetXPositionRight() - lsCenterRef.GetXPositionLeft());
            float force = -scaleFactor * sim.GetK2() * delta;
            force22.SetComponents(force * Vector3.right);
        }

        if (force32)
        {
            float endpoint = sim.GetSpring3Endpoints()[0];
            Vector3 position = force32.transform.position;
            position.x = endpoint;
            force32.transform.position = position;
            float force = -scaleFactor * sim.GetK3() * (endpoint - lsRightRef.GetXPositionLeft());
            force32.SetComponents(force * Vector3.right);
        }
    }

    private void UpdateGraph()
    {
        if (!graph) return;

        if (!sim.IsPaused) elapsedTime += Time.deltaTime;

        graphTime += Time.deltaTime;

        if (graphTime >= graphDeltaTime)
        {
            double[] x = sim.GetX1X2(true);

            Vector2 newX1 = new Vector2(elapsedTime, (float)x[0]);
            Vector2 newX2 = new Vector2(elapsedTime, (float)x[1]);
            Vector2 newCM = new Vector2(elapsedTime, 0.5f * (float)(x[0] + x[1]));
            Vector2 newXR = new Vector2(elapsedTime, (float)(x[1] - x[0] - 2 * sim.x0));

            graph.PlotPoint(0, newXR);
            graph.PlotPoint(1, newX1);
            graph.PlotPoint(2, newX2);
            graph.PlotPoint(3, newCM);

            graphTime = 0;
        }
    }

    public void ResetTime()
    {
        elapsedTime = 0;
    }

    public void SetXRPlotVisibility(bool visible)
    {
        graph.SetLineVisibility(0, visible);
    }

    public void SetX1PlotVisibility(bool visible)
    {
        graph.SetLineVisibility(1, visible);
    }

    public void SetX2PlotVisibility(bool visible)
    {
        graph.SetLineVisibility(2, visible);
    }

    public void SetCMPlotVisibility(bool visible)
    {
        graph.SetLineVisibility(3, visible);
    }
}
