using UnityEngine;

public class FurnitureSelector : MonoBehaviour
{
    public GameObject toolPanelPrefab; // setează în Inspector
    private GameObject currentToolPanel;
    public Transform canvasParent;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int furnitureLayerMask = ~(1 << LayerMask.NameToLayer("Room"));
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, furnitureLayerMask))
            {
                Debug.Log("Lovit: " + hitInfo.collider.name + " | Are SelectableFurniture: " + (hitInfo.collider.GetComponent<SelectableFurniture>() != null));
                //Debug.Log("Lovit: " + hitInfo.collider.name + " | Are SelectableFurniture: " + (hitInfo.collider.GetComponent<SelectableFurniture>() != null));

                /*SelectableFurniture furniture = hitInfo.collider.GetComponent<SelectableFurniture>();
                if (furniture != null)
                {
                    Debug.Log("Ai selectat mobilierul: " + hitInfo.collider.name);
                    FurnitureManipulator.Instance.SelectFurniture(hitInfo.collider.gameObject);
                }*/
                if (currentToolPanel != null)
                    Destroy(currentToolPanel);

                //GameObject panel = Instantiate(toolPanelPrefab);
                //panel.GetComponent<FurnitureToolPanel>().Initialize(hitInfo.collider.gameObject);
                //currentToolPanel = panel;

                GameObject panel = Instantiate(toolPanelPrefab, canvasParent);
                panel.GetComponent<FurnitureToolPanel>().Initialize(hitInfo.collider.gameObject);
                currentToolPanel = panel;
            }

        }
    }
}
