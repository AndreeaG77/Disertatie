using UnityEngine;
using UnityEngine.UI;

public class DeleteRoomButton : MonoBehaviour
{
    public Button deleteButton;

    private GameObject targetRoom;

    public void Initialize(GameObject target)
    {
        targetRoom = target;

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(() =>
            {
                if (targetRoom != null)
                {
                    var roomManager = RoomManager.Instance;
                    var data = roomManager?.FindRoomDataByTransform(targetRoom.transform);
                    if (data != null)
                    {
                        roomManager.DeleteRoom(data);
                    }

                    Collider roomCollider = targetRoom.GetComponentInChildren<Collider>();
                    if (roomCollider != null)
                    {
                        Bounds roomBounds = roomCollider.bounds;

                        string[] targetLayers = { "Furniture", "Door", "Window" };
                        int furnitureLayer = LayerMask.NameToLayer("Furniture");
                        
                        foreach (string layerName in targetLayers)
                        {
                            int layer = LayerMask.NameToLayer(layerName);
                            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                        
                            foreach (GameObject obj in allObjects)
                            {
                                if (obj.layer == layer && obj.activeInHierarchy)
                                {
                                    Collider objCollider = obj.GetComponent<Collider>();
                                    if (objCollider != null && roomBounds.Intersects(objCollider.bounds))
                                    {
                                        Destroy(obj);
                                    }
                                }
                            }
                        }

                    }


                    Destroy(targetRoom);
                }

                Destroy(gameObject);
            });
        }
    }

    void Update()
    {
        if (targetRoom == null) return;

        if (ViewState.CurrentMode != ViewMode.Mode2D)
        {
            Destroy(gameObject);
            return;
        }

        Collider col = targetRoom.GetComponentInChildren<Collider>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            float offsetZ = 0.6f;
            Vector3 cornerWorldPos = new Vector3(bounds.max.x, bounds.center.y, bounds.max.z + offsetZ);

            Vector3 screenPos = Camera.main.WorldToScreenPoint(cornerWorldPos);
            GetComponent<RectTransform>().position = screenPos;
        }
        
    }

}
