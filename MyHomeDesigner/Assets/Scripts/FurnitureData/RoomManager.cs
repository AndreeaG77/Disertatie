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
        public Vector3 viewPosition;

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

    public List<RoomData> GetAllRooms()
    {
        return allRooms;
    }


    public void RegisterRoom(Transform roomTransform)
    {
        roomCount++;
        string roomName = "Room " + roomCount;

        /*RoomData data = new RoomData(roomName, roomTransform); 
        data.viewPosition = viewPosition;
  
        allRooms.Add(data);

        Debug.Log($"RoomManager: Registered {roomName} at {roomTransform.position}");

        OnRoomRegistered?.Invoke(data); */

        Transform cameraSpawn = roomTransform.Find("CameraSpawn");

       /* if (cameraSpawn != null)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.transform.position = cameraSpawn.position;
            marker.transform.localScale = Vector3.one * 0.3f;
            marker.GetComponent<Collider>().enabled = false;
            marker.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            Debug.LogWarning($"CameraSpawn NU a fost găsit pentru {roomName}. Se folosește fallback.");
        }*/
        
        Vector3 viewPosition = cameraSpawn != null 
            ? cameraSpawn.position 
            : roomTransform.position + new Vector3(0, 1.7f, 0);

        RoomData data = new RoomData(roomName, roomTransform);
        data.viewPosition = viewPosition;
        allRooms.Add(data);

        Debug.Log($"RoomManager: Registered {roomName} at {viewPosition}");
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
