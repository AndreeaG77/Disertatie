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
        string[] categories = { "Rooms", "Doors", "Windows", "Kitchen", "LivingRoom", "Bedroom", "Bathroom", "Lighting" };

        foreach (string category in categories)
        {
            GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/" + category);
            Sprite[] loadedThumbnails = Resources.LoadAll<Sprite>("Thumbnails/" + category);
            List<FurnitureItem> furnitureList = new List<FurnitureItem>();

            foreach (GameObject prefab in loadedPrefabs)
            {
                Sprite matchingThumbnail = System.Array.Find(loadedThumbnails, thumb => thumb.name == prefab.name);
                FurnitureItem furnitureItem = new FurnitureItem(prefab.name, category, prefab, matchingThumbnail, DeterminePlacementType(category)
);

                furnitureList.Add(furnitureItem);
                allFurnitureItems.Add(furnitureItem);
            }

            if (furnitureList.Count > 0)
            {
                categorizedFurniture[category] = furnitureList;
            }
        }

    }

    //public List<FurnitureItem> GetAllFurniture()
    //{
    //   return new List<FurnitureItem>(allFurnitureItems);
    //}

    public List<FurnitureItem> GetAllFurniture()
    {
        return allFurnitureItems.FindAll(item =>
        {
            if (ViewState.CurrentMode == ViewMode.Mode2D)
                return item.placementType != PlacementType.Window && item.placementType != PlacementType.Door;

            if (ViewState.CurrentMode == ViewMode.Mode3D)
                return item.placementType != PlacementType.Room;

            return true;
        });
    }


    //public List<FurnitureItem> GetFurnitureByCategory(string category)
    //{
    //    return categorizedFurniture.ContainsKey(category) ? new List<FurnitureItem>(categorizedFurniture[category]) : new List<FurnitureItem>();
    //}

    public List<FurnitureItem> GetFurnitureByCategory(string category)
    {
        if (!categorizedFurniture.ContainsKey(category))
            return new List<FurnitureItem>();

        return categorizedFurniture[category].FindAll(item =>
        {
            if (ViewState.CurrentMode == ViewMode.Mode2D)
                return item.placementType != PlacementType.Window && item.placementType != PlacementType.Door;

            if (ViewState.CurrentMode == ViewMode.Mode3D)
                return item.placementType != PlacementType.Room;

            return true;
        });
    }


    //public List<FurnitureItem> SearchFurniture(string searchText)
    //{
    //    searchText = searchText.ToLower();
    //    return allFurnitureItems.FindAll(item => item.name.Trim().ToLower().StartsWith(searchText));
    //}

    public List<FurnitureItem> SearchFurniture(string searchText)
    {
        searchText = searchText.ToLower();
    
        return allFurnitureItems.FindAll(item =>
        {
            bool matches = item.name.Trim().ToLower().StartsWith(searchText);
    
            if (ViewState.CurrentMode == ViewMode.Mode2D)
                return matches && item.placementType != PlacementType.Window && item.placementType != PlacementType.Door;
    
            if (ViewState.CurrentMode == ViewMode.Mode3D)
                return matches && item.placementType != PlacementType.Room;
    
            return matches;
        });
    }


    private PlacementType DeterminePlacementType(string category)
{
    switch (category.ToLower())
    {
        case "rooms":
            return PlacementType.Room;
        case "windows":
            return PlacementType.Window;
        case "doors":
            return PlacementType.Door;
        default:
            return PlacementType.Furniture;
    }
}
}
