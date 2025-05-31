using UnityEngine;

[System.Serializable]
public class FurnitureItem
{
    public string name;
    public string category;
    public GameObject prefab;
    public Sprite thumbnail;
    public PlacementType placementType;
    public string price;

    public FurnitureItem(string name, string category, GameObject prefab, Sprite thumbnail, PlacementType placementType, string price = "0€")
    {
        this.name = name;
        this.category = category;
        this.prefab = prefab;
        this.thumbnail = thumbnail;
        this.placementType = placementType;
        this.price = price;
    }
}