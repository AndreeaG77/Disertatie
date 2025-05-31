using UnityEngine;

public class FollowRoom : MonoBehaviour
{
    public Transform roomTransform;

    void Update()
    {
        if (Camera.main != null && roomTransform != null)
        {
            Collider col = roomTransform.GetComponentInChildren<Collider>();
            if (col != null)
            {
                Bounds bounds = col.bounds;
                float offsetZ = 1f;
                Vector3 cornerWorldPos = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z + offsetZ);

                Vector3 screenPos = Camera.main.WorldToScreenPoint(cornerWorldPos);
                GetComponent<RectTransform>().position = screenPos;
            }
        }
    }
}
