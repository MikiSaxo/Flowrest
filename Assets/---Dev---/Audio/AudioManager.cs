using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;


    [Header("----- Audio Source -----")] [SerializeField]
    private AudioSource _musicSource;

    [SerializeField] private AudioSource _sFXSource;

    [Header("----- Audio Options -----")] [SerializeField]
    private AudioMixer _audioMixer;

    [Header("----- Other Musics -----")] public Sounds[] Musics;
    [Header("----- Main Musics -----")] public Sounds[] MainMusics;
    [Header("----- SFX -----")] public Sounds[] SFX;

    private float _waveCooldown;
    private float _waveTime;
    private string _currentMusic;
    private int _currentClipIndex;
    private List<int> _randomMusics;
    private bool _mainMusic;


    private void Awake()
    {
        if (Instance == null)
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
            music.Source = gameObject.AddComponent<AudioSource>();
            music.Source.clip = music.Clip;
            music.Source.volume = music.Volume;
            music.Source.pitch = music.Pitch;
            music.Source.loop = music.Loop;
            music.Source.outputAudioMixerGroup = _musicSource.outputAudioMixerGroup;
        }

        foreach (Sounds music in MainMusics)
        {
            music.Source = gameObject.AddComponent<AudioSource>();
            music.Source.clip = music.Clip;
            music.Source.volume = music.Volume;
            music.Source.pitch = music.Pitch;
            music.Source.loop = music.Loop;
            music.Source.outputAudioMixerGroup = _musicSource.outputAudioMixerGroup;
        }

        foreach (Sounds sfx in SFX)
        {
            sfx.Source = gameObject.AddComponent<AudioSource>();
            sfx.Source.clip = sfx.Clip;
            sfx.Source.volume = sfx.Volume;
            sfx.Source.pitch = sfx.Pitch;
            sfx.Source.loop = sfx.Loop;
            sfx.Source.outputAudioMixerGroup = _sFXSource.outputAudioMixerGroup;
        }
    }

    private void Start()
    {
        LaunchPlayLoop();

        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume(.5f);
            SetSFXVolume(.5f);
        }
    }

    private void LaunchPlayLoop()
    {
        ChooseRandomMusic();
        StartCoroutine(PlayMusicLoop());
        PlayMusicLong("Wave1");
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            _musicSource.clip = MainMusics[_randomMusics[_currentClipIndex]].Clip;
            _musicSource.Play();

            yield return new WaitForSeconds(_musicSource.clip.length);

            _currentClipIndex++;
            if (_currentClipIndex >= _randomMusics.Count)
            {
                _currentClipIndex = 0;
            }
        }
    }

    private void ChooseRandomMusic()
    {
        _randomMusics = GenerateRandomIndices(MainMusics.Length);
    }

    private List<int> GenerateRandomIndices(int count)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < count; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < count - 1; i++)
        {
            int randomIndex = Random.Range(i, count);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        return indices;
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

        s.Source.PlayOneShot(s.Clip);
    }

    public void PlayMusicLong(string name)
    {
        print($"Launch {name} music");
        Sounds s = Array.Find(Musics, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found");
            return;
        }

        s.Source.Play();
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
    }

    public void PlaySFXLong(string name)
    {
        // print($"Launch {name} sfx");

        Sounds s = Array.Find(SFX, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found");
            return;
        }

        s.Source.Play();
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
        if (AudioOptionManager.Instance != null)
            AudioOptionManager.Instance.LoadVolume();
    }
}