using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour
{
    public enum DragPlane { XY }
    public DragPlane dragPlane = DragPlane.XY;

    public bool draggable = true;
    public GameEvent onStartDrag;
    public GameEvent onDragging;
    public GameEvent onEndDrag;

    private Camera mainCamera;
    private bool clickedOnUIElement;
    private new Collider collider;
    private Vector3 startPosition;
    private Vector2 viewportClickedPosition;
    private Vector2 visibleWorldXY;
    private bool dragging;

    private void Awake()
    {
        // Get reference to the camera for raycasting
        mainCamera = Camera.main;

        TryGetComponent(out collider);
    }

    private void Update()
    {
        if (!draggable || !collider)
        {
            return;
        }

        // Initial mouse click on the moon
        if (Input.GetMouseButtonDown(0))
        {
            clickedOnUIElement = EventSystem.current.IsPointerOverGameObject();

            if (clickedOnUIElement)
            {
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (collider.Raycast(ray, out _, 1000f))
            {
                startPosition = transform.position;
                viewportClickedPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
                float height = Mathf.Tan(mainCamera.fieldOfView * Mathf.Deg2Rad) * Mathf.Abs(mainCamera.transform.position.z);
                float width = height * mainCamera.aspect;
                visibleWorldXY = new Vector2(width, height);
                dragging = true;

                if (onStartDrag)
                {
                    onStartDrag.Raise();
                }
            }
        }

        // Hitting while dragging
        if (Input.GetMouseButton(0) && dragging && !clickedOnUIElement)
        {
            // Convert Viewport distance to distance along the world space x- and y-axis
            // Constrain dragging to within 15% of the border
            Vector2 viewportPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            float viewportX = Mathf.Clamp(viewportPosition.x, 0.15f, 0.85f);
            float viewportY = Mathf.Clamp(viewportPosition.y, 0.15f, 0.85f);
            viewportPosition = new Vector2(viewportX, viewportY);
            Vector2 worldDelta = (viewportPosition - viewportClickedPosition) * visibleWorldXY;
            Vector2 newPosition = startPosition + new Vector3(worldDelta.x, worldDelta.y, 0);
            transform.position = newPosition;

            if (onDragging)
            {
                onDragging.Raise();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickedOnUIElement = false;
            dragging = false;

            if (onEndDrag)
            {
                onEndDrag.Raise();
            }
        }
    }
}
