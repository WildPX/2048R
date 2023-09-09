using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile States")]
public class TileStates : ScriptableObject
{
    public List<Color> BackgroundColors;
    public List<Color> TextColors;
    public List<int> Numbers;
}
