using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Color backgroundColor = Color.white;
    [SerializeField] private float colorTransitionTime = 0.5f;

    [Header("Orientation")]
    [SerializeField] private Vector3 position = new Vector3(0, 0, -100);
    [SerializeField] private Vector3 lookAt = new Vector3(0, 0, 0);
    [SerializeField] private float moveTime = 1f;

    [Header("Controls")]
    [SerializeField] private bool orthographic;
    [SerializeField] private float fieldOfView = 20;
    [SerializeField] private Vector3 pivotPoint = Vector3.zero;
    [SerializeField] private bool canRotateX;
    [SerializeField] private bool canRotateY;
    [SerializeField] private bool canZoom;
    [SerializeField] private float scrollScaleFactor = 0.1f;
    [SerializeField] private float minZoom = 5;
    [SerializeField] private float maxZoom = 50;

    [SerializeField] private AnimationCurve rotationCurve;

    private Camera mainCamera;
    private Coroutine cameraMoving;
    private Coroutine cameraChangingColor;

    private bool clickedOnUIElement;
    private Vector3 mouseStartPosition;
    private Vector3 cameraStartPosition;
    private Quaternion cameraStartRotation;

    public void AssignCameraReference(Camera camera)
    {
        //Debug.Log("AssignCameraReference called by " + gameObject.name);
        mainCamera = camera;
    }

    public void ReleaseCameraReference()
    {
        //Debug.Log("ReleaseCameraReference called by " + gameObject.name);
        if (cameraMoving != null)
        {
            StopCoroutine(cameraMoving);
        }
        if (cameraChangingColor != null)
        {
            StopCoroutine(cameraChangingColor);
        }
        mainCamera = null;
    }

    // For zooming in and out
    private void OnGUI()
    {
        // Do nothing if no mainCamera reference is assigned or any coroutines are still running
        if (!canZoom || mainCamera == null || cameraMoving != null)
        {
            return;
        }

        if (!mainCamera.orthographic)
        {
            Vector3 position = mainCamera.transform.position - pivotPoint;
            position += -Input.mouseScrollDelta.y * scrollScaleFactor * position.normalized;
            if (position.magnitude < minZoom)
            {
                position = minZoom * position.normalized;
            }
            if (position.magnitude > maxZoom)
            {
                position = maxZoom * position.normalized;
            }
            mainCamera.transform.position = position + pivotPoint;
        }
        else
        {
            float size = mainCamera.orthographicSize - Input.mouseScrollDelta.y * scrollScaleFactor;
            size = Mathf.Min(Mathf.Max(size, minZoom), maxZoom);
            mainCamera.orthographicSize = size;
        }
    }

    private void Update()
    {
        if (mainCamera == null || (!canRotateX && !canRotateY) || cameraMoving != null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPosition = Input.mousePosition;
            cameraStartPosition = mainCamera.transform.position - pivotPoint;
            cameraStartRotation = mainCamera.transform.rotation;
            clickedOnUIElement = EventSystem.current.IsPointerOverGameObject();
        }

        if (Input.GetMouseButton(0) && !clickedOnUIElement)
        {
            Vector3 axis = Vector3.zero;
            if (canRotateX)
            {
                float deltaX = 4 * mainCamera.ScreenToViewportPoint(Input.mousePosition - mouseStartPosition).x;
                axis = Mathf.Rad2Deg * deltaX * Vector3.up;
            }

            if (canRotateY)
            {
                float deltaY = 4 * mainCamera.ScreenToViewportPoint(Input.mousePosition - mouseStartPosition).y;
                axis += Mathf.Rad2Deg * deltaY * Vector3.left;
            }

            Vector3 newPosition = Quaternion.Euler(axis) * cameraStartPosition + pivotPoint;
            Quaternion newRotation = Quaternion.Euler(axis) * cameraStartRotation;
            mainCamera.transform.SetPositionAndRotation(newPosition, newRotation);
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickedOnUIElement = false;
        }
    }

    public void SetPivotPoint(Vector3 position)
    {
        pivotPoint = position;
    }

    public void InitializeCamera()
    {
        if (!CompareRGB(mainCamera.backgroundColor, backgroundColor))
        {
            cameraChangingColor = StartCoroutine(LerpBackgroundColor(backgroundColor, colorTransitionTime));
        }

        // Let SlideManager know the slide's background color to trigger LanguageToggle
        SendMessageUpwards("HandleThemeChange", backgroundColor, SendMessageOptions.DontRequireReceiver);

        // Always put the camera in perspective mode when moving
        mainCamera.orthographic = false;

        Quaternion targetRotation = Quaternion.LookRotation(lookAt - position);
        if (mainCamera.transform.position != position || mainCamera.transform.rotation != targetRotation || mainCamera.fieldOfView != fieldOfView)
        {
            cameraMoving = StartCoroutine(MoveToPosition(mainCamera.transform.position, position, mainCamera.transform.rotation,
                targetRotation, mainCamera.fieldOfView, fieldOfView, moveTime));
        }

        cameraStartPosition = mainCamera.transform.position;
        cameraStartRotation = mainCamera.transform.rotation;
    }

    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 targetPosition, Quaternion startRotation,
        Quaternion targetRotation, float startFOV, float targetFOV, float slideTime)
    {
        float time = 0;

        while (time < slideTime)
        {
            time += Time.deltaTime;
            float t = time / slideTime;
            t = t * t * (3f - 2f * t);  // Apply some smoothing
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationCurve.Evaluate(time / slideTime));
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.LookAt(lookAt);
        mainCamera.fieldOfView = targetFOV;
        if (orthographic)
        {
            float angle = targetFOV * Mathf.Deg2Rad / 2f;
            mainCamera.orthographicSize = Mathf.Tan(angle) * (mainCamera.transform.position - pivotPoint).magnitude;
            mainCamera.orthographic = true;
        }

        cameraMoving = null;
    }

    private IEnumerator LerpBackgroundColor(Color targetColor, float lerpTime)
    {
        float time = 0;
        Color startColor = mainCamera.backgroundColor;

        while (time < lerpTime)
        {
            time += Time.deltaTime;
            mainCamera.backgroundColor = Color.Lerp(startColor, targetColor, time / lerpTime);
            yield return null;
        }

        mainCamera.backgroundColor = targetColor;
    }

    private bool CompareRGB(Color color1, Color color2)
    {
        return (color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b);
    }
}