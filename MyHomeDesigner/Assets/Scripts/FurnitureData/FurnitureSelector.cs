using UnityEngine;

public class FurnitureSelector : MonoBehaviour
{
    public GameObject toolPanelPrefab;
    private GameObject currentToolPanel;

    public GameObject moveToolPanelPrefab;
    private GameObject currentMoveToolPanel;

    public Transform canvasParent;
    private GameObject lastSelectedObject;

    public GameObject scaleToolPanelPrefab;
    private GameObject currentScaleToolPanel;

    public GameObject deleteRoomButtonPrefab;
    private GameObject currentDeleteRoomButton;



    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //int furnitureLayerMask = ~(1 << LayerMask.NameToLayer("Room"));
            int furnitureLayerMask = ~(1 << LayerMask.NameToLayer("Room") | 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Floor"));

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, furnitureLayerMask))
            {
                //Debug.Log("Lovit: " + hitInfo.collider.name + " | Are SelectableFurniture: " + (hitInfo.collider.GetComponent<SelectableFurniture>() != null));
                //Debug.Log("Lovit: " + hitInfo.collider.name + " | Are SelectableFurniture: " + (hitInfo.collider.GetComponent<SelectableFurniture>() != null));

                /*SelectableFurniture furniture = hitInfo.collider.GetComponent<SelectableFurniture>();
                if (furniture != null)
                {
                    Debug.Log("Ai selectat mobilierul: " + hitInfo.collider.name);
                    FurnitureManipulator.Instance.SelectFurniture(hitInfo.collider.gameObject);
                }*/
                int layer = hitInfo.collider.gameObject.layer;
                if (ViewState.CurrentMode == ViewMode.Mode2D &&
                    (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window")))
                {
                    return;
                }

                GameObject hitObject = hitInfo.collider.gameObject;

                if (hitObject != lastSelectedObject)
                {
                    FurnitureManipulator.Instance.ClearMode();
                    lastSelectedObject = hitObject;
                }

                if (currentToolPanel != null)
                {
                    Destroy(currentToolPanel);
                    //FurnitureManipulator.Instance.ClearMode();
                }

                if (currentMoveToolPanel != null)
                {
                    Destroy(currentMoveToolPanel);
                }

                if (currentScaleToolPanel != null)
                {
                    Destroy(currentScaleToolPanel);
                }

                if (currentDeleteRoomButton != null)
                {
                    Destroy(currentDeleteRoomButton);
                }

                //FurnitureManipulator.Instance.ClearMode();

                //GameObject panel = Instantiate(toolPanelPrefab);
                //panel.GetComponent<FurnitureToolPanel>().Initialize(hitInfo.collider.gameObject);
                //currentToolPanel = panel;

                GameObject panel = Instantiate(toolPanelPrefab, canvasParent);
                panel.GetComponent<FurnitureToolPanel>().Initialize(hitInfo.collider.gameObject);
                currentToolPanel = panel;

                GameObject movePanel = Instantiate(moveToolPanelPrefab, canvasParent);
                movePanel.SetActive(false);
                movePanel.GetComponent<MoveToolPanel>().Initialize(hitInfo.collider.gameObject);
                currentMoveToolPanel = movePanel;
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int roomLayerMask = 1 << LayerMask.NameToLayer("Room");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, roomLayerMask))
            {
                if (currentToolPanel != null)
                {
                    Destroy(currentToolPanel);
                }

                if (currentMoveToolPanel != null)
                {
                    Destroy(currentMoveToolPanel);
                }

                if (currentScaleToolPanel != null)
                {
                    Destroy(currentScaleToolPanel);
                }

                if (currentDeleteRoomButton != null)
                {
                    Destroy(currentDeleteRoomButton);
                }

                if (ViewState.CurrentMode == ViewMode.Mode2D)
                {
                    GameObject deleteButton = Instantiate(deleteRoomButtonPrefab, canvasParent);
                    currentDeleteRoomButton = deleteButton;
                    deleteButton.GetComponent<DeleteRoomButton>().Initialize(hit.collider.gameObject);
                }

            }
        }
    }

    
    public void RegisterScaleToolPanel(GameObject panel)
    {
        if (currentScaleToolPanel != null)
            Destroy(currentScaleToolPanel);

        currentScaleToolPanel = panel;
    }

}
