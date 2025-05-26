using UnityEngine;
using UnityEngine.UI;

public class FurnitureToolPanel : MonoBehaviour
{
    public Button moveButton;
    public Button rotateButton;
    public Button scaleButton;
    public Button scaleXButton;
    public Button scaleZButton;
    public Button deleteButton;

    private GameObject targetObject;

    public void Initialize(GameObject target)
    {
        targetObject = target;


        moveButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Move", targetObject));
        rotateButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Rotate", targetObject));
        scaleButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("Scale", targetObject));
        scaleXButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleX", targetObject));
        scaleZButton.onClick.AddListener(() => FurnitureManipulator.Instance.SetMode("ScaleZ", targetObject));
        deleteButton.onClick.AddListener(() => DeleteObject());
        
        //FurnitureManipulator.Instance.ClearMode();
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
            float offsetY = 0.7f + targetObject.transform.localScale.y * 0.5f;
            worldPos = targetObject.transform.position + new Vector3(0, offsetY, 0);
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
