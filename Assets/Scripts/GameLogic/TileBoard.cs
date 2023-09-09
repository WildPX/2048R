using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    // This canvas
    //public Canvas canvas;
    // Tile prefab
    [SerializeField] private Tile _tilePrefab;


    // List of tile states
    // TODO: change states or remove them
    // MAYBE INSTEAD OF TILE STATES I CAN DO STATE WITH ALL OF THE NUMBERS AND COLORS?
    //public TileState[] tileStates;
    [SerializeField] private TileStates _tileStates;
    // Number of tiles. Can be changed if necessary
    //public int numberOfTiles = 16; // default value

    public bool GameOver { get; private set; }

    // Logic for swipes with mouse
    private Vector3 _startSwipePosition;
    private Vector3 _endSwipePosition;
    // (1, 0) = right, (-1, 0) = left
    // (0, 1) = up, (0, -1) = down
    private Vector2Int _direction;

    private bool _isSwiping;
    private bool _didSwipe;
    private const float _minSwipeDistance = 50f;

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
    public TimeManager TMGlobal { get; private set; }
    // SFX Clik of a Note
    [SerializeField] private AudioSource _sfxClickNote;
    private bool _shouldPlay;

    // Current grid of this board, children
    private TileGrid _grid;
    // List of all tiles
    private List<Tile> _tiles;
    // Is input available?
    public bool IsWaiting { get; private set; }
    //private bool lastInputCorrect;
    //private float globalElapsedTime;
    // How much to wait before another input?
    private const float WAIT_DURATION = 0.1f;
    private int _numberOfTiles;

    // Rotation
    // EXPERIMENTAL. Doesn't work properly. Have to work on the way board moves on slide
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
        _numberOfTiles = _tileStates.Numbers.Count;
        IsWaiting = false;
        InputLock = false;
        InputTime = 0f;
        NumberOfMerges = 0;
        CurrentMovePoints = 0;
        _isSwiping = false;
        _didSwipe = false;
        _direction.x = 0;
        _direction.y = 0;
        if (PlayerPrefs.GetString("SFXClickNote") == "On")
        {
            _shouldPlay = true;
        }
        else
        {
            _shouldPlay = false;
        }

        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(_numberOfTiles);
    }

    private void Update()
    {
        // If lock is activated (pause menu)
        if (InputLock)
        {
            if (_isSwiping || _didSwipe || _direction != Vector2Int.zero)
            {
                _isSwiping = false;
                _didSwipe = false;
                _direction = Vector2Int.zero;
            }
            return;
        }

        // Swipes Logic
        if (Input.GetMouseButtonDown(0))
        {
            _isSwiping = true;
            _startSwipePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && _isSwiping)
        {
            _endSwipePosition = Input.mousePosition;
            _isSwiping = false;

            CheckMouseInput();
        }
        else
        {
            _direction = Vector2Int.zero;
        }

        //globalElapsedTime += Time.deltaTime;
        //lastInputCorrect = false;
        // TODO: change how input works. Instead of WASD / Arrows use input states from PlayerPrefs
        // Make the default ones on WASD and Arrows
        if (!IsWaiting) // && !isBoardMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || (_didSwipe && _direction.y == 1))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || (_didSwipe && _direction.y == -1))
            {
                MoveTiles(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) || (_didSwipe && _direction.x == -1))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || (_didSwipe && _direction.x == 1))
            {
                MoveTiles(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
            }
        }
    }
    // Clear this board before initializing tiles
    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells)
        {
            cell.SetTile(null);
        }

        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }
    // Create default tile
    public void CreateTile()
    {
        Tile tile = Instantiate(_tilePrefab, _grid.transform);
        tile.SetState(_tileStates.BackgroundColors[0], _tileStates.TextColors[0], _tileStates.Numbers[0]);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
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

        for (int x = startX; x >= 0 && x < _grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.Height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);
                if (cell.IsOccupied)
                {
                    stateChanged |= MoveTile(cell.ThisTile, direction);
                }
            }
        }

        if (stateChanged)
        {
            if (_shouldPlay)
            {
                _sfxClickNote.Play();
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
        TileCell adjacent = _grid.GetAdjacentCell(tile.Cell, direction);

        while (adjacent != null)
        {
            // Merge Tiles
            if (adjacent.IsOccupied)
            {
                if (CanMerge(tile, adjacent.ThisTile))
                {
                    Merge(tile, adjacent.ThisTile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = _grid.GetAdjacentCell(adjacent, direction);
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
        return a.Number == b.Number && !b.IsLocked;
    }
    // TODO: REWRITE THE LOGIC
    private void Merge(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b.Cell);

        //int index = Mathf.Clamp(GetIndexOfTileState(b.state) + 1, 0, tileStates.Length - 1);
        // index of a current tile state
        int index = GetIndexOfTileState(b) + 1;
        //Debug.Log($"index: {index}, numberOfTiles: {numberOfTiles}");
        // number to add
        int number;
        // change colors
        Color bgColor, tColor;

        if (index >= _numberOfTiles)
        {
            number = b.Number * 2;
            bgColor = _tileStates.BackgroundColors[index % _numberOfTiles];
            tColor = b.TextColor;
            //Debug.Log($"{number}, {bgColor}, {tColor}");
        }
        else
        {
            number = _tileStates.Numbers[index];
            bgColor = _tileStates.BackgroundColors[index];
            tColor = _tileStates.TextColors[index];
        }

        //int number = b.number * 2;

        b.SetState(bgColor, tColor, number);
        NumberOfMerges++;
        CurrentMovePoints += number;
        //CurrentPoints += number;
    }
    // Using the fact that numbers from tiles are only powers of two
    private int GetIndexOfTileState(Tile t)
    {
        int index = 0;
        int number = t.Number;

        while (number > 2)
        {
            number >>= 1;
            index++;
        }

        return index;
    }

    // Coroutine for stopping input and creating new tile
    private IEnumerator WaitForChanges()
    {
        IsWaiting = true;
        yield return new WaitForSeconds(WAIT_DURATION);

        IsWaiting = false;

        foreach (var tile in _tiles)
        {
            tile.SetLocked(false);
        }

        if (_tiles.Count != _grid.Size)
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
        if (_tiles.Count != _grid.Size)
        {
            return false;
        }

        foreach (var tile in _tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);
            TileCell left = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
            TileCell right = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.ThisTile))
            {
                return false;
            }

            if (down != null && CanMerge(tile, down.ThisTile))
            {
                return false;
            }

            if (left != null && CanMerge(tile, left.ThisTile))
            {
                return false;
            }

            if (right != null && CanMerge(tile, right.ThisTile))
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
        _direction = Vector2Int.zero;
        Vector3 swipeDirection = _endSwipePosition - _startSwipePosition;

        float swipeHorizontal = Mathf.Abs(swipeDirection.x);
        float swipeVertical = Mathf.Abs(swipeDirection.y);

        if (swipeHorizontal > _minSwipeDistance || swipeVertical > _minSwipeDistance)
        {
            _didSwipe = true;
            if (swipeHorizontal > swipeVertical)
            {
                if (swipeDirection.x > 0)
                {
                    _direction.x = 1;
                }
                else
                {
                    _direction.x = -1;
                }
            }
            else
            {
                if (swipeDirection.y > 0)
                {
                    _direction.y = 1;
                }
                else
                {
                    _direction.y = -1;
                }
            }
        }
    }

    public void SetTimeManager(TimeManager tm)
    {
        TMGlobal = tm;
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
}