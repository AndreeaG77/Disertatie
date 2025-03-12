using UnityEngine;

[System.Serializable]
public class FurnitureItem
{
    public string name;
    public string category;
    public GameObject prefab;
    public Sprite thumbnail;

    public FurnitureItem(string name, string category, GameObject prefab, Sprite thumbnail)
    {
        this.name = name;
        this.category = category;
        this.prefab = prefab;
        this.thumbnail = thumbnail;
    }
}