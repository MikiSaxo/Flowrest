using UnityEngine;
using UnityEngine.Audio;
using System;
[Serializable] public class Sounds
{
    public string Name;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume;
    [Range(.1f, 3f)] public float Pitch;
    public bool Loop;

    [HideInInspector] public AudioSource Source;
}