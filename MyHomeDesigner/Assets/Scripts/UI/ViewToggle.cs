using UnityEngine;
using UnityEngine.UI;

public class ViewToggle : MonoBehaviour
{
    public Button toggleButton;
    public Transform camera2DPosition;
    public Transform camera3DTarget;
    //public FurnitureMenu furnitureMenu;
    private bool is3DView = false;
    private Camera mainCamera;
    private CameraController cameraController;


    void Start()
    {
        mainCamera = Camera.main;
        cameraController = mainCamera.GetComponent<CameraController>();
        toggleButton.onClick.AddListener(ToggleView);

    }

    void ToggleView()
    {
        is3DView = !is3DView;

        if (is3DView)
        {
            cameraController.enabled = false;

            mainCamera.transform.position = camera3DTarget.position + new Vector3(-3f, 3f, -3f);
            mainCamera.transform.LookAt(camera3DTarget);
            ViewState.CurrentMode = ViewMode.Mode3D;
            FurnitureMenu furnitureMenu = Object.FindFirstObjectByType<FurnitureMenu>();
            if (furnitureMenu != null)
            {
                furnitureMenu.RefreshUI();
                furnitureMenu.RefreshCategoryButtons("3D");
            }
            toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "2D";
        }
        else
        {
            mainCamera.transform.position = camera2DPosition.position;
            mainCamera.transform.rotation = camera2DPosition.rotation;

            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 12.5f;
            Camera.main.GetComponent<Camera3DController>().enabled = false;
            Camera.main.GetComponent<CameraController>().enabled = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ViewState.CurrentMode = ViewMode.Mode2D;
            FurnitureMenu furnitureMenu = Object.FindFirstObjectByType<FurnitureMenu>();
            if (furnitureMenu != null)
            {
                furnitureMenu.RefreshUI();
                furnitureMenu.RefreshCategoryButtons("2D");
            }
            toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "3D";

        }
    }
}
