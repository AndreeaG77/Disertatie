using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed for WASD movement
    public float dragSpeed = 0.1f;  // Speed for mouse dragging
    public float zoomSpeed = 5f;  // Speed for zooming
    public float minZoom = 5f;  // Closest zoom-in level
    public float maxZoom = 20f;  // Furthest zoom-out level

    public Vector2 gridMin = new Vector2(-20f, -20f);  // Grid lower-left corner
    public Vector2 gridMax = new Vector2(20f, 20f);  // Grid upper-right corner

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
        HandleKeyboardMovement();
        HandleMouseDragging();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");  
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            transform.position = ClampPosition(transform.position);  // Reapply clamping after zooming
        }
    }

    void HandleKeyboardMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)){
            moveZ += moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + new Vector3(0, 0, moveZ);
            transform.position = ClampPosition(newPosition);
        }
            
        if (Input.GetKey(KeyCode.S)){
            moveZ -= moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + new Vector3(0, 0, moveZ);
            transform.position = ClampPosition(newPosition);
        }
            
        if (Input.GetKey(KeyCode.A)){
            moveX -= moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + new Vector3(moveX, 0, 0);
            transform.position = ClampPosition(newPosition);
        }
            
        if (Input.GetKey(KeyCode.D)){
            moveX += moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + new Vector3(moveX, 0, 0);
            transform.position = ClampPosition(newPosition);
        }

    }

    void HandleMouseDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            float moveX = -mouseDelta.x * dragSpeed;
            float moveZ = -mouseDelta.y * dragSpeed;

            Vector3 newPosition = transform.position + new Vector3(moveX, 0, moveZ);
            transform.position = ClampPosition(newPosition);

            lastMousePosition = Input.mousePosition;
        }
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;  // Aspect ratio ensures correct width scaling

        float minX = gridMin.x + camWidth;
        float maxX = gridMax.x - camWidth;
        float minY = gridMin.y + camHeight;
        float maxY = gridMax.y - camHeight;

        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedZ = Mathf.Clamp(position.z, minY, maxY);

        return new Vector3(clampedX, position.y, clampedZ);
    }
}
