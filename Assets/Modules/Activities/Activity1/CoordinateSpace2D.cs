using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoordinateSpace2D : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Extent")]
    [SerializeField] private Vector2 xRange = new Vector2(-1, 1);
    [SerializeField] private Vector2 yRange = new Vector2(-1, 1);
    [SerializeField, Min(0)] float borderWidth = 0;
    private Vector2 uvMax;
    private Vector2 uvMin;

    [Header("Properties")]
    [SerializeField] private RectTransform marker;
    [SerializeField] private bool snapToDiagonals;
    [SerializeField] private float snapTolerance = 0.1f;

    [Header("Simulation")]
    [SerializeField] private CoupledOscillationsSimulation sim;
    [SerializeField] private bool startPaused = true;

    [Header("Dotted Line")]
    [SerializeField] private Sprite dot = default;
    [SerializeField] private float maxDotSpacing = 0.1f;
    [SerializeField] private float dotSize = 0.1f;
    [SerializeField] private Color dotColor = Color.black;
    [SerializeField] private bool drawDottedLines;
    private RectTransform xDots;
    private RectTransform yDots;

    [Header("Normal Modes")]
    [SerializeField] private Toggle mode1;
    [SerializeField] private Toggle mode2;

    [Header("Diagonals")]
    [SerializeField] private RectTransform diagonalMode1;
    [SerializeField] private RectTransform diagonalMode2;

    private RectTransform rect;
    private bool mouseIsDown;

    private Camera eventCamera = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Reset(startPaused);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseIsDown = true;
        eventCamera = eventData.pressEventCamera;
        if (sim) sim.Pause();

        if (drawDottedLines)
        {
            if (xDots != null)
            {
                DestroyImmediate(xDots.gameObject);
                xDots = null;
            }
            if (yDots != null)
            {
                DestroyImmediate(yDots.gameObject);
                yDots = null;
            }

            xDots = CreateDottedLineContainer("X Dots", 0);
            yDots = CreateDottedLineContainer("Y Dots", 1);
        }

        if (mode1) mode1.isOn = false;
        if (mode2) mode2.isOn = false;

        if (diagonalMode1) diagonalMode1.gameObject.SetActive(false);
        if (diagonalMode2) diagonalMode2.gameObject.SetActive(false);

        // Recompute exclusion border in UV space
        uvMax = ScaledToNormalizedPosition(new Vector2(xRange.y - borderWidth, yRange.y - borderWidth));
        uvMin = ScaledToNormalizedPosition(new Vector2(xRange.x + borderWidth, yRange.x + borderWidth));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseIsDown = false;
        eventCamera = null;
        if (sim)
        {
            Vector2 uv = ClampUV(ScreenToNormalizedPosition(Input.mousePosition, eventCamera));
            if (snapToDiagonals)
            {
                uv = SnapToDiagonalUV(uv, snapTolerance);
            }
            Vector2 scaledPosition = NormalizedToScaledPosition(uv);
            sim.UpdateX(scaledPosition.x, scaledPosition.y);
            sim.UpdateXDot();
            sim.UpdateMassPositions();
            sim.UpdateSpringPositions();
            sim.Resume();

            if (mode1 != null && CheckForMode1(uv, 0.0000001f))
            {
                mode1.isOn = true;
                if (diagonalMode1) diagonalMode1.gameObject.SetActive(true);
            }

            if (mode2 && CheckForMode2(uv, 0.0000001f))
            {
                mode2.isOn = true;
                if (diagonalMode2) diagonalMode2.gameObject.SetActive(true);
            }
        }

        if (drawDottedLines)
        {
            DestroyImmediate(xDots.gameObject);
            xDots = null;
            DestroyImmediate(yDots.gameObject);
            yDots = null;
        }
    }

    private void Update()
    {
        if (mouseIsDown)
        {
            // Compute normalized (UV) coordinates of the mouse within this UI element
            Vector2 uv = ClampUV(ScreenToNormalizedPosition(Input.mousePosition, eventCamera));

            if (marker)
            {
                if (snapToDiagonals)
                {
                    uv = SnapToDiagonalUV(uv, snapTolerance);
                }
                marker.anchoredPosition = NormalizedToRectPosition(uv);
            }

            if (diagonalMode1) diagonalMode1.gameObject.SetActive(CheckForMode1(uv, 0.0000001f));
            if (diagonalMode2) diagonalMode2.gameObject.SetActive(CheckForMode2(uv, 0.0000001f));

            if (sim)
            {
                Vector2 scaledPosition = NormalizedToScaledPosition(uv);
                sim.UpdateX(scaledPosition.x, scaledPosition.y);
                sim.UpdateMassPositions();
                sim.UpdateSpringPositions();
            }

            if (drawDottedLines)
            {
                ClearChildren(xDots);
                ClearChildren(yDots);

                DrawDottedLine(0, uv, 0.5f * Vector2.one, xDots);
                DrawDottedLine(1, uv, 0.5f * Vector2.one, yDots);
            }
        }

        if (sim)
        {
            if (sim.IsPaused || !marker) { return; }

            double[] x = sim.GetX1X2();
            SetMarkerPositionFromX1X2(x[0], x[1]);
            // marker.anchoredPosition = ScaledToRectPosition(new Vector2((float)x[0], (float)x[1]));
        }
    }

    public void Reset(bool pause)
    {
        if (sim && marker)
        {
            sim.Reset(pause);
            double[] x = sim.GetX1X2();
            SetMarkerPositionFromX1X2(x[0], x[1]);
            // marker.anchoredPosition = ScaledToRectPosition(new Vector2((float)x[0], (float)x[1]));
        }

        if (diagonalMode1) diagonalMode1.gameObject.SetActive(false);
        if (diagonalMode2) diagonalMode2.gameObject.SetActive(false);
    }

    public void SetMarkerPositionFromX1X2(double x1, double x2)
    {
        if (marker)
        {
            marker.anchoredPosition = ScaledToRectPosition(new Vector2((float)x1, (float)x2));
        }
    }

    private Vector2 ScreenToNormalizedPosition(Vector2 position, Camera camera)
    {
        Vector2 normalizedPosition = default;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, position, camera, out var localPosition))
        {
            normalizedPosition = Rect.PointToNormalized(rect.rect, localPosition);
        }

        return normalizedPosition;
    }

    private Vector2 SnapToDiagonalUV(Vector2 uv, float tol)
    {
        Vector2 xy = 2 * uv - Vector2.one;
        float difference = Mathf.Abs(xy.x) - Mathf.Abs(xy.y);
        if (Mathf.Abs(difference) < tol)
        {
            float value = 0.5f * (Mathf.Abs(xy.x) + Mathf.Abs(xy.y));
            xy.x = Mathf.Sign(xy.x) * value;
            xy.y = Mathf.Sign(xy.y) * value;
        }
        return 0.5f * (xy + Vector2.one);
    }

    private Vector2 NormalizedToScaledPosition(Vector2 uv)
    {
        float x = (xRange.y - xRange.x) * uv.x + xRange.x;
        float y = (yRange.y - yRange.x) * uv.y + yRange.x;
        return new Vector2(x, y);
    }

    private Vector2 ScaledToNormalizedPosition(Vector2 scaledPosition)
    {
        float u = (scaledPosition.x - xRange.x) / (xRange.y - xRange.x);
        float v = (scaledPosition.y - yRange.x) / (yRange.y - yRange.x);
        return new Vector2(u, v);
    }

    private Vector2 NormalizedToRectPosition(Vector2 uv)
    {
        return (uv - 0.5f * Vector2.one) * rect.rect.size;
    }

    private Vector2 RectToNormalizedPosition(Vector2 rectPosition)
    {
        return (rectPosition / rect.rect.size) + 0.5f * Vector2.one;
    }

    private Vector2 ScaledToRectPosition(Vector2 scaledPosition)
    {
        Vector2 uv = ScaledToNormalizedPosition(scaledPosition);
        return NormalizedToRectPosition(uv);
    }

    private void ClearChildren(RectTransform rt)
    {
        for (int i = rt.childCount; i > 0; i--)
        {
            DestroyImmediate(rt.GetChild(0).gameObject);
        }
    }

    private RectTransform CreateDottedLineContainer(string name, int siblingIndex)
    {
        var container = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        container.SetParent(transform);
        container.anchoredPosition = Vector2.zero;
        container.localScale = Vector3.one;
        container.SetSiblingIndex(siblingIndex);
        return container;
    }

    private void DrawDottedLine(int axis, Vector2 uv, Vector2 offset, RectTransform parent)
    {
        Vector2 delta = uv - offset;
        float height = axis == 0 ? delta.x : delta.y;
        float sign = Mathf.Sign(height);
        int numDots = Mathf.CeilToInt(Mathf.Abs(height) / maxDotSpacing);
        float spacing = height / numDots;
        for (int i = 0; i < numDots; i++)
        {
            RectTransform dotRT = new GameObject("Dot", typeof(Image)).GetComponent<RectTransform>();
            dotRT.SetParent(parent);
            dotRT.localScale = dotSize * Vector3.one;
            Vector2 position = axis == 0 ? new Vector2(offset.y + i * spacing, uv.y) : new Vector2(uv.x, offset.x + i * spacing);
            dotRT.anchoredPosition = NormalizedToRectPosition(position);
            Image image = dotRT.GetComponent<Image>();
            image.sprite = dot;
            image.raycastTarget = false;
            image.color = dotColor;
            image.preserveAspect = true;
            image.SetNativeSize();
        }
    }

    private bool CheckForMode1(Vector2 uv, float tol = 0)
    {
        return Mathf.Abs(uv.y - uv.x) <= tol;
    }

    private bool CheckForMode2(Vector2 uv, float tol = 0)
    {
        uv -= 0.5f * Vector2.one;
        return Mathf.Abs(uv.y + uv.x) <= tol && (uv.x != uv.y);
    }

    private Vector2 ClampUV(Vector2 uv)
    {
        // Restrict to within borderWidth of the edges
        uv.x = Mathf.Max(Mathf.Min(uv.x, uvMax.x), uvMin.x);
        uv.y = Mathf.Max(Mathf.Min(uv.y, uvMax.y), uvMin.y);
        return uv;
    }
}
