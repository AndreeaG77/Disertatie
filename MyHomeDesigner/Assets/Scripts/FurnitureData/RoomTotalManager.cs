using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomTotalManager : MonoBehaviour
{
    public GameObject roomTotalLabelPrefab;
    public Button showTotalButton;
    public Button hideTotalButton;
    public Transform labelCanvasParent; 
    //private bool totalsAreShown = false;
    public RoomTotalManager roomTotalManager;
    public bool totalsAreShown { get; set; }



    private Dictionary<GameObject, GameObject> roomToLabel = new(); 

    private void Start()
    {
        showTotalButton.onClick.AddListener(ShowTotals);
        hideTotalButton.onClick.AddListener(HideTotals);
        hideTotalButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        bool is3D = ViewState.CurrentMode == ViewMode.Mode3D;

        if (is3D)
        {
            if (hideTotalButton.gameObject.activeSelf)
            {
                foreach (var label in roomToLabel.Values)
                {
                    if (label != null)
                        Destroy(label);
                }
                roomToLabel.Clear();
            }
            showTotalButton.gameObject.SetActive(false);
            hideTotalButton.gameObject.SetActive(false);
            totalsAreShown = false;
        }
        else
        {
            showTotalButton.gameObject.SetActive(!totalsAreShown);
        }
    }


    public void ShowTotals()
    {
        totalsAreShown = true;
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> rooms = new();

        foreach (var obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Room") && obj.transform.parent == null)
                rooms.Add(obj);
        }

        foreach (GameObject room in rooms)
        {
            float total = 0f;
            SelectableFurniture[] allFurniture = Object.FindObjectsByType<SelectableFurniture>(FindObjectsSortMode.None);

            foreach (var furn in allFurniture)
            {
                if (furn.assignedRoom == room)
                {
                    if (float.TryParse(furn.price.Replace("€", "").Trim(), out float price))
                        total += price;
                }
            }

            GameObject label = Instantiate(roomTotalLabelPrefab, labelCanvasParent);
            FollowRoom follow = label.AddComponent<FollowRoom>();
            follow.roomTransform = room.transform;
            //label.GetComponent<TextMeshProUGUI>().text = $"{room.name}: {total} €";
            label.GetComponentInChildren<TextMeshProUGUI>().text = $"{total} €";

            //Vector3 labelPos = room.transform.position + new Vector3(0, 0, 3f);
            //label.transform.position = Camera.main.WorldToScreenPoint(labelPos);

            roomToLabel[room] = label;
        }

        showTotalButton.gameObject.SetActive(false);
        hideTotalButton.gameObject.SetActive(true);
    }

    public void HideTotals()
    {
        foreach (var label in roomToLabel.Values)
        {
            if (label != null)
                Destroy(label);
        }

        roomToLabel.Clear();
        showTotalButton.gameObject.SetActive(true);
        hideTotalButton.gameObject.SetActive(false);
        totalsAreShown = false;
    }
}
