using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnitureDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public FurnitureItem furnitureItem; // Setat din MenuManager
    public Image dragPreviewImage;      // Imaginea fantomă care urmărește cursorul
    public Canvas canvas;               // Canvas-ul principal

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (furnitureItem == null || furnitureItem.thumbnail == null)
            return;

        dragPreviewImage.sprite = furnitureItem.thumbnail;
        dragPreviewImage.gameObject.SetActive(true);
        dragPreviewImage.rectTransform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragPreviewImage.rectTransform.position = Input.mousePosition;
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10f; // distanță față de cameră
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //worldPos.z = 0f; // asigură plasare pe planul corect
        return worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragPreviewImage.gameObject.SetActive(false);

        Vector3 worldPos = GetWorldPositionFromMouse();
        worldPos = SnapToGrid(worldPos, 1f); // SNAP pe grid

        // Obținem bounds-ul obiectului care urmează să fie plasat
        Vector3 checkSize = GetPrefabBounds(furnitureItem.prefab).extents;
        Vector3 checkCenter = worldPos + checkSize;

        // Verifică dacă ar atinge alt collider
        if (Physics.CheckBox(checkCenter, checkSize))
        {
            Debug.Log("Nu poți plasa obiectul peste altul.");
            return;
        }


        Instantiate(furnitureItem.prefab, worldPos, furnitureItem.prefab.transform.rotation);
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
