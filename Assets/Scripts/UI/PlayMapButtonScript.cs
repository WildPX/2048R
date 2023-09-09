using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayMapButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject _mapDataPrefab;
    [SerializeField] private string _jsonPath;
    [SerializeField] private AudioSource _sfxClickPlay;
    [SerializeField] private CanvasGroup _fadeOutBackground;
    [SerializeField] private AudioSource _sound;

    private const float FADEOUT_TIMER = 1f;

    public void OnPlayMapButton()
    {
        _sfxClickPlay.Play();
        GameObject mapData = Instantiate(_mapDataPrefab);
        mapData.GetComponent<MapData>().SetJsonContentPath(_jsonPath);
        DontDestroyOnLoad(mapData);
        StartCoroutine(SoftLoad());
    }

    private IEnumerator SoftLoad()
    {
        float elapsedTime = 0f;
        float startAlpha = _fadeOutBackground.alpha;
        float startVolume = _sound.volume;
        _fadeOutBackground.blocksRaycasts = true;
        
        while (elapsedTime < FADEOUT_TIMER)
        {
            float t = elapsedTime / FADEOUT_TIMER;
            _fadeOutBackground.alpha = Mathf.Lerp(startAlpha, 1f, t);
            _sound.volume = Mathf.Lerp(startVolume, 0f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _fadeOutBackground.alpha = 1f;
        _sound.volume = 0f;

        SceneManager.LoadScene("Game");
    }
}
