using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomButtonUI : MonoBehaviour
{
    private Camera mainCamera;
    private RoomManager.RoomData roomData;


    public void Initialize(RoomManager.RoomData data)
    {
        roomData = data;
        mainCamera = Camera.main;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = roomData.roomName;


        /*GetComponent<Button>().onClick.AddListener(() =>
        {
            StopAllCoroutines();
            FindObjectOfType<ViewToggle>()?.Show2DButton();
            StartCoroutine(MoveCameraSmooth(
                mainCamera.transform,
                roomData.viewPosition,
                Quaternion.Euler(0f, 0f, 0f),
                1f,
                roomData.roomTransform
            ));
        });*/

        GetComponent<Button>().onClick.AddListener(() =>
        {
            StopAllCoroutines();

            if (ViewState.CurrentMode == ViewMode.Mode3D)
            {
                Transform camera2D = Object.FindFirstObjectByType<ViewToggle>().camera2DPositionMove;

                StartCoroutine(MoveThrough2DView(
                    mainCamera.transform,
                    camera2D.position,
                    camera2D.rotation,
                    roomData.viewPosition,
                    Quaternion.Euler(0f, 0f, 0f),
                    1.5f,
                    1.5f,
                    roomData.roomTransform
                ));
            }
            else
            {
                StartCoroutine(MoveCameraSmooth(
                    mainCamera.transform,
                    roomData.viewPosition,
                    Quaternion.Euler(0f, 0f, 0f),
                    1f,
                    roomData.roomTransform
                ));
            }
        });



        /* GetComponent<Button>().onClick.AddListener(() =>
           {
               //Vector3 eyeLevelOffset = new Vector3(0, 1.7f, 0);
               //mainCamera.transform.position = targetRoom.position + eyeLevelOffset;
               //mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

               Vector3 offset = new Vector3(0f, 1.7f, -2f);
               mainCamera.transform.position = targetRoom.position + offset;
               mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f); 
           });*/

        /*GetComponent<Button>().onClick.AddListener(() =>
        {
            Renderer roomRenderer = targetRoom.GetComponentInChildren<Renderer>();
            if (roomRenderer == null)
            {
                Debug.LogWarning("Room has no Renderer to calculate bounds!");
                return;
            }
        
            Vector3 center = roomRenderer.bounds.center;
            Vector3 cameraPos = new Vector3(center.x, 1.7f, center.z); // poziție umană în centrul camerei
        
            mainCamera.transform.position = cameraPos;
            mainCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // privire în față
        });*/

        /*GetComponent<Button>().onClick.AddListener(() =>
        {
            Collider[] colliders = targetRoom.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
            {
                Debug.LogWarning("No colliders found in room!");
                return;
            }

            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }

            Vector3 centerXZ = new Vector3(bounds.center.x, 0f, bounds.center.z);
            float cameraHeight = bounds.min.y + 1.7f; // podea + nivel ochi
            Vector3 cameraPos = new Vector3(centerXZ.x, cameraHeight, centerXZ.z);
            Quaternion targetRot = Quaternion.Euler(0f, 0f, 0f);

            Debug.Log($"Target camera position: {cameraPos}");
            Debug.DrawLine(mainCamera.transform.position, cameraPos, Color.green, 2f);
            Debug.Log("Bounds center: " + bounds.center);
            Debug.Log("Target camera position: " + cameraPos);

            StopAllCoroutines();
            StartCoroutine(MoveCameraSmooth(mainCamera.transform, cameraPos, targetRot, 1f));

        });*/
    }

    private IEnumerator MoveCameraSmooth(Transform cam, Vector3 targetPos, Quaternion targetRot, float duration, Transform roomTransform)
    {
        Camera.main.orthographic = false;
        //cam.fieldOfView = 60f;
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<Camera3DController>().enabled = true;
        ViewState.CurrentMode = ViewMode.Mode3D;

        ViewToggle toggle = FindFirstObjectByType<ViewToggle>();
        if (toggle != null)
        {
            toggle.Show2DButton();
        }

        //Transform floor = roomTransform.GetComponentInChildren<Transform>().Find("Floor");
        //Transform floor = roomTransform.GetComponentInChildren<Transform>(true);
        if (roomTransform != null)
        {
            foreach (Transform t in roomTransform.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.Contains("Floor"))
                {
                    Renderer rend = t.GetComponent<Renderer>();
                    if (rend != null && rend.material != null)
                    {
                        Color c = rend.material.color;
                        c.a = 1f;
                        rend.material.color = c;
                    }
                }
            }
        }

        FurnitureMenu furnitureMenu = Object.FindFirstObjectByType<FurnitureMenu>();
        if (furnitureMenu != null)
        {
            furnitureMenu.RefreshUI();
            furnitureMenu.RefreshCategoryButtons("3D");
        }

        GameObject existingPanel = GameObject.FindWithTag("EditorOnly");
        if (existingPanel != null)
        {
            Destroy(existingPanel);
        }

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            cam.position = Vector3.Lerp(startPos, targetPos, t);
            cam.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;
    }

    private IEnumerator MoveThrough2DView(Transform cam, Vector3 to2DPos, Quaternion to2DRot,
                                          Vector3 toRoomPos, Quaternion toRoomRot,
                                          float duration1, float duration2,
                                          Transform roomTransform)
    {
        yield return MoveCameraSmooth(cam, to2DPos, to2DRot, duration1, null);
        yield return MoveCameraSmooth(cam, toRoomPos, toRoomRot, duration2, roomTransform);
        
    }

}
