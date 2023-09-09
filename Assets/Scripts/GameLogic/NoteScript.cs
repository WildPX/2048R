using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteScript : MonoBehaviour
{
    public float OriginalTime { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsMoving { get; private set; }
    public float FixedTimeLength { get; private set; }

    private float _elapsedTime;
    //private float pauseStartTime;

    private Image _noteImage;

    private Coroutine _routine;
    private Color _startNoteColor = Color.white;
    private Color _endNoteColor = new Color(1f, 1f, 1f, 0f);
    private Vector3 _noteLocalPos = new Vector3(-750, 50, 0);
    private Vector3 _noteLocalPosEnd = new Vector3(750, 50, 0);
    private Vector3 _targetPosition = new Vector3(0, 50, 0);

    //private float globalTime;
    //private float startTime;
    //private float pauseTime;
    //private float timeToHit;

    //private TimeManager TMLocal;

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        IsLocked = false;
        IsMoving = false;
        transform.localPosition = _noteLocalPos;

        _noteImage = GetComponent<Image>();
        //startTime = Time.time;
        //pauseTime = 0f;
        //globalTime = Time.time - startTime;

        //timeToHit = originalTime;
        //Debug.Log(originalTime);
        //Debug.Log(timeToHit);
        _elapsedTime = 0f;
        //pauseStartTime = 0f;
    }

    private void Update()
    {
        if (IsLocked && _routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        else if (!IsLocked && !IsMoving)
        {
            _routine = StartCoroutine(MoveNote());
        }
    }

    private IEnumerator MoveNote()
    {
        IsMoving = true;

        Vector3 startPos = transform.localPosition;

        if (startPos.x < _targetPosition.x)
        {
            while (OriginalTime > FixedTimeLength)
            {
                OriginalTime -= Time.deltaTime;
                yield return null;
            }

            //Debug.Log("Here");
            while (transform.localPosition.x < _targetPosition.x)
            {
                float t = _elapsedTime / OriginalTime;
                //globalTime = Time.time - startTime - pauseTime;
                _elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_noteLocalPos, _targetPosition, t);
                //Debug.Log(transform.localPosition);
                yield return null;
            }

            startPos = transform.localPosition;
            _elapsedTime = 0f;
        }

        //Debug.Log("CYCLE LEFT");

        Color currentColor = _noteImage.color;

        if (startPos.x >= _targetPosition.x)
        {
            //Debug.Log("Not here");
            while (_elapsedTime < FixedTimeLength)
            {
                float t = _elapsedTime / FixedTimeLength;
                _elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_targetPosition, _noteLocalPosEnd, t);
                _noteImage.color = Color.Lerp(_startNoteColor, _endNoteColor, t);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    public void SetOriginalTime(float otime)
    {
        OriginalTime = otime;
    }

    public void SetLocked(bool state)
    {
        IsLocked = state;
    }

    public void SetFixedTimeLength(float time)
    {
        FixedTimeLength = time;
    }

    public void SetMoving(bool state)
    {
        IsMoving = state;
    }

    //private IEnumerator MoveNoteToCenter()
    //{
    //    IsMoving = true;

    //    float startTime = Time.time - pauseStartTime;
    //    float endTime = startTime + pauseDuration;

    //    Vector3 startPos = transform.localPosition;
    //    _elapsedTimeToCenter = 0f;

    //    while (Time.time < endTime)
    //    {
    //        float t = (Time.time - startTime) / (endTime - startTime);
    //        transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
    //        yield return null;
    //    }

    //    //while (timeToHit > fixedTimeLength)
    //    //{
    //    //    timeToHit -= Time.deltaTime;
    //    //    yield return null;
    //    //    timeCheck = timeToHit;
    //    //}
    //    ////Debug.Log(timeCheck);

    //    //while (timeCheck >= 0f)
    //    //{
    //    //    float t = _elapsedTimeToCenter / timeToHit;
    //    //    transform.localPosition = Vector3.Lerp(startPos, _targetPosition, t);
    //    //    _elapsedTimeToCenter += Time.deltaTime;
    //    //    timeCheck -= Time.deltaTime;
    //    //    yield return null;
    //    //}

    //    transform.localPosition = _targetPosition;

    //    IsMoving = false;
    //}

    //private IEnumerator MoveNoteToEnd() 
    //{
    //    IsMoving = true;

    //    Vector3 startPos = transform.localPosition;
    //    Image noteImage = GetComponent<Image>();

    //    while (_elapsedTimeToEnd < fixedTimeLength)
    //    {
    //        float t = _elapsedTimeToEnd / fixedTimeLength;
    //        transform.localPosition = Vector3.Lerp(startPos, _noteLocalPosEnd, t);
    //        noteImage.color = Color.Lerp(_endNoteColor, startNoteColor, t);
    //        _elapsedTimeToEnd += Time.deltaTime;
    //        yield return null;
    //    }

    //    Destroy(gameObject);
    //}
}
