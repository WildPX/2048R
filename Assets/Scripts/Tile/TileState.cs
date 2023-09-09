using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]

public class TileState : ScriptableObject
{
    [SerializeField] private Color backgroundColor;
    [SerializeField] private Color textColor;
}
