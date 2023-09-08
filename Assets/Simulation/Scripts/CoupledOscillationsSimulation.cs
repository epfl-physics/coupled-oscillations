using UnityEngine;

public class CoupledOscillationsSimulation : Simulation
{
    [Header("Objects")]
    [SerializeField] private MassiveObject mass1;
    [SerializeField] private MassiveObject mass2;
    [SerializeField] private Spring spring1;
    [SerializeField] private Spring spring2;
    [SerializeField] private Spring spring3;
    [SerializeField] private Transform wall1;
    [SerializeField] private Transform wall2;

    [Header("Parameters")]
    [SerializeField] private double x1Init = -2;
    [SerializeField] private double x2Init = 2;
    [SerializeField] private double x1Ref = -2;
    [SerializeField] private double x2Ref = 2;
    [SerializeField] private float x1Wall = -5;
    [SerializeField] private float x2Wall = 5;
    [SerializeField] private CoupledOscillationsSimulationState simState;

    [Header("Interactivity")]
    [SerializeField] private bool massesAreDraggable;
    [SerializeField] private Vector2 x1Range = Vector2.left;
    [SerializeField] private Vector2 x2Range = Vector2.right;
    private LayerMask massLayerMask;
    private Plane plane;
    private Camera mainCamera;
    private Transform dragMass = null;
    private Vector3 dragOffset = Vector3.zero;

    [Header("Buttons")]
    [SerializeField] private PlayButton playButton;

    [Header("UI")]
    [SerializeField] private CoordinateSpace2D coordinateSpace;

    private double[] x;
    private double[] xdot;
    private double[][] constants;

    private bool resumeOnMouseUp = false;

    public float x0 => (float)x2Ref;

    private void Awake()
    {
        // Initialize the constants matrix and compute its components
        InitializeCouplingConstants();

        // Initialize equations of motion arrays
        x = new double[4] { x1Init - x1Ref, x2Init - x2Ref, 0, 0 };
        double[] a = ComputeAccelerations();
        xdot = new double[4] { 0, 0, a[0], a[1] };

        // Initialize object positions
        SetWallPositions();
        UpdateMassPositions();
        UpdateSpringPositions();

        // For raycasting and dragging masses
        plane = new Plane(Vector3.forward, Vector3.zero);
        mainCamera = Camera.main;
        massLayerMask = LayerMask.GetMask("Masses");
    }

