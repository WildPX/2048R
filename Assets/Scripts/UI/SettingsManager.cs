using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public AudioMixer audioMixer;
    public Toggle toggleSfxClickNote;

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
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    private void LoadMusicVolume()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    private void LoadSFXVolume()
    {
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void LoadSFXClickNote()
    {
        if (PlayerPrefs.GetString("SFXClickNote") == "On")
        {
            toggleSfxClickNote.isOn = true;
        }
        else
        {
            toggleSfxClickNote.isOn = false;
        }
    }

    private void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
    }

    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
    }

    private void SaveSFXVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
    }

    private void SaveSFXClickNote()
    {
        if (toggleSfxClickNote.isOn)
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
        audioMixer.SetFloat("MasterVolume", masterVolumeSlider.value);
        SaveMasterVolume();
    }

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicVolumeSlider.value);
        SaveMusicVolume();
    }

    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", sfxVolumeSlider.value);
        SaveSFXVolume();
    }

    public void ChangeSFXClickNote()
    {
        SaveSFXClickNote();
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
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
