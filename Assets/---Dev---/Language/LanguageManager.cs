using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language
{
    Francais = 0,
    English = 1
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
    
    [Header("Tile Bored Text")]
    [SerializeField] private string _boredTextFrench;
    [SerializeField] private string _boredTextEnglish;
    
    [Header("Victory - Defeat")] 
    [SerializeField] private string _victoryTextFrench;
    [SerializeField] private string _victoryTextEnglish;
    [Space(5)]
    [SerializeField] private string _gameOverTextFrench;
    [SerializeField] private string _gameOverTextEnglish;
    
    [Header("Menu Victory - Defeat")] 
    [SerializeField] private string _nextTextFrench;
    [SerializeField] private string _nextTextEnglish;
    
    [Space(10f)]
    [Header("--- Pause Menu ---")]
    [Space(10f)]
    
    [Header("Resume Button")] 
    [SerializeField] private string _resumeTextFrench;
    [SerializeField] private string _resumeTextEnglish;
    
    [Header("Restart Button")] 
    [SerializeField] private string _restartTextFrench;
    [SerializeField] private string _restartTextEnglish;
    
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
    
    [Space(10f)]
    [Header("--- Menu Main Screen ---")]
    [Space(10f)]
    
    [Header("Play Button")]
    [SerializeField] private string _playButtonTextFrench;
    [SerializeField] private string _playButtonTextEnglish;

    [Space(10f)]
    [Header("--- Credits Screen ---")]
    [Space(10f)]
    
    [Header("Thanks for Playing")]
    [SerializeField] private string _thxForPlayinTextFrench;
    [SerializeField] private string _thxForPlayinTextEnglish;
    
    [Header("With Help of")]
    [SerializeField] private string _withHelpTextFrench;
    [SerializeField] private string _withHelpTextEnglish;
    
    [Header("Special Thanks")]
    [SerializeField] private string _specialThanksTextFrench;
    [SerializeField] private string _specialThanksTextEnglish;
    
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
        // ChangeToEnglish();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Tongue"))
        {
            if(PlayerPrefs.GetInt("Tongue") == (int)Language.English)
                StartCoroutine(TempWaitToChangeLanguage(Language.English));
            else
                StartCoroutine(TempWaitToChangeLanguage(Language.Francais));
        }
        else
            StartCoroutine(TempWaitToChangeLanguage(Language.Francais));
    }

    IEnumerator TempWaitToChangeLanguage(Language tongue)
    {
        yield return new WaitForSeconds(.1f);
        if(tongue == Language.Francais)
            ChangeToFrench();
        else
            ChangeToEnglish();
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
        print("Change To French");

        Tongue = Language.Francais;
        PlayerPrefs.SetInt("Tongue", (int)Language.Francais);
        ChangeLanguageEvent?.Invoke();
    }
    public void ChangeToEnglish()
    {
        print("Change To English");

        Tongue = Language.English;
        PlayerPrefs.SetInt("Tongue", (int)Language.English);
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
    public string GetBoredText()
    {
        if (Tongue == Language.Francais)
            return _boredTextFrench;

        return _boredTextEnglish;
    }
    public string GetResumeText()
    {
        if (Tongue == Language.Francais)
            return _resumeTextFrench;

        return _resumeTextEnglish;
    }
    public string GetRestartText()
    {
        if (Tongue == Language.Francais)
            return _restartTextFrench;

        return _restartTextEnglish;
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
    public string GetPlayButtonText()
    {
        if (Tongue == Language.Francais)
            return _playButtonTextFrench;

        return _playButtonTextEnglish;
    }
    public string GetThxForPlayingText()
    {
        if (Tongue == Language.Francais)
            return _thxForPlayinTextFrench;

        return _thxForPlayinTextEnglish;
    }
    public string GetWithHelpText()
    {
        if (Tongue == Language.Francais)
            return _withHelpTextFrench;

        return _withHelpTextEnglish;
    }
    public string GetSpecialThanksText()
    {
        if (Tongue == Language.Francais)
            return _specialThanksTextFrench;

        return _specialThanksTextEnglish;
    }
    public string GetNextText()
    {
        if (Tongue == Language.Francais)
            return _nextTextFrench;

        return _nextTextEnglish;
    }
}