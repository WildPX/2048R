using UnityEngine;

public class TileGrid : MonoBehaviour
{
    // All the rows
    public TileRow[] Rows { get; private set; }
    // All the cells
    public TileCell[] Cells { get; private set; }
    // Size of the grid is its length
    public int Size => Cells.Length;
    // Height of the grid is rows' length
    public int Height => Rows.Length;
    // Width of the grid is the division of size and height
    public int Width => Size / Height;

    private void Awake()
    {
        Rows = GetComponentsInChildren<TileRow>();
        Cells = GetComponentsInChildren<TileCell>();
    }
    // Initialize all the coordinates for cells in rows
    private void Start()
    {
        for (int y = 0; y < Rows.Length; y++)
        {
            for (int x = 0; x < Rows[y].Cells.Length; x++)
            {
                Rows[y].Cells[x].SetCoordinates(new Vector2Int(x, y));
            }
        }
    }
    // Get coordinates of the cell
    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return Rows[y].Cells[x];
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
        Vector2Int coordinates = cell.Coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }
    // Find Empty Cell
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, Cells.Length);
        int startingIndex = index;

        while (Cells[index].IsOccupied)
        {
            index++;

            if (index >= Cells.Length)
            {
                index = 0;
            }

            if (startingIndex == index)
            {
                return null;
            }
        }

        return Cells[index];
    }
}
