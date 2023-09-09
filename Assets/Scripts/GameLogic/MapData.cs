using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour 
{
    public string jsonContentPath;
    public MapDataConfig DataConfig { get; private set; }

    private void Start()
    {
        LoadMapData();
    }

    private void LoadMapData()
    {
        TextAsset jsonContent = Resources.Load<TextAsset>(jsonContentPath);

        if (jsonContent == null)
        {
            Debug.LogError("Failed to load JSON file: " + jsonContentPath);
            return;
        }

        DataConfig = JsonUtility.FromJson<MapDataConfig>(jsonContent.text);
        //mapDataConfig.Print();
    }
}

[System.Serializable]
public class MapDataConfig
{
    public string name;
    public string artistName;
    public MusicData musicData;
    //public BestScores bestScores;
    public string backgroundImagePath;
    public string audioPath;

    public void Print()
    {
        Debug.Log("Music Data.");
        Debug.Log(name);
        Debug.Log(artistName);
        Debug.Log(musicData.time);
        Debug.Log(musicData.tempo);
        Debug.Log(musicData.sampleRate);
        Debug.Log(musicData.hopLength);
        Debug.Log(musicData.beatFrames.Count);

        Debug.Log(backgroundImagePath);
        Debug.Log(audioPath);
    }
}

[System.Serializable]
public class MusicData
{
    public int time;
    public float tempo;
    public int sampleRate;
    public int hopLength;
    public List<float> beatFrames;
}

[System.Serializable]
public class BestScores
{
    public int bestScore;
    public float bestAccuracy;
    public int bestStreak;
}