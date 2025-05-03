using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnitureDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public FurnitureItem furnitureItem; // Setat din MenuManager
    public Image dragPreviewImage;      // Imaginea fantomă care urmărește cursorul
    public Canvas canvas;               // Canvas-ul principal
    private GameObject previewInstance;
    public Material highlightMaterial;
    private GameObject highlightBox;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (furnitureItem == null || furnitureItem.thumbnail == null)
            return;

       // dragPreviewImage.sprite = furnitureItem.thumbnail;
       // dragPreviewImage.gameObject.SetActive(true);
       // dragPreviewImage.rectTransform.position = Input.mousePosition;

       // Creează o instanță fantomă
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
        Destroy(highlightBox.GetComponent<Collider>()); // nu vrem collider
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

    /*public void OnDrag(PointerEventData eventData)
    {
        //dragPreviewImage.rectTransform.position = Input.mousePosition;
        if (previewInstance == null)
            return;

        Vector3 pos = SnapToGrid(GetWorldPositionFromMouse(), 1f);
        previewInstance.transform.position = pos;

        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        int previewLayerMask = ~(1 << LayerMask.NameToLayer("Preview"));
        bool overlaps = Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, previewLayerMask);

        if (highlightBox != null)
        {
            Color color = overlaps ? new Color(1f, 0f, 0f, 0.3f) : new Color(1f, 1f, 1f, 0.3f);
            highlightBox.GetComponent<MeshRenderer>().material.color = color;
        }
       // Debug.Log("Highlight color: " + highlightBox.GetComponent<MeshRenderer>().material.color);
    }*/

    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance == null)
            return;
        //Debug.Log("am intrat macar??");
        Vector3 pos = SnapToGrid(GetWorldPositionFromMouse(), 1f);
        previewInstance.transform.position = pos;
        //Debug.Log(furnitureItem.placementType);
        switch (furnitureItem.placementType)
        {
            case PlacementType.Room:
                //Debug.Log("case Room la onDrag");
                UpdateRoomHighlight(pos);
                break;

            case PlacementType.Furniture:
                UpdateFurnitureHighlight(pos);
                break;

            case PlacementType.Window:
            case PlacementType.Door:
                // Mai târziu
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
            // Așezăm pe podea
            float y = room.transform.position.y;
            pos.y = y;

            // Verificăm coliziunea reală
            bool overlaps = IsFurnitureOverlapping(pos);
            valid = !overlaps;
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
        mousePos.z = Camera.main.nearClipPlane + 10f; // distanță față de cameră
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //worldPos.z = 0f; // asigură plasare pe planul corect
        return worldPos;
    }

    /*public void OnEndDrag(PointerEventData eventData)
    {
        dragPreviewImage.gameObject.SetActive(false);

        Vector3 worldPos = GetWorldPositionFromMouse();
        worldPos = SnapToGrid(worldPos, 1f); // SNAP pe grid

        Bounds bounds = GetPrefabBounds(furnitureItem.prefab);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = worldPos + bounds.center - furnitureItem.prefab.transform.position;

        // Verifică coliziune rotită
        if (Physics.CheckBox(checkCenter, checkSize, furnitureItem.prefab.transform.rotation))
        {
            Debug.Log("Nu poți plasa obiectul, ar suprapune altul.");
            return;
        }


        Instantiate(furnitureItem.prefab, worldPos, furnitureItem.prefab.transform.rotation);
    }*/

    /*public void OnEndDrag(PointerEventData eventData)
    {
        if (previewInstance != null)
        {
            Vector3 worldPos = previewInstance.transform.position;

            Bounds bounds = GetPrefabBounds(previewInstance);
            Vector3 checkSize = bounds.extents;
            Vector3 checkCenter = worldPos + bounds.center - previewInstance.transform.position;

            int previewLayerMask = ~(1 << LayerMask.NameToLayer("Preview")); // inversează masca → exclude "Preview"

            if (Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, previewLayerMask))
            {
                Debug.Log("Nu poți plasa obiectul, ar suprapune altul.");
                Destroy(previewInstance);
                return;
            }

            // Plasează obiectul real
            Instantiate(furnitureItem.prefab, worldPos, previewInstance.transform.rotation);

            //Instantiate(furnitureItem.prefab, worldPos, furnitureItem.prefab.transform.rotation);
            Destroy(previewInstance); // Șterge preview-ul
        }
    }*/

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewInstance == null)
            return;

        switch (furnitureItem.placementType)
        {
            case PlacementType.Room:
                //Debug.Log("case Room la OnEndDrag");
                PlaceRoom();
                break;

            case PlacementType.Furniture:
                PlaceFurniture();
                break;

            // Le vom adăuga mai târziu
            case PlacementType.Window:
            case PlacementType.Door:
                Debug.Log("Plasare specială pentru uși și ferestre – de implementat.");
                Destroy(previewInstance);
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

        Instantiate(furnitureItem.prefab, worldPos, previewInstance.transform.rotation);
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
        //Debug.Log("destroy");
    }

    private void PlaceFurniture()
    {
        Vector3 worldPos = previewInstance.transform.position;

        // 1. Verificăm dacă e într-o cameră
        GameObject room = GetRoomUnderCursor(worldPos);
        if (room == null)
        {
            Debug.Log("Mobilierul trebuie să fie plasat într-o cameră.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        // 2. Așezăm pe podea (presupunem plan XY și camere pe Y = 0)
        float y = room.transform.position.y; // sau 0 dacă podeaua e la Y=0
        //worldPos.y = y;
        //float y = room.transform.position.y;
        worldPos.y = y;
        previewInstance.transform.position = worldPos;


        /*// 3. Verificăm suprapunere
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = worldPos + bounds.center - previewInstance.transform.position;

        int previewLayerMask = ~(1 << LayerMask.NameToLayer("Preview"));
        bool overlaps = Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, previewLayerMask);

        if (overlaps)
        {
            Debug.Log("Mobilierul se suprapune cu alt obiect.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }*/

        if (IsFurnitureOverlapping(worldPos))
        {
            Debug.Log("Mobilierul se suprapune cu alt obiect.");
            Destroy(previewInstance);
            if (highlightBox != null) Destroy(highlightBox);
            return;
        }

        // 4. Instanțiem mobilierul
        Instantiate(furnitureItem.prefab, worldPos, previewInstance.transform.rotation);
        Destroy(previewInstance);
        if (highlightBox != null) Destroy(highlightBox);
    }



    private Bounds GetPrefabBounds(GameObject prefab)
    {
        // Obține bounds cumulat pentru toate mesh renderer-ele din prefab
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
    
    /*private bool IsPlacementOverlapping(Vector3 pos)
    {
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        int previewLayerMask = ~(1 << LayerMask.NameToLayer("Preview"));
        return Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, previewLayerMask);
    }*/

    private bool IsPlacementOverlapping(Vector3 pos)
    {
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        // Excludem și layerul "Preview", dar și "Room"
        int previewLayer = LayerMask.NameToLayer("Preview");
        //int previewLayer = LayerMask.NameToLayer("Preview");
        int mask = ~(1 << previewLayer);

        return Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, mask);
    }

    private bool IsFurnitureOverlapping(Vector3 pos)
    {
        Bounds bounds = GetPrefabBounds(previewInstance);
        Vector3 checkSize = bounds.extents;
        Vector3 checkCenter = pos + bounds.center - previewInstance.transform.position;

        int previewLayer = LayerMask.NameToLayer("Preview");
        int roomLayer = LayerMask.NameToLayer("Room");
        int mask = ~( (1 << previewLayer) | (1 << roomLayer) );

        return Physics.CheckBox(checkCenter, checkSize, previewInstance.transform.rotation, mask);
    }



    private GameObject GetRoomUnderCursor(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 5f, Vector3.down); // Tragem o rază în jos
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, LayerMask.GetMask("Room")))
        {
            return hit.collider.gameObject;
        }
        return null;
    }


}
