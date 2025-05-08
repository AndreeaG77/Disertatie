using UnityEngine;
using UnityEngine.UI;

public class ViewToggle : MonoBehaviour
{
    public Button toggleButton;
    //public Text toggleButtonText;

    public Transform camera2DPosition;
    public Transform camera3DTarget;

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
            // Dezactivăm controlul 2D
            cameraController.enabled = false;

            // Poziționăm camera într-un unghi ușor de sus și din lateral
            mainCamera.transform.position = camera3DTarget.position + new Vector3(-3f, 3f, -3f);
            mainCamera.transform.LookAt(camera3DTarget);

            toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "2D";
        }
        else
        {
            // Reactivăm modul 2D
            mainCamera.transform.position = camera2DPosition.position;
            mainCamera.transform.rotation = camera2DPosition.rotation;

            cameraController.enabled = true;
            toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "3D";

        }
    }
}
