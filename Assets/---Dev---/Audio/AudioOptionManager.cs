using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionManager : MonoBehaviour
{
    public static AudioOptionManager Instance;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(_musicSlider.value);
    }

    public void SetSfxVolume()
    {
        AudioManager.Instance.SetSFXVolume(_sfxSlider.value);
    }

    public void LoadVolume()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        
        SetMusicVolume();
        SetSfxVolume();
    }
}