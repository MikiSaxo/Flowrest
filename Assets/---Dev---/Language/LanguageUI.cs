using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageUI : MonoBehaviour
{
    [Header("Order - Memo")] 
    [SerializeField] private TMP_Text _orderTextButton;
    [SerializeField] private TMP_Text _memoTextButton;

    [Header("Victory - Defeat")] 
    [SerializeField] private TMP_Text _victoryText;
    [SerializeField] private TMP_Text _gameOverText;
    
    [Header("Pause Menu")]
    [SerializeField] private TMP_Text _resumeTextButton;
    [SerializeField] private TMP_Text _quitTextButton;

    [Header("Option Menu")]
    [SerializeField] private TMP_Text _musicTextButton;
    [SerializeField] private TMP_Text _sfxTextButton;
    [SerializeField] private TMP_Text _controlsTextButton;
    [SerializeField] private TMP_Text _languageTextButton;
    [SerializeField] private TMP_Text _creditsTextButton;
    [SerializeField] private TMP_Text _backButtonTextButton;
    


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
        _orderTextButton.text = _languageManager.GetOrderText();
        _memoTextButton.text = _languageManager.GetMemoText();
        _languageTextButton.text = _languageManager.GetLanguageText();
        _resumeTextButton.text = _languageManager.GetResumeText();
        RecyclingManager.Instance.UpdateDisplayRecyclingNbLeft();
        // _recycleTextButton.text = _languageManager.GetRecycleText();
        _victoryText.text = _languageManager.GetVictoryText();
        _gameOverText.text = _languageManager.GetGameOverText();
        _quitTextButton.text = _languageManager.GetQuitButtonText();
        _musicTextButton.text = _languageManager.GetMusicText();
        _sfxTextButton.text = _languageManager.GetSfxText();
        _controlsTextButton.text = _languageManager.GetControlsText();
        _creditsTextButton.text = _languageManager.GetCreditsText();
        _backButtonTextButton.text = _languageManager.GetBackButtonText();
    }

    private void OnDisable()
    {
        _languageManager.ChangeLanguageEvent -= ChangeLanguage;
    }
}