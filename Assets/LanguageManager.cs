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

    [Header("Order - Memo")] 
    [SerializeField] private string _orderTextFrench;
    [SerializeField] private string _orderTextEnglish;
    [Space(5)]
    [SerializeField] private string _memoTextFrench;
    [SerializeField] private string _memoTextEnglish;

    [Header("Pause Menu")] 
    [SerializeField] private string _languageTextFrench;
    [SerializeField] private string _languageTextEnglish;
    [Space(5)]
    [SerializeField] private string _resumeTextFrench;
    [SerializeField] private string _resumeTextEnglish;
    
    [Header("Victory - Defeat")] 
    [SerializeField] private string _victoryTextFrench;
    [SerializeField] private string _victoryTextEnglish;
    [Space(5)]
    [SerializeField] private string _gameOverTextFrench;
    [SerializeField] private string _gameOverTextEnglish;

    [Header("Recycle Menu")]
    [SerializeField] private string _recycleTextFrench;
    [SerializeField] private string _recycleTextEnglish;
    

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
        ChangeToEnglish();
    }

    private void Start()
    {
        ChangeToEnglish();
        // ChangeToFrench();
    }

    public void ChooseLanguage(Language newLanguage)
    {
        Tongue = newLanguage;
        ChangeLanguageEvent?.Invoke();
    }

    public void ChangeToFrenchOrEnglish()
    {
        if (Tongue == Language.Francais)
            ChangeToEnglish();
        else
            ChangeToFrench();
    }

    private void ChangeToFrench()
    {
        Tongue = Language.Francais;
        ChangeLanguageEvent?.Invoke();
    }

    private void ChangeToEnglish()
    {
        Tongue = Language.English;
        ChangeLanguageEvent?.Invoke();
    }

    public string GetOrderText()
    {
        if (Tongue == Language.Francais)
            return _orderTextFrench;

        return _orderTextEnglish;
    }

    public string GetMemoText()
    {
        if (Tongue == Language.Francais)
            return _memoTextFrench;

        return _memoTextEnglish;
    }

    public string GetLanguageText()
    {
        if (Tongue == Language.Francais)
            return _languageTextFrench;

        return _languageTextEnglish;
    }

    public string GetResumeText()
    {
        if (Tongue == Language.Francais)
            return _resumeTextFrench;

        return _resumeTextEnglish;
    }
    
    public string GetRecycleText()
    {
        if (Tongue == Language.Francais)
            return _recycleTextFrench;

        return _recycleTextEnglish;
    }
    
    public string GetVictoryText()
    {
        if (Tongue == Language.Francais)
            return _victoryTextFrench;

        return _victoryTextEnglish;
    }
    
    public string GetGameOverText()
    {
        if (Tongue == Language.Francais)
            return _gameOverTextFrench;

        return _gameOverTextEnglish;
    }
}