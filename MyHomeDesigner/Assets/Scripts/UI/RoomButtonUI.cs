using UnityEngine;
using UnityEngine.UI;

public class RoomButtonUI : MonoBehaviour
{
    private Transform targetRoom;
    private Camera mainCamera;

    public void Initialize(string roomName, Transform roomTransform)
    {
        targetRoom = roomTransform;
        mainCamera = Camera.main;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = roomName;


        GetComponent<Button>().onClick.AddListener(() =>
        {
            // Poziționează camera într-un unghi spre această cameră
            mainCamera.transform.position = targetRoom.position + new Vector3(-3f, 3f, -3f);
            mainCamera.transform.LookAt(targetRoom);
        });
    }
}
