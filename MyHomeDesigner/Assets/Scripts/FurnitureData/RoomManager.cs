using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [System.Serializable]
    public class RoomData
    {
        public string roomName;
        public Transform roomTransform;

        public RoomData(string name, Transform transform)
        {
            roomName = name;
            roomTransform = transform;
        }
    }

    public List<RoomData> allRooms = new List<RoomData>();

    public delegate void RoomRegisteredHandler(RoomData newRoom); 
    public event RoomRegisteredHandler OnRoomRegistered;          

    private int roomCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterRoom(Transform roomTransform)
    {
        roomCount++;
        string roomName = "Room " + roomCount;

        RoomData data = new RoomData(roomName, roomTransform);   
        allRooms.Add(data);

        Debug.Log($"RoomManager: Registered {roomName} at {roomTransform.position}");

        OnRoomRegistered?.Invoke(data); 
    }

    public RoomData GetRoomByIndex(int index)
    {
        if (index >= 0 && index < allRooms.Count)
            return allRooms[index];
        return null;
    }

    public int RoomCount => allRooms.Count;  
}
