using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public TextAsset jsonFile;
    public TextMeshProUGUI RText;
    public Image backgroundImage;
    public GameObject aboutMenu;
    public GameObject settingsMenu;
    public GameObject playMenu;
    public AudioSource sfxClick;
    public AudioSource sfxClickBack;

    private const int frameRate = 60;
    private GameObject currentMenu;
    private BackgroundData backgroundData;

    private float softnessChangeDuration;
    private bool frozenFlag;
    private float startSoftness;
    private float targetSoftness;

    private void Awake()
    {
        InitializeVariables();
        LoadData();
    }

    private void Start()
    {
        Application.targetFrameRate = frameRate;
        StartCoroutine(RTextSoftnessChange());
        CloseAllMenus();
    }

    private void InitializeVariables()
    {
        softnessChangeDuration = 0.5f;
        frozenFlag = false;
        startSoftness = 0f;
        targetSoftness = 0.5f;
    }

    private void LoadData()
    {
        backgroundData = JsonUtility.FromJson<BackgroundData>(jsonFile.text);
        BackgroundTheme randomTheme = backgroundData.backgroundTheme[Random.Range(0, backgroundData.backgroundTheme.Count)];
        string imagePath = randomTheme.imagePath;
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite != null)
        {
            backgroundImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Sprite is not found");
            Debug.Log(imagePath);
        }
    }

    private IEnumerator RTextSoftnessChange()
    {
        while (!frozenFlag)
        {
            float elapsedTime = 0f;
            while (elapsedTime < softnessChangeDuration)
            {
                float t = elapsedTime / softnessChangeDuration;
                float softness = Mathf.Lerp(startSoftness, targetSoftness, t);

                ModifyTextSoftness(softness);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            (startSoftness, targetSoftness) = (targetSoftness, startSoftness);
        }
    }

    private void ModifyTextSoftness(float softness)
    {
        RText.fontSharedMaterial.SetFloat("_OutlineSoftness", softness);
    }

    public void OnAboutButton()
    {
        if (currentMenu == aboutMenu)
        {
            PlaySound(sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(sfxClick);
            OpenMenu(aboutMenu);
        }
    }

    public void OnSettingsButton()
    {
        if (currentMenu == settingsMenu)
        {
            PlaySound(sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(sfxClick);
            OpenMenu(settingsMenu);
        }
    }

    public void OnPlayButton()
    {
        if (currentMenu == playMenu)
        {
            PlaySound(sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(sfxClick);
            OpenMenu(playMenu);
        }
    }

    private void OpenMenu(GameObject menu)
    {
        CloseMenu();

        currentMenu = menu;
        currentMenu.SetActive(true);
    }

    private void CloseMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
        }
    }

    private void CloseAllMenus()
    {
        aboutMenu.SetActive(false);
        settingsMenu.SetActive(false);
        playMenu.SetActive(false);
    }

    private void PlaySound(AudioSource source)
    {
        source.Play();
    }
}

[System.Serializable]
public class BackgroundTheme
{
    public string name;
    public string imagePath;
}

[System.Serializable]
public class BackgroundData
{
    public List<BackgroundTheme> backgroundTheme;
}