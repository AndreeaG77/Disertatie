using UnityEngine;
using UnityEngine.UI;

public class MoveToolPanel : MonoBehaviour
{
    public Button moveXButton;
    public Button moveYButton;
    public Button moveZButton;
    //public Button backButton;

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

        /*backButton.onClick.AddListener(() =>
        {
            FurnitureManipulator.Instance.SetMoveAxis("Free");
            FurnitureManipulator.Instance.ClearMode();
            gameObject.SetActive(false);
            FindFirstObjectByType<FurnitureToolPanel>().gameObject.SetActive(true);
        });*/
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
    }
}
