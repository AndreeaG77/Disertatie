using UnityEngine;
using UnityEngine.EventSystems;

public class Camera3DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; // cursorul rămâne în fereastră
        Cursor.visible = true;
    }

    void Update()
    {
        HandleMovement();

        // Rotim camera doar dacă mouse-ul NU e peste UI și butonul drept e apăsat
        if (Input.GetMouseButton(1)) // click dreapta = freelook
        {
            HandleMouseLook();
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        //Vector3 move = Vector3.forward * vertical + Vector3.right * horizontal;
        //transform.position += move * moveSpeed * Time.deltaTime;
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
    
        Vector3 move = forward * vertical + right * horizontal;
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        Debug.Log("am intrat in handlemouselook");
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        rotationY += mouseX;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

   // bool IsPointerOverUI()
   // {
    //    return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    //}
}
