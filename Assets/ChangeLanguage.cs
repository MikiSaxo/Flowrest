using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguage : MonoBehaviour
{
    [SerializeField] private Sprite[] _flags;
    [SerializeField] private Image _flagIcon;


    private void Start()
    {
        LanguageManager.Instance.ChangeLanguageEvent += FrenchOrEnglish;
    }

    public void ChangeLanguageButton()
    {
        LanguageManager.Instance.ChangeToFrenchOrEnglish();
    }

    public void FrenchOrEnglish()
    {
        if (LanguageManager.Instance.Tongue == Language.Francais)
            ChangeToFrench();
        else
            ChangeToEnglish();
    }
    
    public void ChangeToFrench()
    {
        _flagIcon.sprite = _flags[0];
    }

    public void ChangeToEnglish()
    {
        _flagIcon.sprite = _flags[1];
    }

    private void OnDisable()
    {
        LanguageManager.Instance.ChangeLanguageEvent -= FrenchOrEnglish;
    }
}
