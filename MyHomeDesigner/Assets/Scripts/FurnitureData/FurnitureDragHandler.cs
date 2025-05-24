using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class FurnitureDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public FurnitureItem furnitureItem; 
    public Image dragPreviewImage;      
    public Canvas canvas;              
    private GameObject previewInstance;
    public Material highlightMaterial;
    private GameObject highlightBox;
    private GameObject validWallForDoor = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (furnitureItem == null || furnitureItem.thumbnail == null)
            return;


        previewInstance = Instantiate(furnitureItem.prefab);    
        previewInstance.layer = LayerMask.NameToLayer("Preview");
        foreach (Transform child in previewInstance.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Preview");
            foreach (Transform grandChild in child)
            {
                grandChild.gameObject.layer = LayerMask.NameToLayer("Preview");
            }
        }
                //SetTransparentMaterial(previewInstance);
        highlightBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(highlightBox.GetComponent<Collider>()); 
        highlightBox.name = "HighlightBox";
        highlightBox.GetComponent<MeshRenderer>().material = Instantiate(highlightMaterial);
        highlightBox.transform.SetParent(previewInstance.transform);
        BoxCollider box = previewInstance.GetComponent<BoxCollider>();
        if (box != null)
        {
            highlightBox.transform.localPosition = box.center;
            highlightBox.transform.localRotation = Quaternion.identity;
            highlightBox.transform.localScale = box.size;
        }
        else
        {
            Debug.LogWarning("PreviewInstance nu are BoxCollider!");
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance == null)
            return;
        //Vector3 pos = SnapToGrid(GetWorldPositionFromMouse(), 1f);

        Vector3 pos = GetWorldPositionFromMouse();

        //if (furnitureItem.placementType == PlacementType.Furniture)
        //{
        //    SnapPreviewToFloor();
        //}

        // Aplica SnapToGrid doar pentru Room și Furniture
        if (furnitureItem.placementType == PlacementType.Room) //|| furnitureItem.placementType == PlacementType.Furniture)
        {
            pos = SnapToGrid(pos, 1f);
        }

        previewInstance.transform.position = pos;

        switch (furnitureItem.placementType)
        {
            case PlacementType.Room:
                UpdateRoomHighlight(pos);
                break;

            case PlacementType.Furniture:
                UpdateFurnitureHighlight(pos);
                break;

            case PlacementType.Window:
                UpdateWindowHighlight(pos);
                break;

            case PlacementType.Door:
                UpdateDoorHighlight(pos);
                break;
        }
    }

    private void UpdateRoomHighlight(Vector3 pos)
    {
        bool overlaps = IsPlacementOverlapping(pos);

        if (highlightBox != null)
        {
            Color color = overlaps ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 1f, 1f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void UpdateFurnitureHighlight(Vector3 pos)
    {
        GameObject room = GetRoomUnderCursor(pos);
        bool valid = false;

        if (room != null)
        {
            float y = room.transform.position.y;
            pos.y = y;

            bool overlaps = IsFurnitureOverlapping(pos);
            valid = !overlaps;
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    /*private void UpdateDoorHighlight(Vector3 pos)
    {
        GameObject wall = GetWallUnderCursor(pos);
        bool valid = false;

        if (wall != null)
        {
            // Snap poziția ușii pe perete
            Vector3 snapPos = GetSnappedDoorPosition(wall, pos);
            previewInstance.transform.position = snapPos;

            // Verifică coliziune
            valid = !IsFurnitureOverlapping(snapPos);
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }*/

    /*private void UpdateDoorHighlight(Vector3 pos)
    {
        GameObject wall = GetWallUnderCursor(pos);
        bool valid = false;

        if (wall != null)
        {
            // Snap poziția ușii pe perete
            Vector3 snapPos = GetSnappedDoorPosition(wall, pos);
            previewInstance.transform.position = snapPos;

            // Verifică coliziune
            valid = !IsFurnitureOverlapping(snapPos);

            if (valid)
                validWallForDoor = wall; // salvăm peretele doar dacă totul e valid
            else
                validWallForDoor = null;
        }
        else
        {
            validWallForDoor = null; // niciun perete detectat
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }*/

    private void UpdateDoorHighlight(Vector3 pos)
    {
        GameObject wall = GetWallUnderCursor(pos);
        bool valid = false;

        if (wall != null)
        {
            // ROTIRE AUTOMATĂ ÎN FUNCȚIE DE PERETE
            Vector3 wallForward = wall.transform.forward;
            Vector3 doorForward = previewInstance.transform.forward;
            float angle = Vector3.Angle(wallForward, doorForward);

            if (angle > 45f && angle < 135f)
            {
                previewInstance.transform.rotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            // Snap poziția ușii pe perete
            Vector3 snapPos = GetSnappedDoorPosition(wall, pos);
            previewInstance.transform.position = snapPos;

            // Verifică coliziune
            valid = !IsFurnitureOverlapping(snapPos);

            if (valid)
                validWallForDoor = wall;
            else
                validWallForDoor = null;
        }
        else
        {
            validWallForDoor = null;
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void UpdateWindowHighlight(Vector3 pos)
    {
        GameObject wall = GetWallUnderCursor(pos);
        bool valid = false;

        if (wall != null)
        {
            // ROTIRE AUTOMATĂ ÎN FUNCȚIE DE PERETE
            Vector3 wallForward = wall.transform.forward;
            Vector3 doorForward = previewInstance.transform.forward;
            float angle = Vector3.Angle(wallForward, doorForward);

            if (angle > 45f && angle < 135f)
            {
                previewInstance.transform.rotation *= Quaternion.Euler(0f, 90f, 0f);
            }

            // Snap poziția ușii pe perete
            Vector3 snapPos = GetSnappedWindowPosition(wall, pos);
            previewInstance.transform.position = snapPos;

            // Verifică coliziune
            valid = !IsFurnitureOverlapping(snapPos);

            if (valid)
                validWallForDoor = wall;
            else
                validWallForDoor = null;
        }
        else
        {
            validWallForDoor = null;
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }


    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        //mousePos.z = Camera.main.nearClipPlane + 10f; // distanță față de cameră
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //worldPos.z = 0f; // asigură plasare pe planul corect
        //return worldPos;
        float zOffset = ViewState.CurrentMode == ViewMode.Mode3D ? 4f : 12f;
        mousePos.z = Camera.main.nearClipPlane + zOffset;

        return Camera.main.ScreenToWorldPoint(mousePos);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewInstance == null)
            return;

        switch (furnitureItem.placementType)
        {
            case PlacementType.Room:
                PlaceRoom();
                break;

            case PlacementType.Furniture:
                PlaceFurniture();
                break;

            case PlacementType.Window:
                PlaceWindow();
                break;

            case PlacementType.Door:
                PlaceDoor();
                break;
        }
    }


    private void PlaceRoom()
    {
        Vector3 worldPos = previewInstance.transform.position;

        bool overlaps = IsPlacementOverlapping(worldPos);

        if (overlaps)
        {
            Debug.Log("Nu poți plasa camera, se suprapune.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        GameObject roomInstance = Instantiate(furnitureItem.prefab, worldPos, previewInstance.transform.rotation);
        Vector3 roomCenter = roomInstance.transform.position;
        Vector3 viewPos = roomCenter + new Vector3(0, 1.7f, -1.5f);

        //RoomManager.Instance.RegisterRoom(roomInstance.transform, viewPos);
        RoomManager.Instance.RegisterRoom(roomInstance.transform);
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
    }

    private void PlaceFurniture()
    {
        Vector3 worldPos = previewInstance.transform.position;

        GameObject room = GetRoomUnderCursor(worldPos);
        if (room == null)
        {
            Debug.Log("Mobilierul trebuie să fie plasat într-o cameră.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        float y = room.transform.position.y;
        worldPos.y = y;
        //previewInstance.transform.position = worldPos;


        if (IsFurnitureOverlapping(worldPos))
        {
            Debug.Log("Mobilierul se suprapune cu alt obiect.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        GameObject instance = Instantiate(furnitureItem.prefab, previewInstance.transform.position, previewInstance.transform.rotation);
        Rigidbody rb = instance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;  
            rb.useGravity = true;

            int roomLayer = LayerMask.NameToLayer("Room");
            Collider[] furnitureColliders = instance.GetComponentsInChildren<Collider>();

            foreach (var col in furnitureColliders)
            {
                Physics.IgnoreLayerCollision(col.gameObject.layer, roomLayer, true);
            }

            StartCoroutine(SetKinematicAfterLanding(rb)); 
       }

        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);

        //GameObject instance = Instantiate(furnitureItem.prefab, worldPos, previewInstance.transform.rotation);
        instance.AddComponent<SelectableFurniture>();

    }

    private void SnapPreviewToFloor()
    {
        if (previewInstance == null)
            return;

        Vector3 origin = previewInstance.transform.position + Vector3.up * 1f;
        Ray downRay = new Ray(origin, Vector3.down);

        if (Physics.Raycast(downRay, out RaycastHit hit, 5f, LayerMask.GetMask("Floor")))
        {
            Vector3 snappedPosition = previewInstance.transform.position;
            snappedPosition.y = hit.point.y;
            previewInstance.transform.position = snappedPosition;
        }
    }


    IEnumerator SetKinematicAfterLanding(Rigidbody rb)
    {
        yield return new WaitUntil(() => rb.IsSleeping());
        //yield return new WaitForSeconds(0.5f);
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void PlaceDoor()
    {
        Vector3 pos = previewInstance.transform.position;

        GameObject room = GetRoomUnderCursor(pos);
        if (room == null)
        {
            Debug.Log("Usa trebuie să fie plasat într-o cameră.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        if (validWallForDoor == null)
        {
            Debug.Log("Ușa trebuie să fie plasată pe un perete.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        Vector3 snappedPos = GetSnappedDoorPosition(validWallForDoor, pos);
        if (IsFurnitureOverlapping(snappedPos))
        {
            Debug.Log("Ușa se suprapune cu alt obiect.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        Instantiate(furnitureItem.prefab, snappedPos, previewInstance.transform.rotation);
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
    }

    private void PlaceWindow()
    {
        Vector3 pos = previewInstance.transform.position;

        GameObject room = GetRoomUnderCursor(pos);
        if (room == null)
        {
            Debug.Log("Fereastra trebuie să fie plasat într-o cameră.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        if (validWallForDoor == null)
        {
            Debug.Log("Fereastra trebuie să fie plasată pe un perete.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        Vector3 snappedPos = GetSnappedWindowPosition(validWallForDoor, pos);
        if (IsFurnitureOverlapping(snappedPos))
        {
            Debug.Log("Fereastra se suprapune cu alt obiect.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        Instantiate(furnitureItem.prefab, snappedPos, previewInstance.transform.rotation);
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
    }


    private Bounds GetPrefabBounds(GameObject prefab)
    {
        Bounds bounds = new Bounds(prefab.transform.position, Vector3.zero);
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }   

        return bounds;
    }

    private Vector3 SnapToGrid(Vector3 pos, float cellSize)
    {
        pos.x = Mathf.Round(pos.x / cellSize) * cellSize;
        pos.y = Mathf.Round(pos.y / cellSize) * cellSize;
        //pos.z = 0f; // dacă grila e în plan XY
        return pos;
    }
    

    private bool IsPlacementOverlapping(Vector3 pos)
    {
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        int previewLayer = LayerMask.NameToLayer("Preview");
        //int previewLayer = LayerMask.NameToLayer("Preview");
        int mask = ~(1 << previewLayer);

        return Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, mask);
    }

    /*private bool IsFurnitureOverlapping(Vector3 pos)
    {
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        int previewLayer = LayerMask.NameToLayer("Preview");
        int roomLayer = LayerMask.NameToLayer("Room");
        int mask = ~( (1 << previewLayer) | (1 << roomLayer) );

        return Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, mask);
    }*/

    /*bool IsFurnitureOverlapping(Vector3 position)
    {
        int ignoreMask = LayerMask.GetMask("Room", "Floor", "Preview");
        int mask = ~ignoreMask;
        Vector3 halfExtents = furnitureItem.prefab.GetComponent<Renderer>().bounds.extents;
        Collider[] hits = Physics.OverlapBox(position, halfExtents, Quaternion.identity, mask);

        foreach (var hit in hits)
        {
            if (hit.gameObject.name.Contains("Dulap"))
            {
                Debug.Log("il vede sau nu??");
                return true;
            }
        }
        return false;
    }*/
    
    bool IsFurnitureOverlapping(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 3f, Vector3.down);
        float distance = 10f;
        int furnitureMask = LayerMask.GetMask("Furniture");

        return Physics.Raycast(ray, distance, furnitureMask);
    }


    /*bool IsRaycastHittingFurniture(GameObject prefab, Vector3 position)
    {
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Prefabul nu are Renderer!");
            return false;
        }

        Vector3 extents = renderer.bounds.extents;
        float heightOffset = 2f; 
        float castDistance = 5f;

        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(extents.x, 0, extents.z),
            new Vector3(-extents.x, 0, extents.z),
            new Vector3(extents.x, 0, -extents.z),
            new Vector3(-extents.x, 0, -extents.z)
        };

        int furnitureMask = LayerMask.GetMask("Furniture");

        foreach (var offset in offsets)
        {
            Vector3 rayOrigin = position + offset + Vector3.up * heightOffset;

            if (Physics.Raycast(rayOrigin, Vector3.down, castDistance, furnitureMask))
            {
                Debug.DrawRay(rayOrigin, Vector3.down * castDistance, Color.red, 1f);
                return true;
            }
        }

        return false;
    }*/



    private GameObject GetRoomUnderCursor(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 5f, Vector3.down); 
        //mask = (LayerMask.GetMask("Room") || LayerMask.GetMask("Floor"));
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, (LayerMask.GetMask("Room"))))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    /*private GameObject GetRoomUnderCursor(Vector3 pos)
    {
        int combinedLayerMask = LayerMask.GetMask("Room", "Floor");
        Ray ray = new Ray(pos + Vector3.up * 0.5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f, combinedLayerMask))
        {
            return hit.collider.gameObject;
        }
        Collider[] hits = Physics.OverlapSphere(pos, 0.25f, combinedLayerMask);
        if (hits.Length > 0)
        {
            return hits[0].gameObject;
        }

        return null;
    }*/

    /*private GameObject GetRoomUnderCursor(GameObject previewObj)
    {
        float boundsBottomY = previewObj.GetComponent<Renderer>().bounds.min.y;
        Vector3 rayOrigin = new Vector3(previewObj.transform.position.x, boundsBottomY + 0.1f, previewObj.transform.position.z);

        Ray ray = new Ray(rayOrigin, Vector3.down);
        int mask = LayerMask.GetMask("Floor", "Room");

        if (Physics.Raycast(ray, out RaycastHit hit, 0.2f, mask))
        {
            // Obiectul este deasupra podelei și NU îngropat în ea
            if (Mathf.Abs(hit.point.y - boundsBottomY) < 0.05f)
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }*/



    /*private GameObject GetWallUnderCursor(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 5f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Wall")))
        {
            Debug.Log("Perete detectat: " + hit.collider.name);  // ADĂUGAT
            return hit.collider.gameObject;
        }
        Debug.Log("NU a fost detectat niciun perete");  // ADĂUGAT
        return null;
    }*/

    private GameObject GetWallUnderCursor(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // trage din camera, spre scenă
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Wall")))
        {
            Debug.Log("Perete detectat: " + hit.collider.name);
            return hit.collider.gameObject;
        }

        Debug.Log("NU a fost detectat niciun perete");
        return null;
    }


    /*private Vector3 GetSnappedDoorPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds bounds = wall.GetComponent<Renderer>().bounds;

        Vector3 snapPos = bounds.center;
        Vector3 dir = (originalPos - bounds.center).normalized;

        float offset = 0.01f; // mic offset să nu intre în perete

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            // Atașează ușa de fața laterală a peretelui
            snapPos.x += Mathf.Sign(dir.x) * (bounds.extents.x + previewInstance.GetComponent<Renderer>().bounds.extents.x + offset);
            snapPos.z = originalPos.z;
        }
        else
        {
            // Atașează ușa de fața frontală a peretelui
            snapPos.z += Mathf.Sign(dir.z) * (bounds.extents.z + previewInstance.GetComponent<Renderer>().bounds.extents.z + offset);
            snapPos.x = originalPos.x;
        }

        snapPos.y = wall.transform.position.y; // așezăm la bază

        return snapPos;
    }*/

    /*private Vector3 GetSnappedDoorPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds bounds = wall.GetComponent<Renderer>().bounds;
        Vector3 snapPos = originalPos; // păstrăm poziția originală

        float offset = 0.01f;
        Vector3 dir = (originalPos - bounds.center).normalized;

        // Determinăm direcția dominantă a peretelui (x sau z)
        bool isWallAlongX = bounds.size.x > bounds.size.z;

        if (isWallAlongX)
        {
            // Peretele e de-a lungul axei X → blocăm Z
            snapPos.z = bounds.center.z + Mathf.Sign(dir.z) * (bounds.extents.z + previewInstance.GetComponent<Renderer>().bounds.extents.z + offset);
        }
        else
        {
            // Peretele e de-a lungul axei Z → blocăm X
            snapPos.x = bounds.center.x + Mathf.Sign(dir.x) * (bounds.extents.x + previewInstance.GetComponent<Renderer>().bounds.extents.x + offset);
        }

        // Aliniază la bază
        snapPos.y = wall.transform.position.y;

        return snapPos;
    }*/

   /* private Vector3 GetSnappedDoorPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds bounds = wall.GetComponent<Renderer>().bounds;
        Vector3 snapPos = originalPos;
    
        float offset = 0.01f;
        Vector3 dir = (originalPos - bounds.center).normalized;
    
        // Determină axa dominantă a peretelui
        bool isWallAlongX = bounds.size.x > bounds.size.z;
    
        // Luăm dimensiunea ușii cu rotația luată în calcul
        BoxCollider doorCollider = previewInstance.GetComponent<BoxCollider>();
        Vector3 doorSizeWorld = Vector3.Scale(doorCollider.size, previewInstance.transform.lossyScale);
        Vector3 doorExtents = doorSizeWorld * 0.5f;
    
        if (isWallAlongX)
        {
            // Ușa se atașează pe Z
            snapPos.z = bounds.center.z + Mathf.Sign(dir.z) * (bounds.extents.z + doorExtents.z + offset);
        }
        else
        {
            // Ușa se atașează pe X
            snapPos.x = bounds.center.x + Mathf.Sign(dir.x) * (bounds.extents.x + doorExtents.x + offset);
        }
    
        snapPos.y = wall.transform.position.y;
        return snapPos;
    }*/

    /*private Vector3 GetSnappedDoorPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds wallBounds = wall.GetComponent<Renderer>().bounds;
        Bounds doorBounds = previewInstance.GetComponent<Renderer>().bounds;

        Vector3 snapPos = originalPos;

        float offset = 0.001f; // mai mic, să nu o arunce prea departe
        Vector3 dir = (originalPos - wallBounds.center).normalized;

        bool isWallAlongX = wallBounds.size.x > wallBounds.size.z;

        if (isWallAlongX)
        {
            // Peretele este orizontal (X major), deci blocăm Z
            snapPos.z = wallBounds.center.z + Mathf.Sign(dir.z) * (wallBounds.extents.z + doorBounds.extents.z) + Mathf.Sign(dir.z) * offset;
        }
        else
        {
            // Peretele este vertical (Z major), deci blocăm X
            snapPos.x = wallBounds.center.x + Mathf.Sign(dir.x) * (wallBounds.extents.x + doorBounds.extents.x) + Mathf.Sign(dir.x) * offset;
        }

        snapPos.y = wall.transform.position.y; // aliniere pe podea

        return snapPos;
    }*/

    private Vector3 GetSnappedDoorPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds wallBounds = wall.GetComponent<Renderer>().bounds;
        Bounds doorBounds = previewInstance.GetComponent<Renderer>().bounds;

        Vector3 snapPos = originalPos;
        float offset = 0.01f;

        Vector3 dir = (originalPos - wallBounds.center).normalized;
        bool wallIsAlongX = wallBounds.size.x > wallBounds.size.z;

        if (wallIsAlongX)
        {
            // Plasăm ușa lipită de fața frontală a peretelui (Z)
            float doorHalfDepth = doorBounds.extents.z;
            float wallFaceZ = wallBounds.center.z + Mathf.Sign(dir.z) * wallBounds.extents.z;

            snapPos.z = wallFaceZ + Mathf.Sign(dir.z) * (doorHalfDepth + offset);
            snapPos.x = Mathf.Clamp(originalPos.x, wallBounds.min.x + doorBounds.extents.x, wallBounds.max.x - doorBounds.extents.x);
        }
        else
        {
            // Plasăm ușa lipită de fața laterală a peretelui (X)
            float doorHalfWidth = doorBounds.extents.x;
            float wallFaceX = wallBounds.center.x + Mathf.Sign(dir.x) * wallBounds.extents.x;

            snapPos.x = wallFaceX + Mathf.Sign(dir.x) * (doorHalfWidth + offset);
            snapPos.z = Mathf.Clamp(originalPos.z, wallBounds.min.z + doorBounds.extents.z, wallBounds.max.z - doorBounds.extents.z);
        }
    
        // Așezăm pe podea (presupunem baza la poziția Y a peretelui)
        snapPos.y = wall.transform.position.y;
    
        return snapPos;
    }

    private Vector3 GetSnappedWindowPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds wallBounds = wall.GetComponent<Renderer>().bounds;
        Bounds doorBounds = previewInstance.GetComponent<Renderer>().bounds;

        Vector3 snapPos = originalPos;
        float offset = 0.01f;

        Vector3 dir = (originalPos - wallBounds.center).normalized;
        bool wallIsAlongX = wallBounds.size.x > wallBounds.size.z;

        if (wallIsAlongX)
        {
            // Plasăm ușa lipită de fața frontală a peretelui (Z)
            float doorHalfDepth = doorBounds.extents.z;
            float wallFaceZ = wallBounds.center.z + Mathf.Sign(dir.z) * wallBounds.extents.z;

            snapPos.z = wallFaceZ + Mathf.Sign(dir.z) * (doorHalfDepth + offset);
            snapPos.x = Mathf.Clamp(originalPos.x, wallBounds.min.x + doorBounds.extents.x, wallBounds.max.x - doorBounds.extents.x);
        }
        else
        {
            // Plasăm ușa lipită de fața laterală a peretelui (X)
            float doorHalfWidth = doorBounds.extents.x;
            float wallFaceX = wallBounds.center.x + Mathf.Sign(dir.x) * wallBounds.extents.x;

            snapPos.x = wallFaceX + Mathf.Sign(dir.x) * (doorHalfWidth + offset);
            snapPos.z = Mathf.Clamp(originalPos.z, wallBounds.min.z + doorBounds.extents.z, wallBounds.max.z - doorBounds.extents.z);
        }
    
        // Așezăm la mijlocul peretelui (presupunem baza la poziția Y a peretelui)
        snapPos.y = wall.transform.position.y + wall.GetComponent<Renderer>().bounds.size.y / 2f;

    
        return snapPos;
    }



}
