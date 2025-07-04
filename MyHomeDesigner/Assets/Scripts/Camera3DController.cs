using UnityEngine;

public class Camera3DController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            HandleMouseLook();
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        Vector3 move = (forward * vertical + right * horizontal) * moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        rotationY += mouseX;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
