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

    public void OnDrag(PointerEventData eventData)
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

    public void OnEndDrag(PointerEventData eventData)
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
}
