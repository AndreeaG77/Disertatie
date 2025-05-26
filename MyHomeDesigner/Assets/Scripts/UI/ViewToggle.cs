using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ViewToggle : MonoBehaviour
{
    public Button toggleButton;
    public Transform camera2DPositionOriginal;
    public Transform camera2DPositionMove;
    private Camera mainCamera;
    private CameraController cameraController;

    void Start()
    {
        mainCamera = Camera.main;
        cameraController = mainCamera.GetComponent<CameraController>();
        toggleButton.onClick.AddListener(SwitchTo2D);

        toggleButton.gameObject.SetActive(false);
    }

    /* public void SwitchTo2D()
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

         toggleButton.gameObject.SetActive(false);

         foreach (var room in RoomManager.Instance.GetAllRooms())
         {
             foreach (Transform t in room.roomTransform.GetComponentsInChildren<Transform>(true))
             {
                 if (t.name.Contains("Floor"))
                 {
                     Renderer rend = t.GetComponent<Renderer>();
                     if (rend != null && rend.material != null)
                     {
                         Color c = rend.material.color;
                         c.a = 150 / 255f;
                         rend.material.color = c;
                     }
                 }
             }
         }

         FurnitureMenu furnitureMenu = Object.FindFirstObjectByType<FurnitureMenu>();
         if (furnitureMenu != null)
         {
             furnitureMenu.RefreshUI();
             furnitureMenu.RefreshCategoryButtons("2D");
         }
     }
     */
    
    public void SwitchTo2D()
    {
         StartCoroutine(SmoothTransitionTo2D());
    }

    private IEnumerator SmoothTransitionTo2D()
    {
        Camera.main.orthographic = false;
        Camera.main.GetComponent<Camera3DController>().enabled = false;
        Camera.main.GetComponent<CameraController>().enabled = false;

        Transform cam = Camera.main.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        Vector3 endPos = camera2DPositionOriginal.position;
        Quaternion endRot = camera2DPositionOriginal.rotation;

        float duration = 1.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerpT = t / duration;
            cam.position = Vector3.Lerp(startPos, endPos, lerpT);
            cam.rotation = Quaternion.Lerp(startRot, endRot, lerpT);
            yield return null;
        }

        cam.position = endPos;
        cam.rotation = endRot;

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 10f;
        Camera.main.GetComponent<CameraController>().enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ViewState.CurrentMode = ViewMode.Mode2D;

        toggleButton.gameObject.SetActive(false);

        foreach (var room in RoomManager.Instance.GetAllRooms())
        {
            foreach (Transform obj in room.roomTransform.GetComponentsInChildren<Transform>(true))
            {
                if (obj.name.Contains("Floor"))
                {
                    Renderer rend = obj.GetComponent<Renderer>();
                    if (rend != null && rend.material != null)
                    {
                        Color c = rend.material.color;
                        c.a = 150 / 255f;
                        rend.material.color = c;
                    }
                }
            }
        }

        FurnitureMenu furnitureMenu = Object.FindFirstObjectByType<FurnitureMenu>();
        if (furnitureMenu != null)
        {
            furnitureMenu.RefreshUI();
            furnitureMenu.RefreshCategoryButtons("2D");
        }
        
        GameObject existingPanel = GameObject.FindWithTag("EditorOnly");
        if (existingPanel != null)
        {
            FurnitureManipulator.Instance.ClearMode();
            Destroy(existingPanel);
        }
    }


    public void Show2DButton()
    {
        toggleButton.gameObject.SetActive(true);
        toggleButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "2D";
    }
}
