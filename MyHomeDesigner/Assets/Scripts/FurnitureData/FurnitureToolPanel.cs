using UnityEngine;
using UnityEngine.UI;

public class FurnitureToolPanel : MonoBehaviour
{
    public Button moveButton;
    public Button rotateButton;
    public Button scaleButton;
    //public Button scaleXButton;
    //public Button scaleYButton;
    //public Button scaleZButton;
    public Button deleteButton;
    public GameObject scaleToolPanelPrefab;
    private GameObject scalePanelInstance;

    private GameObject targetObject;

    public void Initialize(GameObject target)
    {
        targetObject = target;

        int layer = target.layer;
        bool isDoorOrWindow = false;
        if ((layer == LayerMask.NameToLayer("Door")) || (layer == LayerMask.NameToLayer("Window")))
            isDoorOrWindow = true;
        //bool isDoor = layer == LayerMask.NameToLayer("Door");

        moveButton.gameObject.SetActive(true);
        rotateButton.gameObject.SetActive(!isDoorOrWindow);
        //scaleButton.gameObject.SetActive(!isDoorOrWindow);  
        scaleButton.gameObject.SetActive(true);
        //scaleXButton.gameObject.SetActive(true);
        //if (ViewState.CurrentMode == ViewMode.Mode2D) scaleYButton.gameObject.SetActive(false); // sau true dacă vrei tot timpul
        //else scaleYButton.gameObject.SetActive(true);
        //scaleZButton.gameObject.SetActive(!isDoorOrWindow); 
        deleteButton.gameObject.SetActive(true);

        /*moveButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Move", targetObject));
        rotateButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Rotate", targetObject));
        scaleButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Scale", targetObject));
        scaleXButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleX", targetObject));
        scaleZButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleZ", targetObject));
        deleteButton.onClick.AddListener(() => DeleteObject());*/
        
        if (ViewState.CurrentMode == ViewMode.Mode3D && !isDoorOrWindow)
            moveButton.onClick.AddListener(() =>
            {
                FurnitureManipulator.Instance.SetMode("Move", targetObject);
                gameObject.SetActive(false);    
                MoveToolPanel.Instance.Show(targetObject); 
                //FindFirstObjectByType<MoveToolPanel>().gameObject.SetActive(true);
            });
        else moveButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Move", targetObject));
        if (!isDoorOrWindow) rotateButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Rotate", targetObject));
        //if (!isDoorOrWindow) scaleButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Scale", targetObject));
        scaleButton.onClick.AddListener(() =>
        {
            FurnitureManipulator.Instance.ClearMode();
            gameObject.SetActive(false);

            if (scalePanelInstance != null)
                Destroy(scalePanelInstance);

            scalePanelInstance = Instantiate(scaleToolPanelPrefab, transform.parent);
            scalePanelInstance.GetComponent<ScaleToolPanel>().Initialize(targetObject);
            FindFirstObjectByType<FurnitureSelector>().RegisterScaleToolPanel(scalePanelInstance);

        });

        //scaleXButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleX", targetObject));
        //if (ViewState.CurrentMode == ViewMode.Mode3D) scaleYButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleY", targetObject));
        //if (!isDoorOrWindow) scaleZButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleZ", targetObject));
        deleteButton.onClick.AddListener(() => DeleteObject());
    }

    /*void Update()
    {
        if (targetObject != null)
        {
            float offsetZ = 0.7f + targetObject.transform.localScale.z * 0.4f;
            Vector3 worldPos = targetObject.transform.position + new Vector3(0, 0, offsetZ);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            GetComponent<RectTransform>().position = screenPos;
        }
    }*/

    void Update()
    {
        if (targetObject == null) return;

        Vector3 worldPos;

        if (ViewState.CurrentMode == ViewMode.Mode2D)
        {
            float offsetZ = 0.7f + targetObject.transform.localScale.z * 0.4f;
            worldPos = targetObject.transform.position + new Vector3(0, 0, offsetZ);
        }
        else
        {
            Collider col = targetObject.GetComponent<Collider>();
            if (col != null)
            {
                Vector3 topPoint = col.bounds.max;
                float offset = 0.5f; 
                worldPos = new Vector3(topPoint.x, topPoint.y + offset, topPoint.z);
            }
            else
            {
                worldPos = targetObject.transform.position + Vector3.up * 1f;
            }

        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (ViewState.CurrentMode == ViewMode.Mode3D && screenPos.z < 0)
        {
            GetComponent<RectTransform>().position = new Vector3(-9999, -9999);
        }
        else
        {
            GetComponent<RectTransform>().position = screenPos;
        }
        
        //GetComponent<RectTransform>().position = screenPos;
    }


    private void DeleteObject()
    {
        if (targetObject != null)
        {
            Destroy(targetObject);
            FurnitureManipulator.Instance.ClearMode();
            Destroy(gameObject);
        }
    }


}
