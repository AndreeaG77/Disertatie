using System.Collections;
using UnityEngine;

public class RoomButtonGenerator : MonoBehaviour
{
    public GameObject roomButtonPrefab;
    public Transform buttonParent;

    private void OnEnable() 
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnRoomRegistered += HandleRoomRegistered; 
    }

    private void OnDisable() 
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnRoomRegistered -= HandleRoomRegistered; 
    }

    private void HandleRoomRegistered(RoomManager.RoomData data) 
    {
        GameObject btn = Instantiate(roomButtonPrefab, buttonParent); 
        //btn.GetComponent<RoomButtonUI>().Initialize(data.roomName, data.roomTransform);
        btn.GetComponent<RoomButtonUI>().Initialize(data);
 
    }
}
