using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    // This canvas
    //public Canvas canvas;
    // Tile prefab
    public Tile tilePrefab;


    // List of tile states
    // TODO: change states or remove them
    // MAYBE INSTEAD OF TILE STATES I CAN DO STATE WITH ALL OF THE NUMBERS AND COLORS?
    //public TileState[] tileStates;
    public TileStates tileStates;
    // Number of tiles. Can be changed if necessary
    //public int numberOfTiles = 16; // default value

    public bool GameOver { get; set; }

    // Logic for swipes with mouse
    private Vector3 startSwipePosition;
    private Vector3 endSwipePosition;
    // (1, 0) = right, (-1, 0) = left
    // (0, 1) = up, (0, -1) = down
    private Vector2Int direction;

    private bool isSwiping;
    private bool didSwipe;
    private const float minSwipeDistance = 50f;

    // Input time for RhythmController
    public float InputTime { get; private set; }
    /// Point system
    // Current number of points
    public int CurrentPoints { get; private set; }
    // Number of merges
    public int NumberOfMerges { get; private set; }
    // Number of points received from the current move
    public int CurrentMovePoints { get; private set; }
    // Locks the board if needed
    public bool InputLock { get; private set; }
    // TMGlobal = TimeManager from RhythmController. 
    // Initializes in RController
    public TimeManager TMGlobal { get; set; }
    // SFX Clik of a Note
    public AudioSource sfxClickNote;
    private bool shouldPlay;

    // Current grid of this board, children
    private TileGrid grid;
    // List of all tiles
    private List<Tile> tiles;
    // Is input available?
    public bool IsWaiting { get; private set; }
    //private bool lastInputCorrect;
    //private float globalElapsedTime;
    // How much to wait before another input?
    private const float waitDuration = 0.1f;
    private int numberOfTiles;

    // Rotation
    // EXPERIMENTAL. Doesn't work properly. Have to redo the way board moves on slide
    //private bool isBoardMoving = false;
    //private const float boardMoveDuration = 0.1f;
    //private Vector3 boardMoveOffset = new Vector3(0f, 5f, 0f);
    //private const float boardRotationOffset = 1.5f;

    private void Awake()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        GameOver = false;
        numberOfTiles = tileStates.Numbers.Count;
        IsWaiting = false;
        InputLock = false;
        InputTime = 0f;
        NumberOfMerges = 0;
        CurrentMovePoints = 0;
        isSwiping = false;
        didSwipe = false;
        direction.x = 0;
        direction.y = 0;
        if (PlayerPrefs.GetString("SFXClickNote") == "On")
        {
            shouldPlay = true;
        }
        else
        {
            shouldPlay = false;
        }

        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(numberOfTiles);
    }

    private void Update()
    {
        // If lock is activated (pause menu)
        if (InputLock)
        {
            if (isSwiping || didSwipe || direction != Vector2Int.zero)
            {
                isSwiping = false;
                didSwipe = false;
                direction = Vector2Int.zero;
            }
            return;
        }

        // Swipes Logic
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startSwipePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            endSwipePosition = Input.mousePosition;
            isSwiping = false;

            CheckMouseInput();
        }
        else
        {
            direction = Vector2Int.zero;
        }

        //globalElapsedTime += Time.deltaTime;
        //lastInputCorrect = false;
        // TODO: change how input works. Instead of WASD / Arrows use input states from PlayerPrefs
        // Make the default ones on WASD and Arrows
        if (!IsWaiting) // && !isBoardMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || (didSwipe && direction.y == 1))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || (didSwipe && direction.y == -1))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || (didSwipe && direction.x == -1))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || (didSwipe && direction.x == 1))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }
    // Clear this board before initializing tiles
    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }
    // Create default tile
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates.BackgroundColors[0], tileStates.TextColors[0], tileStates.Numbers[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }
    // When got input, move all tiles to this direction
    private void MoveTiles(Vector2Int direction,
                           int startX, int incrementX,
                           int startY, int incrementY)
    {
        bool stateChanged = false;

        // Nullify the number of merges and current move points
        NumberOfMerges = 0;
        CurrentMovePoints = 0;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.isOccupied)
                {
                    stateChanged |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (stateChanged)
        {
            if (shouldPlay)
            {
                sfxClickNote.Play();
            }
            //lastInputCorrect = true;
            InputTime = TMGlobal.GetGlobalTime();
            CurrentPoints += CurrentMovePoints * NumberOfMerges;
            StartCoroutine(WaitForChanges());
            //StartCoroutine(MoveAndRotateBoard(direction));
        }
    }
    // Move one tile
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        // Move Tile while there still is not occupied cell
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            // Merge Tiles
            if (adjacent.isOccupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }
    // Can we merge this two tiles?
    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.isLocked;
    }
    // Merging of two tiles
    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        //int index = Mathf.Clamp(GetIndexOfTileState(b.state) + 1, 0, tileStates.Length - 1);
        // index of a current tile state
        int index = GetIndexOfTileState(b) + 1;
        //Debug.Log($"index: {index}, numberOfTiles: {numberOfTiles}");
        // number to add
        int number;
        // change colors
        Color bgColor, tColor;

        if (index >= numberOfTiles)
        {
            number = b.number * 2;
            bgColor = tileStates.BackgroundColors[index % numberOfTiles];
            tColor = b.textColor;
            //Debug.Log($"{number}, {bgColor}, {tColor}");
        }
        else
        {
            number = tileStates.Numbers[index];
            bgColor = tileStates.BackgroundColors[index];
            tColor = tileStates.TextColors[index];
        }

        //int number = b.number * 2;

        b.SetState(bgColor, tColor, number);
        NumberOfMerges++;
        CurrentMovePoints += number;
        //CurrentPoints += number;
    }
    // Get current tile state
    //private int GetIndexOfTileState(TileState state)
    //{
    //    for (int i = 0; i < tileStates.Length; i++)
    //    {
    //        if (state == tileStates[i])
    //        {
    //            return i;
    //        }
    //    }

    //    return -1;
    //}
    // Using the fact that numbers from tiles are only powers of two
    private int GetIndexOfTileState(Tile t)
    {
        int index = 0;
        int number = t.number;

        while (number > 2)
        {
            number >>= 1;
            index++;
        }

        return index;
    }

    // EXPERIMENTAL. Does not work properly
    // Move board to direction of movement
    //private IEnumerator MoveAndRotateBoard(Vector2Int direction)
    //{
    //    while (IsWaiting)
    //    {
    //        yield return null;
    //    }
    //    isBoardMoving = true;

    //    bool isRotating = (direction == Vector2Int.left || direction == Vector2Int.right);
    //    Quaternion startRotation = transform.rotation;
    //    Quaternion targetRotation = Quaternion.Euler(0f, 0f, (direction == Vector2Int.left ? -boardRotationOffset : boardRotationOffset));
    //    Vector3 startPosition = transform.position;
    //    Vector3 targetPosition = startPosition + (isRotating ? Vector3.zero : boardMoveOffset);

    //    float elapsed = 0f;
    //    while (elapsed < boardMoveDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / boardMoveDuration);

    //        if (isRotating)
    //        {
    //            transform.rotation = Quaternion.LerpUnclamped(startRotation, targetRotation, t);
    //        }
    //        else
    //        {
    //            transform.position = Vector3.LerpUnclamped(startPosition, targetPosition, t);
    //        }

    //        yield return null;
    //    }

    //    transform.SetPositionAndRotation(targetPosition, targetRotation);
    //    yield return new WaitForSeconds(0.01f);
    //    transform.SetPositionAndRotation(startPosition, startRotation);

    //    isBoardMoving = false;
    //}

    // Coroutine for stopping input and creating new tile
    private IEnumerator WaitForChanges()
    {
        IsWaiting = true;
        yield return new WaitForSeconds(waitDuration);

        IsWaiting = false;

        foreach (var tile in tiles)
        {
            tile.isLocked = false;
        }

        if (tiles.Count != grid.size)
        {
            CreateTile();
        }

        if (CheckForGameOver())
        {
            GameOver = true;
        }
    }
    // Is it game over?
    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }
    public void SetCurrentPoints(int points)
    {
        CurrentPoints = points;
    }
    public void SetInputLock(bool state)
    {
        InputLock = state;
    }

    private void CheckMouseInput()
    {
        direction = Vector2Int.zero;
        Vector3 swipeDirection = endSwipePosition - startSwipePosition;

        float swipeHorizontal = Mathf.Abs(swipeDirection.x);
        float swipeVertical = Mathf.Abs(swipeDirection.y);

        if (swipeHorizontal > minSwipeDistance || swipeVertical > minSwipeDistance)
        {
            didSwipe = true;
            if (swipeHorizontal > swipeVertical)
            {
                if (swipeDirection.x > 0)
                {
                    direction.x = 1;
                }
                else
                {
                    direction.x = -1;
                }
            }
            else
            {
                if (swipeDirection.y > 0)
                {
                    direction.y = 1;
                }
                else
                {
                    direction.y = -1;
                }
            }
        }
    }
}