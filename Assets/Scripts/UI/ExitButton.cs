using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField] private CanvasGroup _exitImage;
    [SerializeField] private AudioSource _soundClip;

    private const float EXITFADEOUT = 3f;
    private const float ENDVOLUME = 0f;
    private float _startAlpha;
    private const float ENDALPHA = 1f;
    private float _startVolume;

    private void Awake()
    {
        _startAlpha = _exitImage.alpha;
        _startVolume = _soundClip.volume;
    }

    public void OnExitButton()
    {
        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        float elapsedTime = 0f;

        _exitImage.blocksRaycasts = true;

        while (elapsedTime < EXITFADEOUT)
        {
            float t = elapsedTime / EXITFADEOUT;
            _exitImage.alpha = Mathf.Lerp(_startAlpha, ENDALPHA, t);
            _soundClip.volume = Mathf.Lerp(_startVolume, ENDVOLUME, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _exitImage.alpha = ENDALPHA;
        _soundClip.volume = ENDVOLUME;

        Application.Quit();
    }
}
