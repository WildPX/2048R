using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    // State of the tile
    // TODO: change how states work. Maybe get rid of them?
    //public TileState state { get; private set; }
    public Color BackgroundColor { get; private set; }
    public Color TextColor { get; private set; }
    public int Number { get; private set; }
    public int PowerOfTwo { get; private set; }
    // Tile stands in this cell right now
    public TileCell Cell { get; private set; }
    // Is tile locked? Locked tiles can not be moved
    public bool IsLocked { get; private set; }

    private Image _background;
    private TextMeshProUGUI _text;
    private const float _durationMoveAnimation = 0.1f;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    // Set state in this tile
    //public void SetState(TileState curState, int curNumber)
    //{
    //    state = curState;
    //    number = curNumber;

    //    background.color = state.backgroundColor;
    //    text.color = state.textColor;
    //    text.text = number.ToString();
    //}
    public void SetState(Color bgColor, Color tColor, int numValue)
    {
        BackgroundColor = bgColor;
        TextColor = tColor;
        Number = numValue;

        _background.color = BackgroundColor;
        _text.color = TextColor;
        _text.text = Number.ToString();
    }
    // Spawn this tile
    public void Spawn(TileCell cell)
    {
        if (this.Cell != null)
        {
            this.Cell.SetTile(null);
        }

        this.Cell = cell;
        this.Cell.SetTile(this);

        transform.position = cell.transform.position;
    }
    // Move this tile to that 'cell'
    public void MoveTo(TileCell cell)
    {
        if (this.Cell != null)
        {
            this.Cell.SetTile(null);
        }

        this.Cell = cell;
        this.Cell.SetTile(this);

        StartCoroutine(MoveAnimation(cell.transform.position, false));
    }
    // Merge this tile with the other tile that stands on that 'cell'
    public void Merge(TileCell cell)
    {
        if (this.Cell != null)
        {
            this.Cell.SetTile(null);
        }

        this.Cell = null;

        cell.ThisTile.IsLocked = true;
        //StartCoroutine(MoveAnimation(cell.transform.position, true));
        StartCoroutine(MoveAnimation(cell.transform.position, true));
    }
    // Coroutine for movement
    // TODO: Rework this one
    //private IEnumerator MoveAnimation(Vector3 target, bool isMerging)
    //{
    //    float elapsed = 0f;

    //    Vector3 current = transform.position;

    //    while (elapsed < durationMoveAnimation)
    //    {
    //        transform.position = Vector3.Lerp(current, target, elapsed / durationMoveAnimation);
    //        elapsed += Time.deltaTime;
    //        //Debug.Log($"FDT:{Time.smoothDeltaTime}");
    //        //Debug.Log($"Elapsed:{elapsed}");
    //        yield return null;
    //    }

    //    transform.position = target;

    //    if (isMerging)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private IEnumerator MoveAnimation(Vector3 target, bool isMerging)
    {
        float startTime = Time.time;
        float endTime = startTime + _durationMoveAnimation;

        Vector3 current = transform.position;

        while(Time.time < endTime)
        {
            float t = (Time.time - startTime) / _durationMoveAnimation;
            transform.position = Vector3.Lerp(current, target, t);
            yield return null;
        }

        transform.position = target;

        if (isMerging)
        {
            Destroy(gameObject);
        }
    }

    public void SetLocked(bool state)
    {
        IsLocked = state;
    }
}
