using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    // State of the tile
    // TODO: change how states work. Maybe get rid of them?
    //public TileState state { get; private set; }
    public Color backgroundColor { get; private set; }
    public Color textColor { get; private set; }
    public int number { get; private set; }
    // Tile stands in this cell right now
    public TileCell cell { get; private set; }
    // Is tile locked? Locked tiles can not be moved
    public bool isLocked { get; set; }

    private Image background;
    private TextMeshProUGUI text;
    private const float durationMoveAnimation = 0.1f;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
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
        backgroundColor = bgColor;
        textColor = tColor;
        number = numValue;

        background.color = backgroundColor;
        text.color = textColor;
        text.text = number.ToString();
    }
    // Spawn this tile
    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }
    // Move this tile to that 'cell'
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(MoveAnimation(cell.transform.position, false));
    }
    // Merge this tile with the other tile that stands on that 'cell'
    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;

        cell.tile.isLocked = true;
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
        float endTime = startTime + durationMoveAnimation;

        Vector3 current = transform.position;

        while(Time.time < endTime)
        {
            float t = (Time.time - startTime) / durationMoveAnimation;
            transform.position = Vector3.Lerp(current, target, t);
            yield return null;
        }

        transform.position = target;

        if (isMerging)
        {
            Destroy(gameObject);
        }
    }
}
