using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class FurnitureMenu : MonoBehaviour
{
    [Header("UI References")]
    public Transform furnitureContentPanel;
    public GameObject furniturePanel; // Parent for furniture buttons
    public GameObject furnitureItemPrefab; // Prefab for each furniture item in UI
    public TMP_InputField searchInputField; // Search bar input field
    public Button exitButton; // Exit button
    public Button menuButton;

    private List<FurnitureItem> allFurnitureItems = new List<FurnitureItem>(); // Stores all items
    private List<GameObject> spawnedFurnitureUI = new List<GameObject>(); // Stores created UI elements

    void Start()
    {
        searchInputField.onValueChanged.AddListener(OnSearchValueChanged);
        DisplayFurnitureItems(FurnitureManager.Instance.GetAllFurniture());
        allFurnitureItems = FurnitureManager.Instance.GetAllFurniture();
        exitButton.onClick.AddListener(CloseMenu);
    }
    
    public void DisplayFurnitureItems(List<FurnitureItem> itemsToShow)
    {
        ClearFurnitureUI();
        //Debug.Log("Displaying " + itemsToShow.Count + " furniture items");
        foreach (FurnitureItem item in itemsToShow)
        {
            GameObject newItem = Instantiate(furnitureItemPrefab, furnitureContentPanel);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.name;
            newItem.transform.Find("Image").GetComponent<Image>().sprite = item.thumbnail;
            //Debug.Log(item.name + "\n");
            newItem.GetComponent<Button>().onClick.AddListener(() => OnFurnitureSelected(item));

            spawnedFurnitureUI.Add(newItem);
        }
    }

    public void OnCategorySelected(string category)
    {
        searchInputField.text = ""; // Clear search when switching category
        List<FurnitureItem> filteredItems = allFurnitureItems.FindAll(item => item.category == category);
        DisplayFurnitureItems(filteredItems);
    }

    public void OnSearchValueChanged(string searchText)
    {
        List<FurnitureItem> filteredItems = allFurnitureItems.FindAll(item => item.name.ToLower().Contains(searchText.ToLower()));
        DisplayFurnitureItems(filteredItems);
    }

    public void OnFurnitureSelected(FurnitureItem item)
    {
        Debug.Log("Selected: " + item.name);
    }

    public void ClearFurnitureUI()
    {
        foreach (GameObject obj in spawnedFurnitureUI)
            Destroy(obj);
        spawnedFurnitureUI.Clear();
    }

    public void CloseMenu()
    {
        furniturePanel.SetActive(false);
        menuButton.gameObject.SetActive(true);
    }

    public void OpenMenu()
    {
        furniturePanel.SetActive(true);
        menuButton.gameObject.SetActive(false);
    }
}

