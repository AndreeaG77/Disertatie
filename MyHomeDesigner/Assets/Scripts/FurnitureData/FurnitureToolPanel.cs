using UnityEngine;
using UnityEngine.UI;

public class FurnitureToolPanel : MonoBehaviour
{
    public Button moveButton;
    public Button rotateButton;
    public Button scaleButton;
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

        moveButton.gameObject.SetActive(true);
        if (ViewState.CurrentMode == ViewMode.Mode2D)
            FurnitureManipulator.Instance.SetMoveAxis("Free");
        rotateButton.gameObject.SetActive(!isDoorOrWindow);
        scaleButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        
        if (ViewState.CurrentMode == ViewMode.Mode3D && !isDoorOrWindow)
            moveButton.onClick.AddListener(() =>
            {
                FurnitureManipulator.Instance.SetMode("Move", targetObject);
                gameObject.SetActive(false);    
                MoveToolPanel.Instance.Show(targetObject); 
            });
        else moveButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Move", targetObject));
        if (!isDoorOrWindow) rotateButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Rotate", targetObject));
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

        deleteButton.onClick.AddListener(() => DeleteObject());
    }

    void Update()
    {
        if (targetObject == null) return;

        Vector3 worldPos;

        if (ViewState.CurrentMode == ViewMode.Mode2D)
        {
            //float offsetZ = 0.7f + targetObject.transform.localScale.z * 0.5f;
            //worldPos = targetObject.transform.position + new Vector3(0, 0, offsetZ);
            Renderer renderer = targetObject.GetComponent<Renderer>();
            float halfLengthZ = renderer.bounds.size.z * 0.5f;
            float extraOffset = 0.5f;

            float offsetZ = halfLengthZ + extraOffset;
            worldPos = targetObject.transform.position + new Vector3(0, 0, offsetZ);
        }
        else
        {
            Collider col = targetObject.GetComponent<Collider>();
            if (col != null)
            {
                Bounds bounds = col.bounds;
                float extraOffset = 0.6f;
                worldPos = bounds.center + Vector3.up * (bounds.extents.y + extraOffset);
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
