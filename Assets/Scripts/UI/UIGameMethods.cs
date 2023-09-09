using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameMethods : MonoBehaviour
{
    public AudioSource sfxClick;

    private RhythmController rhythmController;
    // Seconds to wait before loading scene
    private const float delayBeforeLoad = 1f;

    private void Start()
    {
        rhythmController = GetComponent<RhythmController>();
    }

    public void OnMenuButton()
    {
        PlaySound();
        var mapData = FindObjectOfType<MapData>();
        Destroy(mapData.gameObject);
        StartCoroutine(SoftLoad("Menu"));
    }

    public void OnRetryButton()
    {
        PlaySound();
        StartCoroutine(SoftLoad("Game"));
    }

    public void OnContinueButton()
    {
        PlaySound();
        rhythmController.pauseScreen.interactable = false;
        rhythmController.SetUnpause();
    }

    private IEnumerator SoftLoad(string scene)
    {
        float elapsedTime = 0f;
        float startAlpha = rhythmController.fadeOutBackground.alpha;
        rhythmController.fadeOutBackground.blocksRaycasts = true;
        rhythmController.pauseScreen.interactable = false;

        while (elapsedTime < delayBeforeLoad)
        {
            float t = elapsedTime / delayBeforeLoad;
            rhythmController.fadeOutBackground.alpha = Mathf.Lerp(startAlpha, 1f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }

    private void PlaySound()
    {
        sfxClick.Play();
    }
}