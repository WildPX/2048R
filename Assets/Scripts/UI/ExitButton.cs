using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Image exitImage;
    public AudioSource soundClip;

    private const float exitFadeout = 3f;
    private Color startColor;
    private Color endColor;
    private float startVolume;
    private const float endVolume = 0f;

    private void Awake()
    {
        startColor = exitImage.color;
        endColor = Color.black;
        startVolume = soundClip.volume;
    }

    public void OnExitButton()
    {
        StartCoroutine(ExitRoutine());
    }

    private IEnumerator ExitRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < exitFadeout)
        {
            float t = elapsedTime / exitFadeout;
            exitImage.color = Color.Lerp(startColor, endColor, t);
            soundClip.volume = Mathf.Lerp(startVolume, endVolume, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        exitImage.color = endColor;
        soundClip.volume = endVolume;

        Application.Quit();
    }
}
