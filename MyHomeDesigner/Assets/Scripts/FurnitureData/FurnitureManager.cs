using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    public static FurnitureManager Instance;

    private Dictionary<string, List<FurnitureItem>> categorizedFurniture = new Dictionary<string, List<FurnitureItem>>();
    private List<FurnitureItem> allFurnitureItems = new List<FurnitureItem>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadFurnitureFromResources();
    }

    void LoadFurnitureFromResources()
    {
        string[] categories = { "Rooms", "Doors", "Windows", "Kitchen", "LivingRoom", "Bedroom", "Bathroom", "Lighting" }; // Add more if needed

        foreach (string category in categories)
        {
            GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/" + category);
            Sprite[] loadedThumbnails = Resources.LoadAll<Sprite>("Thumbnails/" + category);
            List<FurnitureItem> furnitureList = new List<FurnitureItem>();
            //Debug.Log(category + " " + loadedThumbnails.Length);

            foreach (GameObject prefab in loadedPrefabs)
            {
                // Try to find the matching thumbnail by name
                Sprite matchingThumbnail = System.Array.Find(loadedThumbnails, thumb => thumb.name == prefab.name);
                //if (matchingThumbnail != null)
                //{
                //    
                //    Debug.Log("Thumbnail set for prefab: " + prefab.name);
                //}
                // Create FurnitureData
                FurnitureItem furnitureItem = new FurnitureItem(prefab.name, category, prefab, matchingThumbnail);
                furnitureList.Add(furnitureItem);
                allFurnitureItems.Add(furnitureItem);
            }

            if (furnitureList.Count > 0)
            {
                categorizedFurniture[category] = furnitureList;
            }
        }

        Debug.Log("Loaded " + allFurnitureItems.Count + " furniture items from Resources.");
    }

    public List<FurnitureItem> GetAllFurniture()
    {
        return new List<FurnitureItem>(allFurnitureItems);
    }

    public List<FurnitureItem> GetFurnitureByCategory(string category)
    {
        return categorizedFurniture.ContainsKey(category) ? new List<FurnitureItem>(categorizedFurniture[category]) : new List<FurnitureItem>();
    }

    public List<FurnitureItem> SearchFurniture(string searchText)
    {
        searchText = searchText.ToLower();
        return allFurnitureItems.FindAll(item => item.name.ToLower().Contains(searchText));
    }
}
