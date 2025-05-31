using UnityEngine;

public class SelectableFurniture : MonoBehaviour
{
    public GameObject assignedRoom;
    public string price; 
    void Start()
    {
        Debug.Log("SelectableFurniture attached to " + gameObject.name);
        AssignToRoom(); 
    }

    private void OnMouseDown()
    {
        Debug.Log("click");
        FurnitureManipulator.Instance.SelectFurniture(this.gameObject);
    }

    private void AssignToRoom()
    {
        Collider[] overlaps = Physics.OverlapBox(transform.position, transform.localScale / 2f);

        foreach (var col in overlaps)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Room"))
            {
                assignedRoom = col.gameObject;
                Debug.Log($"Obiectul {name} a fost asignat camerei {assignedRoom.name}");
                break;
            }
        }
    }
}
