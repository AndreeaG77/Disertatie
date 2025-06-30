using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float dragSpeed = 0.1f; 
    public float zoomSpeed = 5f;  
    public float minZoom = 5f; 
    public float maxZoom = 20f; 
    public Vector2 gridMin = new Vector2(0f, 0f);  // Grid lower-left corner
    public Vector2 gridMax = new Vector2(50f, 40f);  // Grid upper-right corner
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
        HandleKeyboardMovement();
    }

    void HandleZoom()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");  
            if (scroll != 0f)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                //cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
                float maxWidthBased = (gridMax.x - gridMin.x) / (2f * cam.aspect);
                float maxHeightBased = (gridMax.y - gridMin.y) / 2f;
                float dynamicMaxZoom = Mathf.Min(maxWidthBased, maxHeightBased);
                
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, dynamicMaxZoom);

                transform.position = ClampPosition(transform.position); 
        }
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
