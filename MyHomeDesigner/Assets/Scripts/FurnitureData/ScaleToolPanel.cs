using UnityEngine;
using UnityEngine.UI;

public class ScaleToolPanel : MonoBehaviour
{
    public Button scaleButton;
    public Button scaleXButton;
    public Button scaleYButton;
    public Button scaleZButton;

    private GameObject targetObject;

    public void Initialize(GameObject target)
    {
        targetObject = target;

        int layer = target.layer;
        bool isDoorOrWindow = (layer == LayerMask.NameToLayer("Door") || layer == LayerMask.NameToLayer("Window"));

        scaleButton.gameObject.SetActive(!isDoorOrWindow);
        scaleXButton.gameObject.SetActive(true);

        if (ViewState.CurrentMode == ViewMode.Mode2D)
            scaleYButton.gameObject.SetActive(false);
        else
            scaleYButton.gameObject.SetActive(true);

        scaleZButton.gameObject.SetActive(!isDoorOrWindow);

        if (!isDoorOrWindow) scaleButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Scale", targetObject));
        scaleXButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleX", targetObject));
        if (ViewState.CurrentMode == ViewMode.Mode3D)
            scaleYButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleY", targetObject));
        if (!isDoorOrWindow)
            scaleZButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleZ", targetObject));


    
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
}
