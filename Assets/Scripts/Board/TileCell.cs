using UnityEngine;

public class TileCell : MonoBehaviour
{
    // Coordinates of this cell on the board
    public Vector2Int Coordinates { get; private set; }
    // Tile that stands on this cell
    public Tile ThisTile { get; private set; }
    // If this cell is empty then IsEmpty = true
    public bool IsEmpty => ThisTile == null;
    // If this cell is occupied then IsOccupied = true
    public bool IsOccupied => ThisTile != null;

    public void SetCoordinates(Vector2Int coord)
    {
        Coordinates = coord;
    }

    public void SetTile(Tile t)
    {
        ThisTile = t;
    }
}
