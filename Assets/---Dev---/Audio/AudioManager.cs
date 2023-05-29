using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    

    [Header("----- Audio Source -----")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sFXSource;
    [Header("----- Audio Options -----")]
    [SerializeField] private AudioMixer _audioMixer;
    [Header("----- Musics -----")]
    public Sounds[] Musics;
    [Header("----- SFX -----")]
    public Sounds[] SFX;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        foreach (Sounds music in Musics)
        {
            music.Source = _musicSource;
            music.Source.clip = music.Clip;
            music.Source.volume = music.Volume;
            music.Source.pitch = music.Pitch;
            music.Source.loop = music.Loop;
        }
        foreach (Sounds sfx in SFX)
        {
            sfx.Source = _sFXSource;
            sfx.Source.clip = sfx.Clip;
            sfx.Source.volume = sfx.Volume;
            sfx.Source.pitch = sfx.Pitch;
            sfx.Source.loop = sfx.Loop;
        }
    }

    private void Start()
    {
        //Scene currentScene = SceneManager.GetActiveScene();
        //if (currentScene.name == "MainMenu")
        //    PlaySound("MusicMain");
        //else
        //    PlaySound("MusicGame");


        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume(.5f);
            SetSFXVolume(.5f);
        }
        
        PlayMusic("MainMusic");
    }
    
    public void PlayMusic(string name)
    {
        // print($"Launch {name} music");
        Sounds s = Array.Find(Musics, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found");
            return;
        }
        // s.Source.Play();
        s.Source.PlayOneShot(s.Clip);
    }
    public void PlaySFX(string name)
    {
        // print($"Launch {name} sfx");
        Sounds s = Array.Find(SFX, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found");
            return;
        }
        s.Source.PlayOneShot(s.Clip);
        // s.Source.Play();
    }
    public void StopMusic(string name)
    {
        Sounds s = Array.Find(Musics, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found in StopSound");
            return;
        }
        s.Source.Stop();
    }
    public void StopSFX(string name)
    {
        Sounds s = Array.Find(SFX, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found in StopSound");
            return;
        }
        s.Source.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        if(AudioOptionManager.Instance != null)
            AudioOptionManager.Instance.LoadVolume();
    }
}