    private void OnEnable()
    {
        // Use ICs from simulation state if available
        if (simState)
        {
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = simState.x[i];
                xdot[i] = simState.xdot[i];
            }
        }
    }

    private void OnDisable()
    {
        // Pass current state to sims in other scenes
        if (simState)
        {
            for (int i = 0; i < x.Length; i++)
            {
                simState.x[i] = x[i];
                simState.xdot[i] = xdot[i];
            }
        }
    }

    // Handle mouse clicks and dragging masses
    private void Update()
    {
        if (!massesAreDraggable) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            float maxDistance = 2 * Mathf.Abs(mainCamera.transform.position.z);
            if (Physics.Raycast(ray, out hitInfo, maxDistance, massLayerMask))
            {
                dragMass = hitInfo.transform;
                dragOffset = hitInfo.point - dragMass.position;
                resumeOnMouseUp = !IsPaused;
                Pause();
            }
        }

        if (dragMass != null && Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 position = dragMass.position;
                position.x = ray.GetPoint(distance).x;
                if (dragMass.name.Contains("1"))
                {
                    position.x = Mathf.Min(Mathf.Max(position.x, x1Range.x), x1Range.y);
                    UpdateX(position.x - (float)x1Ref, (float)x[1]);
                }
                else if (dragMass.name.Contains("2"))
                {
                    position.x = Mathf.Min(Mathf.Max(position.x, x2Range.x), x2Range.y);
                    UpdateX((float)x[0], position.x - (float)x2Ref);
                }
                dragMass.position = position;
                UpdateSpringPositions();

                if (coordinateSpace) coordinateSpace.SetMarkerPositionFromX1X2(x[0], x[1]);
            }
        }

        if (dragMass != null && Input.GetMouseButtonUp(0))
        {
            if (dragMass.name.Contains("1"))
            {
                UpdateX(dragMass.position.x - (float)x1Ref, (float)x[1]);
            }
            else if (dragMass.name.Contains("2"))
            {
                UpdateX((float)x[0], dragMass.position.x - (float)x2Ref);
            }
            UpdateXDot();
            UpdateSpringPositions();

            dragMass = null;
            dragOffset = Vector3.zero;

            if (coordinateSpace) coordinateSpace.SetMarkerPositionFromX1X2(x[0], x[1]);
            if (resumeOnMouseUp) Resume();
        }
    }

    // Solve the equations of motion
    private void FixedUpdate()
    {
        if (IsPaused) { return; }

        int numSubsteps = 10;
        double deltaTime = Time.fixedDeltaTime / numSubsteps;
        for (int i = 0; i < numSubsteps; i++)
        {
            TakeLeapfrogStep(deltaTime);
        }

        UpdateMassPositions();
        UpdateSpringPositions();
    }

    public void UpdateMassPositions()
    {
        if (mass1) mass1.transform.position = (float)(x1Ref + x[0]) * Vector3.right;
        if (mass2) mass2.transform.position = (float)(x2Ref + x[1]) * Vector3.right;
    }

    private void SetWallPositions()
    {
        if (wall1) wall1.position = x1Wall * Vector3.right;
        if (wall2) wall2.position = x2Wall * Vector3.right;
    }

    public void UpdateSpringPositions()
    {
        float x1 = (float)(x[0] + x1Ref);
        float x2 = (float)(x[1] + x2Ref);

        float offsetWall1 = wall1 ? 0.5f * wall1.localScale.x : 0;
        float offsetWall2 = wall2 ? 0.5f * wall2.localScale.x : 0;
        float offsetMass1 = mass1 ? mass1.HalfScale : 0;
        float offsetMass2 = mass2 ? mass2.HalfScale : 0;

        if (spring1) spring1.SetEndpoints((x1Wall + offsetWall1) * Vector3.right, (x1 - offsetMass1) * Vector3.right);
        if (spring2) spring2.SetEndpoints((x1 + offsetMass1) * Vector3.right, (x2 - offsetMass2) * Vector3.right);
        if (spring3) spring3.SetEndpoints((x2 + offsetMass2) * Vector3.right, (x2Wall - offsetWall2) * Vector3.right);
    }

    private double[] ComputeAccelerations()
    {
        double[] a = new double[2];
        a[0] = constants[0][0] * x[0] + constants[0][1] * (x[1] - x[0]);
        a[1] = constants[1][0] * x[1] + constants[1][1] * (x[0] - x[1]);
        return a;
    }

    private void InitializeCouplingConstants()
    {
        constants = new double[2][];
        constants[0] = new double[2] { 1, 0 };
        constants[1] = new double[2] { 0, 1 };
        UpdateCouplingConstants();
    }

    private void UpdateCouplingConstants()
    {
        // TODO generalize for all 3 springs having different constants
        if (spring1 && spring2 && mass1 && mass2)
        {
            constants[0][0] = -spring1.k / mass1.mass;
            constants[0][1] = spring2.k / mass1.mass;
            constants[1][0] = -spring1.k / mass2.mass;
            constants[1][1] = spring2.k / mass2.mass;
        }
    }

    private void TakeLeapfrogStep(double deltaTime)
    {
        // Update positions with current velocities and accelerations
        x[0] += deltaTime * (xdot[0] + 0.5 * xdot[2] * deltaTime);
        x[1] += deltaTime * (xdot[1] + 0.5 * xdot[3] * deltaTime);

        // Compute new accelerations and update velocities
        double[] aNew = ComputeAccelerations();
        x[2] += 0.5 * (xdot[2] + aNew[0]) * deltaTime;
        x[3] += 0.5 * (xdot[3] + aNew[1]) * deltaTime;

        // Update accelerations
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = aNew[0];
        xdot[3] = aNew[1];
    }

    public void UpdateX(float x1, float x2)
    {
        x[0] = x1;
        x[1] = x2;
        x[2] = 0;
        x[3] = 0;
    }

    public void UpdateXDot()
    {
        double[] a = ComputeAccelerations();
        xdot[0] = x[2];
        xdot[1] = x[3];
        xdot[2] = a[0];
        xdot[3] = a[1];
    }

    public double[] GetX1X2(bool absolute = false)
    {
        double[] x1x2 = new double[2] { x[0], x[1] };
        if (absolute)
        {
            x1x2[0] += x1Ref;
            x1x2[1] += x2Ref;
        }
        return x1x2;
    }

    public float[] GetSpring1Endpoints()
    {
        float offsetWall1 = wall1 ? 0.5f * wall1.localScale.x : 0;
        // float offsetMass1 = mass1 ? mass1.HalfScale : 0;
        float offsetMass1 = 0;
        return new float[2] { x1Wall + offsetWall1, (float)(x[0] + x1Ref) - offsetMass1 };
    }

    public float[] GetSpring2Endpoints()
    {
        // float offsetMass1 = mass1 ? mass1.HalfScale : 0;
        // float offsetMass2 = mass2 ? mass2.HalfScale : 0;
        float offsetMass1 = 0;
        float offsetMass2 = 0;
        return new float[2] { (float)(x[0] + x1Ref) + offsetMass1, (float)(x[1] + x2Ref) - offsetMass2 };
    }

    public float[] GetSpring3Endpoints()
    {
        float offsetWall2 = wall2 ? 0.5f * wall2.localScale.x : 0;
        // float offsetMass2 = mass2 ? mass2.HalfScale : 0;
        float offsetMass2 = 0;
        return new float[2] { (float)(x[1] + x2Ref) + offsetMass2, x2Wall - offsetWall2 };
    }

    public float GetK1()
    {
        return spring1.k;
    }

    public float GetK2()
    {
        return spring2.k;
    }

    public float GetK3()
    {
        return spring3.k;
    }

    public float[] GetNormalModePeriods()
    {
        float t1 = 2 * Mathf.PI * Mathf.Sqrt(mass1.mass / spring1.k);
        float t2 = 2 * Mathf.PI * Mathf.Sqrt(mass1.mass / (spring1.k + 2 * spring2.k));
        return new float[2] { t1, t2 };
    }

    public void EnterNormalMode(bool first, float amplitude)
    {
        float sign = first ? 1 : -1;
        UpdateX(-amplitude, -sign * amplitude);
        UpdateXDot();
        UpdateMassPositions();
        UpdateSpringPositions();
    }

    public void SetMass(float value, bool startFromRest = false)
    {
        if (mass1 && mass2)
        {
            mass1.SetMass(value);
            mass2.SetMass(value);
            UpdateCouplingConstants();
            if (IsPaused) UpdateSpringPositions();
            if (startFromRest)
            {
                UpdateX((float)x[0], (float)x[1]);
                UpdateXDot();
            }
        }
    }

    public void SetK1(float value, bool startFromRest = false)
    {
        if (spring1 && spring3)
        {
            spring1.k = value;
            spring3.k = value;
            UpdateCouplingConstants();
            if (IsPaused) UpdateSpringPositions();
            if (startFromRest)
            {
                UpdateX((float)x[0], (float)x[1]);
                UpdateXDot();
            }
        }
    }

    public void SetK2(float value, bool startFromRest = false)
    {
        if (spring2)
        {
            spring2.k = value;
            UpdateCouplingConstants();
            if (IsPaused) UpdateSpringPositions();
            if (startFromRest)
            {
                UpdateX((float)x[0], (float)x[1]);
                UpdateXDot();
            }
        }
    }

    public void SetMassInteractivity(bool interactive)
    {
        massesAreDraggable = interactive;
    }

    private double ComputeEnergy()
    {
        double KE = mass1.mass * x[2] * x[2] + mass2.mass * x[3] * x[3];
        double PE = spring1.k * (x[0] * x[0] + x[1] * x[1]) + spring2.k * (x[1] - x[0]) * (x[1] - x[0]);
        return 0.5 * (KE + PE);
    }

    public void Reset(bool pause)
    {
        // Initialize the constants matrix and compute its components
        InitializeCouplingConstants();

        // Initialize equations of motion arrays
        x = new double[4] { x1Init - x1Ref, x2Init - x2Ref, 0, 0 };
        double[] a = ComputeAccelerations();
        xdot = new double[4] { 0, 0, a[0], a[1] };

        // Initialize object positions
        SetWallPositions();
        UpdateMassPositions();
        UpdateSpringPositions();

        if (pause) Pause();
    }

    public override void Pause()
    {
        base.Pause();
        if (playButton) playButton.Pause();
    }

    public override void Resume()
    {
        base.Resume();
        if (playButton) playButton.Play();
    }
}
