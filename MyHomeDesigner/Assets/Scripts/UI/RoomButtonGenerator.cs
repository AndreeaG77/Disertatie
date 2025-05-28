using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;  
public class RoomButtonGenerator : MonoBehaviour
{
    public GameObject roomButtonPrefab;
    public Transform buttonParent;
    private List<GameObject> roomButtons = new List<GameObject>();

    private void OnEnable()
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnRoomRegistered += HandleRoomRegistered;
            RoomManager.Instance.OnRoomDeleted += HandleRoomDeleted;
        }

    }

    private void OnDisable()
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnRoomRegistered -= HandleRoomRegistered;
            RoomManager.Instance.OnRoomDeleted -= HandleRoomDeleted;
        }

    }

    private void HandleRoomRegistered(RoomManager.RoomData data)
    {
        GameObject btn = Instantiate(roomButtonPrefab, buttonParent);
        //btn.GetComponent<RoomButtonUI>().Initialize(data.roomName, data.roomTransform);
        btn.GetComponent<RoomButtonUI>().Initialize(data);
        roomButtons.Add(btn);

    }
    
    private void HandleRoomDeleted(RoomManager.RoomData data)
    {
        var btn = roomButtons.FirstOrDefault(b => b.GetComponent<RoomButtonUI>().RepresentsRoom(data));
        if (btn != null)
        {
            roomButtons.Remove(btn);
            Destroy(btn);
        }

        for (int i = 0; i < roomButtons.Count; i++)
        {
            var ui = roomButtons[i].GetComponent<RoomButtonUI>();
            ui.UpdateLabel($"Room {i + 1}");
        }
    }
}
