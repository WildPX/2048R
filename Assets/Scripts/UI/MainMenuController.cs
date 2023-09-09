using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TextAsset _jsonFile;
    [SerializeField] private TextMeshProUGUI _rText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private GameObject _aboutMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _playMenu;
    [SerializeField] private AudioSource _sfxClick;
    [SerializeField] private AudioSource _sfxClickBack;

    private const int FRAMERATE = 60;
    private GameObject _currentMenu;
    private _backgroundData _backgroundData;

    private float _softnessChangeDuration;
    private bool _frozenFlag;
    private float _startSoftness;
    private float _targetSoftness;

    private void Awake()
    {
        InitializeVariables();
        LoadData();
    }

    private void Start()
    {
        Application.targetFrameRate = FRAMERATE;
        StartCoroutine(_rTextSoftnessChange());
        CloseAllMenus();
    }

    private void InitializeVariables()
    {
        _softnessChangeDuration = 0.5f;
        _frozenFlag = false;
        _startSoftness = 0f;
        _targetSoftness = 0.5f;
    }

    private void LoadData()
    {
        _backgroundData = JsonUtility.FromJson<_backgroundData>(_jsonFile.text);
        BackgroundTheme randomTheme = _backgroundData.backgroundTheme[Random.Range(0, _backgroundData.backgroundTheme.Count)];
        string imagePath = randomTheme.imagePath;
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite != null)
        {
            _backgroundImage.sprite = sprite;
        }
        else
        {
            Debug.LogError("Sprite is not found");
            Debug.Log(imagePath);
        }
    }

    private IEnumerator _rTextSoftnessChange()
    {
        while (!_frozenFlag)
        {
            float elapsedTime = 0f;
            while (elapsedTime < _softnessChangeDuration)
            {
                float t = elapsedTime / _softnessChangeDuration;
                float softness = Mathf.Lerp(_startSoftness, _targetSoftness, t);

                ModifyTextSoftness(softness);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            (_startSoftness, _targetSoftness) = (_targetSoftness, _startSoftness);
        }
    }

    private void ModifyTextSoftness(float softness)
    {
        _rText.fontSharedMaterial.SetFloat("_OutlineSoftness", softness);
    }

    public void OnAboutButton()
    {
        if (_currentMenu == _aboutMenu)
        {
            PlaySound(_sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(_sfxClick);
            OpenMenu(_aboutMenu);
        }
    }

    public void OnSettingsButton()
    {
        if (_currentMenu == _settingsMenu)
        {
            PlaySound(_sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(_sfxClick);
            OpenMenu(_settingsMenu);
        }
    }

    public void OnPlayButton()
    {
        if (_currentMenu == _playMenu)
        {
            PlaySound(_sfxClickBack);
            CloseMenu();
        }
        else
        {
            PlaySound(_sfxClick);
            OpenMenu(_playMenu);
        }
    }

    private void OpenMenu(GameObject menu)
    {
        CloseMenu();

        _currentMenu = menu;
        _currentMenu.SetActive(true);
    }

    private void CloseMenu()
    {
        if (_currentMenu != null)
        {
            _currentMenu.SetActive(false);
            _currentMenu = null;
        }
    }

    private void CloseAllMenus()
    {
        _aboutMenu.SetActive(false);
        _settingsMenu.SetActive(false);
        _playMenu.SetActive(false);
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
public class _backgroundData
{
    public List<BackgroundTheme> backgroundTheme;
}