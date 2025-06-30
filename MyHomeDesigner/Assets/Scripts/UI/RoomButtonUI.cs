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
    }

    private IEnumerator MoveCameraSmooth(Transform cam, Vector3 targetPos, Quaternion targetRot, float duration, Transform roomTransform)
    {
        Camera.main.orthographic = false;
        Camera.main.GetComponent<CameraController>().enabled = false;
        Camera.main.GetComponent<Camera3DController>().enabled = true;
        ViewState.CurrentMode = ViewMode.Mode3D;

        ViewToggle toggle = FindFirstObjectByType<ViewToggle>();
        if (toggle != null)
        {
            toggle.Show2DButton();
        }

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
            FurnitureManipulator.Instance.ClearMode();
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
    
    public bool RepresentsRoom(RoomManager.RoomData data)
    {
        return this.roomData == data;
    }

    public void UpdateLabel(string name)
    {
        roomData.roomName = name;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = name;
    }
}
