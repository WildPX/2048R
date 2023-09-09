using UnityEngine;

public class TileCell : MonoBehaviour
{
    // Coordinates of this cell on the board
    public Vector2Int coordinates { get; set; }
    // Tile that stands on this cell
    public Tile tile { get; set; }
    // If this cell is empty then isEmpty = true
    public bool isEmpty => tile == null;
    // If this cell is occupied then isOccupied = true
    public bool isOccupied => tile != null;
}
