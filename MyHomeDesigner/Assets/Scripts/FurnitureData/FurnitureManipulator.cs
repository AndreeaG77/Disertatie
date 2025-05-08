using UnityEngine;
using UnityEngine.EventSystems;

public class FurnitureManipulator : MonoBehaviour
{
    public static FurnitureManipulator Instance;

    private GameObject selectedFurniture;
    public enum ManipulationMode { None, Move, Rotate, Scale, ScaleX, ScaleZ }
    private ManipulationMode currentMode = ManipulationMode.None;
    

    public void SetMode(string mode, GameObject target)
    {
        selectedFurniture = target;
    
        switch (mode)
        {
            case "Move": currentMode = ManipulationMode.Move; break;
            case "Rotate": currentMode = ManipulationMode.Rotate; break;
            case "Scale": currentMode = ManipulationMode.Scale; break;
            case "ScaleX": currentMode = ManipulationMode.ScaleX; break;
            case "ScaleZ": currentMode = ManipulationMode.ScaleZ; break;
            default: currentMode = ManipulationMode.None; break;
        }
    }


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        Debug.Log("Mobilier selectat: " + furniture.name);
    }

    void Update()
    {
        if (selectedFurniture == null) return;

        switch (currentMode)
        {
            case ManipulationMode.Move:
                HandleMovement();
                break;
            case ManipulationMode.Rotate:
                HandleRotation();
                break;
            case ManipulationMode.Scale:
                HandleScaling();
                break;
            case ManipulationMode.ScaleX:
                HandleAxisScaling(Vector3.right); 
                break;
            case ManipulationMode.ScaleZ:
                HandleAxisScaling(Vector3.forward); 
                break;
        }
    }


    void HandleMovement()
    {

        if (Input.GetMouseButton(0)) // drag cu click stânga
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = ~(1 << LayerMask.NameToLayer("Room"));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            //if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Room")))
            {
                //Debug.Log("test1");
                //Debug.Log(selectedFurniture);
                //Debug.Log(hit.collider.gameObject);
                if (hit.collider.gameObject != selectedFurniture)
                    return;
                //Debug.Log("test2");
                Vector3 newPos = hit.point;
                newPos.y = selectedFurniture.transform.position.y; // păstrăm poziția pe Y
                selectedFurniture.transform.position = newPos;
            }
        }
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
            selectedFurniture.transform.Rotate(Vector3.up, -100f * Time.deltaTime);
        if (Input.GetKey(KeyCode.E))
            selectedFurniture.transform.Rotate(Vector3.up, 100f * Time.deltaTime);
    }

    void HandleScaling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 scale = selectedFurniture.transform.localScale;
            scale += Vector3.one * scroll;
            scale = Vector3.Max(scale, Vector3.one * 0.2f); // dimensiune minimă
            scale = Vector3.Min(scale, Vector3.one * 5f);   // dimensiune maximă
            selectedFurniture.transform.localScale = scale;
        }
    }

    void HandleUniformScaling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 scale = selectedFurniture.transform.localScale;
            scale += Vector3.one * scroll;
            scale = ClampScale(scale);
            selectedFurniture.transform.localScale = scale;
        }
    }

    void HandleAxisScaling(Vector3 axis)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 scale = selectedFurniture.transform.localScale;
            scale += axis * scroll;
            scale = ClampScale(scale);
            selectedFurniture.transform.localScale = scale;
        }
    }

    Vector3 ClampScale(Vector3 scale)
    {
        Vector3 min = Vector3.one * 0.2f;
        Vector3 max = Vector3.one * 5f;
        return new Vector3(
            Mathf.Clamp(scale.x, min.x, max.x),
            Mathf.Clamp(scale.y, min.y, max.y),
            Mathf.Clamp(scale.z, min.z, max.z)
        );
    }

}
