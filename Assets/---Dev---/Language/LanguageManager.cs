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
    
    [Header("Recycle Menu")]
    [SerializeField] private string _recycleTextFrench;
    [SerializeField] private string _recycleTextEnglish;
   
    [Header("No Energy Text")]
    [SerializeField] private string _noEnergyTextFrench;
    [SerializeField] private string _noEnergyTextEnglish;

    
    [Header("Victory - Defeat")] 
    [SerializeField] private string _victoryTextFrench;
    [SerializeField] private string _victoryTextEnglish;
    [Space(5)]
    [SerializeField] private string _gameOverTextFrench;
    [SerializeField] private string _gameOverTextEnglish;

    [Space(10f)]
    [Header("--- Pause Menu ---")]
    [Space(10f)]
    
    [Header("Resume Button")] 
    [SerializeField] private string _resumeTextFrench;
    [SerializeField] private string _resumeTextEnglish;
    [Header("Quit Button")] 
    [SerializeField] private string _quitButtonTextFrench;
    [SerializeField] private string _quitButtonTextEnglish;
    
    [Space(10f)]
    [Header("--- Options ---")]
    [Space(10f)]
    
    [Header("Music")]
    [SerializeField] private string _musicTextFrench;
    [SerializeField] private string _musicTextEnglish;
    
    [Header("SFX")]
    [SerializeField] private string _sfxTextFrench;
    [SerializeField] private string _sfxTextEnglish;
    
    [Header("Controls")]
    [SerializeField] private string _controlTextFrench;
    [SerializeField] private string _controlTextEnglish;
   
    [Header("Language")]
    [SerializeField] private string _languageTextFrench;
    [SerializeField] private string _languageTextEnglish;

    [Header("Credits")]
    [SerializeField] private string _creditTextFrench;
    [SerializeField] private string _creditTextEnglish;
    
    [Header("Back Button")]
    [SerializeField] private string _backButtonTextFrench;
    [SerializeField] private string _backButtonTextEnglish;
    

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
        // ChangeToEnglish();
    }

    private void Start()
    {
        // ChangeToEnglish();
        StartCoroutine(TempWaitToChangeLanguage());
    }

    IEnumerator TempWaitToChangeLanguage()
    {
        yield return new WaitForSeconds(.3f);
        ChangeToFrench();
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

    public void ChangeToFrench()
    {
        Tongue = Language.Francais;
        ChangeLanguageEvent?.Invoke();
    }

    public void ChangeToEnglish()
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
    
    public string GetNoEnergyText()
    {
        if (Tongue == Language.Francais)
            return _noEnergyTextFrench;

        return _noEnergyTextEnglish;
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
    public string GetQuitButtonText()
    {
        if (Tongue == Language.Francais)
            return _quitButtonTextFrench;

        return _quitButtonTextEnglish;
    }
    public string GetMusicText()
    {
        if (Tongue == Language.Francais)
            return _musicTextFrench;

        return _musicTextEnglish;
    }
    public string GetSfxText()
    {
        if (Tongue == Language.Francais)
            return _sfxTextFrench;

        return _sfxTextEnglish;
    }
    public string GetControlsText()
    {
        if (Tongue == Language.Francais)
            return _controlTextFrench;

        return _controlTextEnglish;
    }
    public string GetCreditsText()
    {
        if (Tongue == Language.Francais)
            return _creditTextFrench;

        return _creditTextEnglish;
    }
    public string GetBackButtonText()
    {
        if (Tongue == Language.Francais)
            return _backButtonTextFrench;

        return _backButtonTextEnglish;
    }
}