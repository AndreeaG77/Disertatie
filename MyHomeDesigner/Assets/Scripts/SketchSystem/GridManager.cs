using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;  // Number of grid cells horizontally
    public int height = 10; // Number of grid cells vertically
    public float cellSize = 1f; // Size of each cell
    public Material gridMaterial; // Assign in the inspector

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GameObject gridParent = new GameObject("GridLines");

        for (int x = 0; x <= width; x++)
        {
            CreateLine(new Vector3(x * cellSize, 0, 0), new Vector3(x * cellSize, 0, height * cellSize), gridParent);
        }

        for (int y = 0; y <= height; y++)
        {
            CreateLine(new Vector3(0, 0, y * cellSize), new Vector3(width * cellSize, 0, y * cellSize), gridParent);
        }
    }

    void CreateLine(Vector3 start, Vector3 end, GameObject parent)
    {
        GameObject line = new GameObject("GridLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });

        lr.startWidth = 0.02f; // Thickness of grid lines
        lr.endWidth = 0.02f;
        lr.material = gridMaterial;
        line.transform.SetParent(parent.transform);
    }
}
