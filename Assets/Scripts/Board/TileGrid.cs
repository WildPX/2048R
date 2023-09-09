using UnityEngine;

public class TileGrid : MonoBehaviour
{
    // All the rows
    public TileRow[] rows { get; private set; }
    // All the cells
    public TileCell[] cells { get; private set; }
    // Size of the grid is its length
    public int size => cells.Length;
    // Height of the grid is rows' length
    public int height => rows.Length;
    // Width of the grid is the division of size and height
    public int width => size / height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }
    // Initialize all the coordinates for cells in rows
    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }
    // Get coordinates of the cell
    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return rows[y].cells[x];
        }
        else
        {
            return null;
        }
    }
    // Get coordinates of the cell from Vector2
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }
    // Get adjacent cell that stands infront of the current movement (Input from user)
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }
    // Find Empty Cell
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;

        while (cells[index].isOccupied)
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0;
            }

            if (startingIndex == index)
            {
                return null;
            }
        }

        return cells[index];
    }
}
