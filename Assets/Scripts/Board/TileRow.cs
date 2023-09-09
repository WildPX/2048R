using UnityEngine;

public class TileRow : MonoBehaviour
{
    // Array with all of the cells from this row
    public TileCell[] Cells { get; private set; }
    private void Awake()
    {
        // Get every cell in this row and put it into array
        Cells = GetComponentsInChildren<TileCell>();
    }
}
