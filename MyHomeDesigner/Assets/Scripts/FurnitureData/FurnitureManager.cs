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
        string[] categories = { "Rooms", "Doors", "Windows", "Kitchen", "LivingRoom", "Bedroom", "Bathroom", "Miscellaneous" };

        foreach (string category in categories)
        {
            GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/" + category);
            Sprite[] loadedThumbnails = Resources.LoadAll<Sprite>("Thumbnails/" + category);
            List<FurnitureItem> furnitureList = new List<FurnitureItem>();

            foreach (GameObject prefab in loadedPrefabs)
            {
                Sprite matchingThumbnail = System.Array.Find(loadedThumbnails, thumb => thumb.name == prefab.name);
                string price = FurniturePriceDatabase.GetPrice(category, prefab.name);
                if (category == "Rooms")
                    price = "-";
                FurnitureItem furnitureItem = new FurnitureItem(prefab.name, category, prefab, matchingThumbnail, DeterminePlacementType(prefab), price);

                furnitureList.Add(furnitureItem);
                allFurnitureItems.Add(furnitureItem);
            }

            if (furnitureList.Count > 0)
            {
                categorizedFurniture[category] = furnitureList;
            }
        }

    }

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
    
    private PlacementType DeterminePlacementType(GameObject prefab)
    {
        int layer = prefab.layer;

        if (layer == LayerMask.NameToLayer("Room"))
            return PlacementType.Room;
        if (layer == LayerMask.NameToLayer("Door"))
            return PlacementType.Door;
        if (layer == LayerMask.NameToLayer("Window"))
            return PlacementType.Window;

        return PlacementType.Furniture;
    }
}
