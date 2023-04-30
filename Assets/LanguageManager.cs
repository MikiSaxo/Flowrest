using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language
{
    Francais,
    English
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public event Action ChangeLanguageEvent;
    public Language Tongue { get; set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        Tongue = Language.Francais;
    }

    public void ChooseLanguage(Language newLanguage)
    {
        Tongue = newLanguage;
        ChangeLanguageEvent?.Invoke();
    }

    public void ChangeToFrenchOrEnglish()
    {
        if (Tongue == Language.Francais)
        {
            ChangeToEnglish();
        }
        else
            ChangeToFrench();
        
        ChangeLanguageEvent?.Invoke();
    }

    private void ChangeToFrench()
    {
        Tongue = Language.Francais;
    }

    private void ChangeToEnglish()
    {
        Tongue = Language.English;
    }
}