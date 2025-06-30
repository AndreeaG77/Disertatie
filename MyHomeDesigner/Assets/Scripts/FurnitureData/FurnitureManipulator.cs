    using UnityEngine;
    using UnityEngine.EventSystems;

    public class FurnitureManipulator : MonoBehaviour
    {
        public static FurnitureManipulator Instance;

        private GameObject selectedFurniture;
        public enum ManipulationMode { None, Move, Rotate, Scale, ScaleX, ScaleY, ScaleZ }
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
            case "ScaleY": currentMode = ManipulationMode.ScaleY; break;
            case "ScaleZ": currentMode = ManipulationMode.ScaleZ; break;
            default: currentMode = ManipulationMode.None; break;
        }
    }

        public enum MoveAxis { Free, X, Y, Z }
        private MoveAxis moveAxis = MoveAxis.Free;

        public void SetMoveAxis(string axis)
        {
            switch (axis)
            {
                case "X": moveAxis = MoveAxis.X; break;
                case "Y": moveAxis = MoveAxis.Y; break;
                case "Z": moveAxis = MoveAxis.Z; break;
                case "Free": moveAxis = MoveAxis.Free; break;
            }
        }



        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private string selectedType = "Furniture"; // Default

        public void SelectFurniture(GameObject furniture)
        {
            selectedFurniture = furniture;
        
            int layer = selectedFurniture.layer;
            if (layer == LayerMask.NameToLayer("Door")) selectedType = "Door";
            else if (layer == LayerMask.NameToLayer("Window")) selectedType = "Window";
            else selectedType = "Furniture";
        
            Debug.Log("Mobilier selectat: " + furniture.name + " | Tip: " + selectedType);
        }

        void Update()
        {
            if (selectedFurniture == null || currentMode == ManipulationMode.None)
                return;

            switch (currentMode)
            {
                case ManipulationMode.Move:
                    if (selectedType == "Door")
                        HandleDoorMovementX();
                    else if (selectedType == "Window")
                        HandleWindowMovement();
                    else
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
                case ManipulationMode.ScaleY:
                    HandleAxisScaling(Vector3.up);
                    break;
                case ManipulationMode.ScaleZ:
                    HandleAxisScaling(Vector3.forward);
                    break;
            }
        }

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

                    Vector3 currentPos = selectedFurniture.transform.position;
                    Vector3 targetPos = hit.point;

                    switch (moveAxis)
                    {
                        case MoveAxis.X:
                            targetPos = new Vector3(hit.point.x, currentPos.y, currentPos.z);
                            break;
                        case MoveAxis.Y:
                        {
                            Plane yPlane = new Plane(Vector3.right, currentPos); 
                            if (yPlane.Raycast(ray, out float enter))
                            {
                                Vector3 hitPoint = ray.GetPoint(enter);

                                float halfHeight = selectedFurniture.GetComponent<Renderer>().bounds.extents.y;

                                float minY = 1.5f + halfHeight;  
                                float maxY = 4f - halfHeight;   

                                float clampedY = Mathf.Clamp(hitPoint.y, minY, maxY);
                                targetPos = new Vector3(currentPos.x, clampedY, currentPos.z);
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }

                        case MoveAxis.Z:
                            targetPos = new Vector3(currentPos.x, currentPos.y, hit.point.z);
                            break;
                        case MoveAxis.Free:
                            targetPos = new Vector3(hit.point.x, currentPos.y, hit.point.z);
                            break;
                    }

                    BoxCollider col = selectedFurniture.GetComponent<BoxCollider>();
                    Vector3 size = Vector3.Scale(col.size, selectedFurniture.transform.lossyScale);
                    Vector3 halfExtents = size * 0.5f;

                    Collider[] hits = Physics.OverlapBox(targetPos, halfExtents, selectedFurniture.transform.rotation);
                    foreach (var h in hits)
                    {
                        if (h.gameObject != selectedFurniture && h.gameObject.layer != LayerMask.NameToLayer("Room") && h.gameObject.layer != LayerMask.NameToLayer("Floor"))
                        {
                            Debug.Log("Suprapunere cu: " + h.name);
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


        void HandleDoorMovementX()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int mask = ~(1 << LayerMask.NameToLayer("Room"));

                if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
                {
                    if (hit.collider.gameObject != selectedFurniture)
                        return;

                    Vector3 currentPos = selectedFurniture.transform.position;
                    Vector3 hitPoint = hit.point;

                    bool wallIsAlongX = Mathf.Abs(selectedFurniture.transform.forward.z) > Mathf.Abs(selectedFurniture.transform.forward.x);
                    Vector3 targetPos = currentPos;

                    if (wallIsAlongX)
                    {
                        targetPos.x = hitPoint.x;
                    }
                    else
                    {
                        targetPos.z = hitPoint.z;
                    }

                    targetPos.y = currentPos.y;

                    BoxCollider col = selectedFurniture.GetComponent<BoxCollider>();
                    Vector3 size = Vector3.Scale(col.size, selectedFurniture.transform.lossyScale);
                    Vector3 halfExtents = col.bounds.extents;
                    Vector3 centerOffset = col.bounds.center - selectedFurniture.transform.position;

                    Collider[] hits = Physics.OverlapBox(
                        targetPos + centerOffset,
                        halfExtents,
                        Quaternion.identity
                    );

                    foreach (var h in hits)
                    {
                        if (h.gameObject != selectedFurniture)
                        {
                            int otherLayer = h.gameObject.layer;
                            if (otherLayer == LayerMask.NameToLayer("Door") || otherLayer == LayerMask.NameToLayer("Window") || otherLayer == LayerMask.NameToLayer("Wall"))
                            {
                                Debug.Log("Ușa se suprapune cu altă ușă sau fereastră: " + h.name);
                                return;
                            }
                        }
                    }

                    Rigidbody rb = selectedFurniture.GetComponent<Rigidbody>();
                    if (rb != null){
                        rb.MovePosition(targetPos);
                    }
                        
                    else
                        selectedFurniture.transform.position = targetPos;
                }
            }
        }


        void HandleWindowMovement()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int mask = ~(1 << LayerMask.NameToLayer("Room"));

                if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
                {
                    if (hit.collider.gameObject != selectedFurniture)
                        return;

                    Vector3 currentPos = selectedFurniture.transform.position;
                    Vector3 hitPoint = hit.point;
                    
                    bool wallIsAlongX = Mathf.Abs(selectedFurniture.transform.forward.z) > Mathf.Abs(selectedFurniture.transform.forward.x);

                    Vector3 targetPos = currentPos;

                    if (wallIsAlongX)
                    {
                        targetPos.x = hitPoint.x;
                    }
                    else
                    {
                        targetPos.z = hitPoint.z;
                    }

                    targetPos.y = hitPoint.y;

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
                        if (h.gameObject != selectedFurniture)
                        {
                            int otherLayer = h.gameObject.layer;
                            if (otherLayer == LayerMask.NameToLayer("Door") || otherLayer == LayerMask.NameToLayer("Window") || otherLayer == LayerMask.NameToLayer("Wall") || otherLayer == LayerMask.NameToLayer("Floor"))
                            {
                                Debug.Log("Fereastra se suprapune cu altă ușă sau fereastră: " + h.name);
                                return;
                            }
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
                Vector3 oldScale = selectedFurniture.transform.localScale;
                Vector3 newScale = oldScale + Vector3.one * scroll;
                newScale = ClampScale(newScale);

                selectedFurniture.transform.localScale = newScale;
                float newBottom = selectedFurniture.GetComponent<Renderer>().bounds.min.y;
                float oldBottom = selectedFurniture.GetComponent<Renderer>().bounds.min.y;
                float delta = oldBottom - newBottom;

                Vector3 testPosition = selectedFurniture.transform.position;
                testPosition.y += delta;

                BoxCollider col = selectedFurniture.GetComponent<BoxCollider>();
                Vector3 size = Vector3.Scale(col.size, selectedFurniture.transform.lossyScale);
                Vector3 halfExtents = size * 0.5f;

                Collider[] hits = Physics.OverlapBox(
                    testPosition,
                    halfExtents,
                    selectedFurniture.transform.rotation
                );

                bool collides = false;
                foreach (var h in hits)
                {
                    if (h.gameObject != selectedFurniture &&
                        h.gameObject.layer != LayerMask.NameToLayer("Room") &&
                        h.gameObject.layer != LayerMask.NameToLayer("Floor"))
                    {
                        collides = true;
                        break;
                    }
                }

                if (collides)
                {
                    selectedFurniture.transform.localScale = oldScale;
                    return;
                }

                selectedFurniture.transform.position = testPosition;
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
                Vector3 oldScale = selectedFurniture.transform.localScale;
                Vector3 newScale = oldScale + axis * scroll;
                newScale = ClampScale(newScale);

                selectedFurniture.transform.localScale = newScale;

                float delta = 0f;
                if (axis == Vector3.up)
                {
                    float oldBottom = selectedFurniture.GetComponent<Renderer>().bounds.min.y;
                    float newBottom = selectedFurniture.GetComponent<Renderer>().bounds.min.y;
                    delta = oldBottom - newBottom;
                }

                Vector3 proposedPosition = selectedFurniture.transform.position;
                if (axis == Vector3.up)
                {
                    proposedPosition.y += delta;
                }

                BoxCollider col = selectedFurniture.GetComponent<BoxCollider>();
                Vector3 size = Vector3.Scale(col.size, selectedFurniture.transform.lossyScale);
                Vector3 halfExtents = size * 0.5f;

                Collider[] hits = Physics.OverlapBox(
                    proposedPosition,
                    halfExtents,
                    selectedFurniture.transform.rotation
                );

                bool collides = false;
                foreach (var h in hits)
                {
                    if (h.gameObject != selectedFurniture &&
                        h.gameObject.layer != LayerMask.NameToLayer("Room") &&
                        h.gameObject.layer != LayerMask.NameToLayer("Floor"))
                    {
                        collides = true;
                        break;
                    }
                }

                if (collides)
                {
                    selectedFurniture.transform.localScale = oldScale;
                    return;
                }

                selectedFurniture.transform.position = proposedPosition;
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
