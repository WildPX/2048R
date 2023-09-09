//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class NoteFieldController : MonoBehaviour
//{
//    public RhythmController rhythmController;
//    public GameObject markerPrefab;

//    private ScrollRect scrollRect;
//    private float trackDuration;
//    private List<float> beatTimes; // ћассив временных меток ударов
//    private RectTransform contentRectTransform;

//    private void Start()
//    {
//        scrollRect = GetComponent<ScrollRect>();
//        beatTimes = rhythmController.CurrentMusicData.beatFrames;
//        trackDuration = rhythmController.CurrentMusicData.time;

//        contentRectTransform = scrollRect.content;

//        // —оздание точек на ScrollRect дл€ каждой временной метки
//        foreach (float beatTime in beatTimes)
//        {
//            GameObject marker = Instantiate(markerPrefab, contentRectTransform);
//            PositionMarker(marker, beatTime);
//        }
//    }

//    private void Update()
//    {
//        // ќбновление положени€ точек с течением времени
//        float currentTime = Time.time;
//        for (int i = 0; i < contentRectTransform.childCount; i++)
//        {
//            Transform marker = contentRectTransform.GetChild(i);
//            float beatTime = beatTimes[i];

//            if (beatTime <= currentTime)
//            {
//                float progress = (currentTime - beatTime) / trackDuration;
//                float scrollPosition = progress * contentRectTransform.rect.height;
//                marker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -scrollPosition);
//            }
//        }
//    }

//    private void PositionMarker(GameObject marker, float beatTime)
//    {
//        float progress = beatTime / trackDuration;
//        float scrollPosition = progress * contentRectTransform.rect.height;
//        marker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -scrollPosition);
//    }
//}
