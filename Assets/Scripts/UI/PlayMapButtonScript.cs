using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayMapButtonScript : MonoBehaviour
{
    public GameObject mapDataPrefab;
    public string jsonPath;
    public AudioSource sfxClickPlay;
    public CanvasGroup fadeOutBackground;
    public AudioSource sound;

    private const float fadeOutTimer = 1f;

    public void OnPlayMapButton()
    {
        sfxClickPlay.Play();
        GameObject mapData = Instantiate(mapDataPrefab);
        mapData.GetComponent<MapData>().jsonContentPath = jsonPath;
        DontDestroyOnLoad(mapData);
        StartCoroutine(SoftLoad());
    }

    private IEnumerator SoftLoad()
    {
        float elapsedTime = 0f;
        float startAlpha = fadeOutBackground.alpha;
        float startVolume = sound.volume;
        fadeOutBackground.blocksRaycasts = true;
        
        while (elapsedTime < fadeOutTimer)
        {
            float t = elapsedTime / fadeOutTimer;
            fadeOutBackground.alpha = Mathf.Lerp(startAlpha, 1f, t);
            sound.volume = Mathf.Lerp(startVolume, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeOutBackground.alpha = 1f;
        sound.volume = 0f;

        SceneManager.LoadScene("Game");
    }
}
