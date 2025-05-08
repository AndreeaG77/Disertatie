using UnityEngine;

public class SelectableFurniture : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SelectableFurniture attached to " + gameObject.name);
    }
    private void OnMouseDown()
    {
        Debug.Log("click");
        FurnitureManipulator.Instance.SelectFurniture(this.gameObject);
    }
}
