using UnityEngine;
using UnityEngine.UI;

public class MoveToolPanel : MonoBehaviour
{
    public Button moveXButton;
    public Button moveYButton;
    public Button moveZButton;
    private GameObject targetObject;

    public void Initialize(GameObject target)
    {
        targetObject = target;
    }

    public static MoveToolPanel Instance;

    void Awake()
    {
        Instance = this;

        moveXButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMoveAxis("X"));
        moveYButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMoveAxis("Y"));
        moveZButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMoveAxis("Z"));
    }

    public void Show(GameObject target)
    {
        targetObject = target;
        gameObject.SetActive(true);
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
