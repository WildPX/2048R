using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteScript : MonoBehaviour
{
    public float originalTime { get; set; }
    public bool isLocked { get; set; }
    public bool isMoving { get; set; }
    public float fixedTimeLength { get; set; }

    private float elapsedTime;
    //private float pauseStartTime;

    private Image noteImage;

    private Coroutine routine;
    private Color startNoteColor = Color.white;
    private Color endNoteColor = new Color(1f, 1f, 1f, 0f);
    private Vector3 noteLocalPos = new Vector3(-750, 50, 0);
    private Vector3 noteLocalPosEnd = new Vector3(750, 50, 0);
    private Vector3 targetPosition = new Vector3(0, 50, 0);

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
        isLocked = false;
        isMoving = false;
        transform.localPosition = noteLocalPos;

        noteImage = GetComponent<Image>();
        //startTime = Time.time;
        //pauseTime = 0f;
        //globalTime = Time.time - startTime;

        //timeToHit = originalTime;
        //Debug.Log(originalTime);
        //Debug.Log(timeToHit);
        elapsedTime = 0f;
        //pauseStartTime = 0f;
    }

    private void Update()
    {
        if (isLocked && routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        else if (!isLocked && !isMoving)
        {
            routine = StartCoroutine(MoveNote());
        }
    }

    private IEnumerator MoveNote()
    {
        isMoving = true;

        Vector3 startPos = transform.localPosition;

        if (startPos.x < targetPosition.x)
        {
            while (originalTime > fixedTimeLength)
            {
                originalTime -= Time.deltaTime;
                yield return null;
            }

            //Debug.Log("Here");
            while (transform.localPosition.x < targetPosition.x)
            {
                float t = elapsedTime / originalTime;
                //globalTime = Time.time - startTime - pauseTime;
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(noteLocalPos, targetPosition, t);
                //Debug.Log(transform.localPosition);
                yield return null;
            }

            startPos = transform.localPosition;
            elapsedTime = 0f;
        }

        //Debug.Log("CYCLE LEFT");

        Color currentColor = noteImage.color;

        if (startPos.x >= targetPosition.x)
        {
            //Debug.Log("Not here");
            while (elapsedTime < fixedTimeLength)
            {
                float t = elapsedTime / fixedTimeLength;
                elapsedTime += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(targetPosition, noteLocalPosEnd, t);
                noteImage.color = Color.Lerp(startNoteColor, endNoteColor, t);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    //private IEnumerator MoveNoteToCenter()
    //{
    //    isMoving = true;

    //    float startTime = Time.time - pauseStartTime;
    //    float endTime = startTime + pauseDuration;

    //    Vector3 startPos = transform.localPosition;
    //    elapsedTimeToCenter = 0f;

    //    while (Time.time < endTime)
    //    {
    //        float t = (Time.time - startTime) / (endTime - startTime);
    //        transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
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
    //    //    float t = elapsedTimeToCenter / timeToHit;
    //    //    transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
    //    //    elapsedTimeToCenter += Time.deltaTime;
    //    //    timeCheck -= Time.deltaTime;
    //    //    yield return null;
    //    //}

    //    transform.localPosition = targetPosition;

    //    isMoving = false;
    //}

    //private IEnumerator MoveNoteToEnd() 
    //{
    //    isMoving = true;

    //    Vector3 startPos = transform.localPosition;
    //    Image noteImage = GetComponent<Image>();

    //    while (elapsedTimeToEnd < fixedTimeLength)
    //    {
    //        float t = elapsedTimeToEnd / fixedTimeLength;
    //        transform.localPosition = Vector3.Lerp(startPos, noteLocalPosEnd, t);
    //        noteImage.color = Color.Lerp(endNoteColor, startNoteColor, t);
    //        elapsedTimeToEnd += Time.deltaTime;
    //        yield return null;
    //    }

    //    Destroy(gameObject);
    //}
}
