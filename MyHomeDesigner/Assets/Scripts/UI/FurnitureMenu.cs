using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class FurnitureMenu : MonoBehaviour
{
    [Header("UI References")]
    public Canvas canvas;
    public Image dragPreviewImage;
    public Transform furnitureContentPanel;
    public GameObject furniturePanel; 
    public GameObject furnitureItemPrefab;
    public TMP_InputField searchInputField; 
    public Button exitButton; 
    public Button menuButton;
    public GameObject roomsCategoryButton;
    public GameObject doorsCategoryButton;
    public GameObject windowsCategoryButton;


    private List<FurnitureItem> allFurnitureItems = new List<FurnitureItem>(); 
    private List<GameObject> spawnedFurnitureUI = new List<GameObject>(); 

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
        foreach (FurnitureItem item in itemsToShow)
        {
            GameObject newItem = Instantiate(furnitureItemPrefab, furnitureContentPanel);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.name;
            newItem.transform.Find("Image").GetComponent<Image>().sprite = item.thumbnail;
            Transform priceTagTransform = newItem.transform.Find("PriceTagParent/PriceTag");
            if (priceTagTransform != null)
            {
                TextMeshProUGUI priceText = priceTagTransform.GetComponent<TextMeshProUGUI>();
                if (priceText != null)
                {
                    /*if (item.category == "Rooms")
                    {
                        priceTagTransform.gameObject.SetActive(false);
                    }
                    else*/
                    priceText.text = item.price;
                }
            }

            newItem.GetComponent<Button>().onClick.AddListener(() => OnFurnitureSelected(item));



            FurnitureDragHandler dragHandler = newItem.GetComponent<FurnitureDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.furnitureItem = item;
                dragHandler.canvas = canvas;
                dragHandler.dragPreviewImage = dragPreviewImage;
            }


            spawnedFurnitureUI.Add(newItem);
        }
    }

    public void OnCategorySelected(string category)
    {
        searchInputField.text = ""; 
        List<FurnitureItem> filteredItems = allFurnitureItems.FindAll(item => item.category == category);
        DisplayFurnitureItems(filteredItems);
    }

    public void OnSearchValueChanged(string searchText)
    {
        List<FurnitureItem> filteredItems = allFurnitureItems.FindAll(item => item.name.ToLower().StartsWith(searchText.ToLower()));
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

    public void RefreshUI()
    {
        allFurnitureItems = FurnitureManager.Instance.GetAllFurniture();

        if (!string.IsNullOrEmpty(searchInputField.text))
        {
            OnSearchValueChanged(searchInputField.text);
        }
        else
        {
            DisplayFurnitureItems(allFurnitureItems);
        }
    }

    public void RefreshCategoryButtons(string mode)
    {
        bool is2D = mode == "2D";

        if(is2D)
        {
            doorsCategoryButton.SetActive(!is2D);
            windowsCategoryButton.SetActive(!is2D);
            roomsCategoryButton.SetActive(is2D);
        }
        else
        {
            roomsCategoryButton.SetActive(is2D);
            doorsCategoryButton.SetActive(!is2D);
            windowsCategoryButton.SetActive(!is2D);
        }
        
    }


}

