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
        //currentMode = ManipulationMode.None;
        Debug.Log("Mobilier selectat: " + furniture.name);
    }

    void Update()
    {
        if (selectedFurniture == null || currentMode == ManipulationMode.None)
            return;

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


    /*void HandleMovement()
    {

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = ~(1 << LayerMask.NameToLayer("Room"));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            //if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Room")))
            {
                if (hit.collider.gameObject != selectedFurniture)
                    return;
                Vector3 newPos = hit.point;
                newPos.y = selectedFurniture.transform.position.y;
                selectedFurniture.transform.position = newPos;
            }
        }
    }*/

    /*void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = ~(1 << LayerMask.NameToLayer("Room"));

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            {
                if (hit.collider.gameObject != selectedFurniture)
                    return;

                Vector3 newPos = hit.point;
                newPos.y = selectedFurniture.transform.position.y;

                Rigidbody rb = selectedFurniture.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.MovePosition(newPos);
                }
                else
                {
                    selectedFurniture.transform.position = newPos; // fallback
                }
            }
        }
    }*/

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = ~(1 << LayerMask.NameToLayer("Room"));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            {
                if (hit.collider.gameObject != selectedFurniture)
                    return;

                Vector3 targetPos = hit.point;
                targetPos.y = selectedFurniture.transform.position.y;

                //Vector3 halfExtents = selectedFurniture.GetComponent<Collider>().bounds.extents;

                //Collider[] hits = Physics.OverlapBox(targetPos, halfExtents, selectedFurniture.transform.rotation);
                BoxCollider col = selectedFurniture.GetComponent<BoxCollider>();
                Vector3 size = Vector3.Scale(col.size, selectedFurniture.transform.lossyScale);
                Vector3 halfExtents = size * 0.5f;

                Collider[] hits = Physics.OverlapBox(
                    targetPos,
                    halfExtents,
                    selectedFurniture.transform.rotation
                );

                foreach (var h in hits)
                {
                    if (h.gameObject != selectedFurniture && h.gameObject.layer != LayerMask.NameToLayer("Room") && h.gameObject.layer != LayerMask.NameToLayer("Floor"))
                    {
                        Debug.Log("Mutarea ar cauza suprapunere cu: " + h.name);
                        return;
                    }
                }

                Rigidbody rb = selectedFurniture.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.MovePosition(targetPos);
                else
                    selectedFurniture.transform.position = targetPos;
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
            scale = Vector3.Max(scale, Vector3.one * 0.2f);
            scale = Vector3.Min(scale, Vector3.one * 5f);
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
    
    public void ClearMode()
    {
        currentMode = ManipulationMode.None;
    }


}
