using UnityEngine;

public class SelectableFurniture : MonoBehaviour
{
    public GameObject assignedRoom;
    public string price;
    void Start()
    {
        AssignToRoom();
    }

    private void OnMouseDown()
    {
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
                break;
            }
        }
    }
        
    public void ForceAssignToRoom()
    {
        AssignToRoom();
    }

}
