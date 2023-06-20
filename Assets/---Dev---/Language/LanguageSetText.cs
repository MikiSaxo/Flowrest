using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LanguageSetText : MonoBehaviour
{
    [Header("Order - Memo")] 
    [SerializeField] private TMP_Text _orderTextButton;
    [SerializeField] private TMP_Text _memoTextButton;

    [Header("Victory - Defeat")] 
    [SerializeField] private TMP_Text _victoryText;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _nextTextButton;
    
    [Header("Pop Up")]
    [SerializeField] private TMP_Text _nextTextPopUpButton;
    
    [Header("Pause Menu")]
    [SerializeField] private TMP_Text _resumeTextButton;
    [SerializeField] private TMP_Text _restartTextButton;
    [SerializeField] private TMP_Text _restartVictoryTextButton;
    [SerializeField] private TMP_Text _quitTextButton;

    [Header("Option Menu")]
    [SerializeField] private TMP_Text _musicTextButton;
    [SerializeField] private TMP_Text _sfxTextButton;
    [SerializeField] private TMP_Text _controlsTextButton;
    [SerializeField] private TMP_Text _languageTextButton;
    [SerializeField] private TMP_Text _creditsTextButton;
    [SerializeField] private TMP_Text _backTextButton;
    
    [Header("Main Menu")]
    [SerializeField] private TMP_Text _playButtonTextButton;
    [SerializeField] private TMP_Text _continueButtonTextButton;
    [SerializeField] private TMP_Text _newGameButtonTextButton;
    [SerializeField] private TMP_Text _quitButton;
    [SerializeField] private TMP_Text _popUpNewGameText;
    [SerializeField] private TMP_Text _backTextButtonMain;
    [SerializeField] private TMP_Text _yesButton;
    [SerializeField] private TMP_Text _noButton;
    
    [Header("Credits Screen")]
    [SerializeField] private TMP_Text _thxForPlayinText;
    [SerializeField] private TMP_Text _withHelpText;
    [SerializeField] private TMP_Text _specialThanksText;


    // [Header("Recycle")]
    // [SerializeField] private TMP_Text _recycleTextButton;
    
    private LanguageManager _languageManager;

    private void Start()
    {
        _languageManager = LanguageManager.Instance;

        _languageManager.ChangeLanguageEvent += ChangeLanguage;
    }

    private void ChangeLanguage()
    {
        _musicTextButton.text = _languageManager.GetMusicText();
        _sfxTextButton.text = _languageManager.GetSfxText();
        _controlsTextButton.text = _languageManager.GetControlsText();
        _languageTextButton.text = _languageManager.GetLanguageText();
        _creditsTextButton.text = _languageManager.GetCreditsText();
        _backTextButton.text = _languageManager.GetBackButtonText();
        

        var sceneIndex = SceneManager.GetActiveScene();

        if (sceneIndex.buildIndex == 0)
        {
            _playButtonTextButton.text = _languageManager.GetPlayButtonText();
            _continueButtonTextButton.text = _languageManager.GetContinueButtonText();
            _newGameButtonTextButton.text = _languageManager.GetNewGameButtonText();
            _quitButton.text = _languageManager.GetQuitButtonText();   
            _popUpNewGameText.text = _languageManager.GetPopUpNewGameButtonText();   
            _yesButton.text = _languageManager.GetYesText();   
            _noButton.text = _languageManager.GetNoText();   
            _backTextButtonMain.text = _languageManager.GetBackButtonText();
        }
        else
        {
            _orderTextButton.text = _languageManager.GetOrderText();
            _memoTextButton.text = _languageManager.GetMemoText();
            _victoryText.text = _languageManager.GetVictoryText();
            _gameOverText.text = _languageManager.GetGameOverText();
            _resumeTextButton.text = _languageManager.GetResumeText();
            _restartTextButton.text = _languageManager.GetRestartText();
            _restartVictoryTextButton.text = _languageManager.GetRestartText();
            _quitTextButton.text = _languageManager.GetQuitButtonText();
            RecyclingManager.Instance.UpdateDisplayRecyclingNbLeft();
            _thxForPlayinText.text = _languageManager.GetThxForPlayingText();
            _withHelpText.text = _languageManager.GetWithHelpText();
            _specialThanksText.text = _languageManager.GetSpecialThanksText();
            _nextTextButton.text = _languageManager.GetNextText();
            _nextTextPopUpButton.text = _languageManager.GetNextText();
        }
    }

    private void OnDisable()
    {
        _languageManager.ChangeLanguageEvent -= ChangeLanguage;
    }
}