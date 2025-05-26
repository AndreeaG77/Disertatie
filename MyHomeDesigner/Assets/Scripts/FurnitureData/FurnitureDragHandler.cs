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
            //Color color = overlaps ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 1f, 1f, 0.3f);
            Color color = overlaps ? new Color(1f, 0f, 0f, 0.3f) : new Color(0f, 1f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void UpdateFurnitureHighlight(Vector3 pos)
    {
        bool valid = false;

        if (ViewState.CurrentMode == ViewMode.Mode2D)
        {
            GameObject room = GetRoomUnderCursor(pos);
            if (room != null)
            {
                float y = room.transform.position.y;
                pos.y = y;

                bool overlaps = IsFurnitureOverlapping(pos);
                valid = !overlaps;
            }
        }
        else
        {
            GameObject room = GetRoomUnderCursor3D(pos);
            if (room != null)
            {
                bool overlaps = IsFurnitureOverlapping(pos);
                valid = !overlaps;
            }
        }

        if (highlightBox != null)
        {
            //Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            Color color = valid ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
    }
    private void UpdateDoorHighlight(Vector3 pos)
    {
        bool valid = false;
        GameObject wall = GetWallUnderCursor(pos);

        if (wall != null)
        {
            Vector3 snappedPos = GetSnappedDoorPosition(wall, pos);
            previewInstance.transform.position = snappedPos;
            previewInstance.transform.rotation = Quaternion.LookRotation(-wall.transform.forward);

            Collider[] overlaps = Physics.OverlapBox(
                previewInstance.transform.position,
                previewInstance.GetComponent<Renderer>().bounds.extents,
                previewInstance.transform.rotation
            );

            bool overlapWithDoorOrWindow = false;
            foreach (var col in overlaps)
            {
                int layer = col.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window"))
                {
                    overlapWithDoorOrWindow = true;
                    break;
                }
            }

            valid = !overlapWithDoorOrWindow;

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
            //Color color = valid ? new Color(1f, 1f, 1f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            Color color = valid ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }

    }



    private void UpdateWindowHighlight(Vector3 pos)
    {
        bool valid = false;
        GameObject wall = GetWallUnderCursor(pos);

        if (wall != null)
        {
            Vector3 snapPos = GetSnappedWindowPosition(wall, pos);
            previewInstance.transform.position = snapPos;
            previewInstance.transform.rotation = Quaternion.LookRotation(-wall.transform.forward);

            Collider[] overlaps = Physics.OverlapBox(
                previewInstance.transform.position,
                previewInstance.GetComponent<Renderer>().bounds.extents,
                previewInstance.transform.rotation
            );

            bool overlapWithDoorOrWindow = false;
            foreach (var col in overlaps)
            {
                int layer = col.gameObject.layer;
                if (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window"))
                {
                    overlapWithDoorOrWindow = true;
                    break;
                }
            }

            valid = !overlapWithDoorOrWindow;
            validWallForDoor = valid ? wall : null;
        }
        else
        {
            validWallForDoor = null;
        }

        if (highlightBox != null)
        {
            Color color = valid ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
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
        GameObject room;
        if (ViewState.CurrentMode == ViewMode.Mode2D)
        {
            room = GetRoomUnderCursor(worldPos);
        }
        else
        {
            room = GetRoomUnderCursor3D(worldPos);
        }

        if (room == null)
        {
            Debug.Log("Mobilierul trebuie să fie plasat într-o cameră.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        float y = room.transform.position.y;
        worldPos.y = y;

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
        if (previewInstance == null)
            return;
    
        if (validWallForDoor == null)
        {
            Debug.Log("Nu există perete valid în spatele ușii.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }
    
        Vector3 pos = previewInstance.transform.position;
    
        Collider[] overlaps = Physics.OverlapBox(
            pos,
            previewInstance.GetComponent<Renderer>().bounds.extents,
            previewInstance.transform.rotation
        );
    
        foreach (var col in overlaps)
        {
            int layer = col.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window"))
            {
                Debug.Log("Există deja o ușă sau fereastră în această poziție.");
                Destroy(previewInstance);
                if (highlightBox != null) Destroy(highlightBox);
                return;
            }
        }
    
        GameObject instance = Instantiate(furnitureItem.prefab, pos, previewInstance.transform.rotation);
        instance.layer = LayerMask.NameToLayer("Door"); // sau setează în editor direct
    
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
    }



    private void PlaceWindow()
    {
        if (previewInstance == null)
            return;

        if (validWallForDoor == null)
        {
            Debug.Log("Fereastra trebuie să fie plasată pe un perete.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        Vector3 pos = previewInstance.transform.position;

        Collider[] overlaps = Physics.OverlapBox(
            pos,
            previewInstance.GetComponent<Renderer>().bounds.extents,
            previewInstance.transform.rotation
        );

        foreach (var col in overlaps)
        {
            int layer = col.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window"))
            {
                Debug.Log("Fereastra se suprapune cu alt obiect.");
                Destroy(previewInstance);
                if (highlightBox != null) Destroy(highlightBox);
                return;
            }
        }

        GameObject instance = Instantiate(furnitureItem.prefab, pos, previewInstance.transform.rotation);
        instance.layer = LayerMask.NameToLayer("Window");

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
    bool IsFurnitureOverlapping(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 3f, Vector3.down);
        float distance = 3.5f;
        int furnitureMask = LayerMask.GetMask("Furniture");

        return Physics.Raycast(ray, distance, furnitureMask);
    }

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


    private GameObject GetRoomUnderCursor3D(Vector3 worldPos)
    {
        Collider[] overlapping = Physics.OverlapBox(
            worldPos,
            previewInstance.GetComponent<Renderer>().bounds.extents,
            Quaternion.identity,
            LayerMask.GetMask("Wall", "Floor")
        );

        if (overlapping.Length > 0)
        {
            return null;
        }

        return RoomManager.Instance.GetRoomByIndex(0).roomTransform.gameObject;;
    }

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
            float doorHalfDepth = doorBounds.extents.z;
            float wallFaceZ = wallBounds.center.z + Mathf.Sign(dir.z) * wallBounds.extents.z;

            snapPos.z = wallFaceZ + Mathf.Sign(dir.z) * (doorHalfDepth + offset);
            snapPos.x = Mathf.Clamp(originalPos.x, wallBounds.min.x + doorBounds.extents.x, wallBounds.max.x - doorBounds.extents.x);
        }
        else
        {
            float doorHalfWidth = doorBounds.extents.x;
            float wallFaceX = wallBounds.center.x + Mathf.Sign(dir.x) * wallBounds.extents.x;

            snapPos.x = wallFaceX + Mathf.Sign(dir.x) * (doorHalfWidth + offset);
            snapPos.z = Mathf.Clamp(originalPos.z, wallBounds.min.z + doorBounds.extents.z, wallBounds.max.z - doorBounds.extents.z);
        }

        //snapPos.y = wall.transform.position.y;
        // Plasare la nivel cu podeaua (baza peretelui)
        float wallBottomY = wall.GetComponent<Renderer>().bounds.min.y;
        float doorHalfHeight = previewInstance.GetComponent<Renderer>().bounds.extents.y;

        snapPos.y = wallBottomY + doorHalfHeight;

    
        return snapPos;
    }

    private Vector3 GetSnappedWindowPosition(GameObject wall, Vector3 originalPos)
    {
        Bounds wallBounds = wall.GetComponent<Renderer>().bounds;
        Bounds windowBounds = previewInstance.GetComponent<Renderer>().bounds;

        Vector3 snapPos = originalPos;
        float offset = 0.01f;

        Vector3 dir = (originalPos - wallBounds.center).normalized;
        bool wallIsAlongX = wallBounds.size.x > wallBounds.size.z;

        if (wallIsAlongX)
        {
            float windowHalfDepth = windowBounds.extents.z;
            float wallFaceZ = wallBounds.center.z + Mathf.Sign(dir.z) * wallBounds.extents.z;

            snapPos.z = wallFaceZ + Mathf.Sign(dir.z) * (windowHalfDepth + offset);
            snapPos.x = Mathf.Clamp(originalPos.x, wallBounds.min.x + windowBounds.extents.x, wallBounds.max.x - windowBounds.extents.x);
        }
        else
        {
            float doorHalfWidth = windowBounds.extents.x;
            float wallFaceX = wallBounds.center.x + Mathf.Sign(dir.x) * wallBounds.extents.x;

            snapPos.x = wallFaceX + Mathf.Sign(dir.x) * (doorHalfWidth + offset);
            snapPos.z = Mathf.Clamp(originalPos.z, wallBounds.min.z + windowBounds.extents.z, wallBounds.max.z - windowBounds.extents.z);
        }
        //snapPos.y = wall.transform.position.y + wall.GetComponent<Renderer>().bounds.size.y / 2f;
        //snapPos.y = Mathf.Clamp(originalPos.y, wallBounds.min.y + windowBounds.extents.y, wallBounds.max.y - windowBounds.extents.y);

    
        return snapPos;
    }

}
