using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Toggle _toggleSfxClickNote;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", -15);
            LoadMasterVolume();
        }
        else
        {
            LoadMasterVolume();
        }

        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", -15);
            LoadMusicVolume();
        }
        else
        {
            LoadMusicVolume();
        }

        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", -15);
            LoadSFXVolume();
        }
        else
        {
            LoadSFXVolume();
        }
        if (!PlayerPrefs.HasKey("SFXClickNote"))
        {
            PlayerPrefs.SetString("SFXClickNote", "On");
            LoadSFXClickNote();
        }
        else
        {
            LoadSFXClickNote();
        }
    }

    private void LoadMasterVolume()
    {
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    private void LoadMusicVolume()
    {
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    private void LoadSFXVolume()
    {
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void LoadSFXClickNote()
    {
        if (PlayerPrefs.GetString("SFXClickNote") == "On")
        {
            _toggleSfxClickNote.isOn = true;
        }
        else
        {
            _toggleSfxClickNote.isOn = false;
        }
    }

    private void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolumeSlider.value);
    }

    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicVolumeSlider.value);
    }

    private void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolumeSlider.value);
    }

    private void SaveSFXClickNote()
    {
        if (_toggleSfxClickNote.isOn)
        {
            PlayerPrefs.SetString("SFXClickNote", "On");
        }
        else
        {
            PlayerPrefs.SetString("SFXClickNote", "Off");
        }
    }

    public void ChangeMasterVolume()
    {
        _audioMixer.SetFloat("MasterVolume", _masterVolumeSlider.value);
        SaveMasterVolume();
    }

    public void ChangeMusicVolume()
    {
        _audioMixer.SetFloat("MusicVolume", _musicVolumeSlider.value);
        SaveMusicVolume();
    }

    public void ChangeSFXVolume()
    {
        _audioMixer.SetFloat("SFXVolume", _sfxVolumeSlider.value);
        SaveSFXVolume();
    }

    public void ChangeSFXClickNote()
    {
        SaveSFXClickNote();
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
//[System.Serializable]
//public class SettingsData
//{
//    public AudioSettings audio;
//    public GraphicsSettings graphics;
//    public ControlsSettings controls;
//}

//[System.Serializable]
//public class AudioSettings
//{
//    public string outputdevice;
//    public float master;
//    public float music;
//    public float sfx;
//}

//[System.Serializable]
//public class GraphicsSettings
//{
//    public int width;
//    public int height;
//    public bool fullscreen;
//    public bool borderless;
//}

//[System.Serializable]
//public class ControlsSettings
//{
//    public bool keyboard;
//    public bool gamepad;
//}
