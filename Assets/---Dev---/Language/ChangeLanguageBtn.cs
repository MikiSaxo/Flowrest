using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageBtn : MonoBehaviour
{
    [SerializeField] private Sprite[] _flags;
    [SerializeField] private Image _flagIcon;


    private void Start()
    {
        LanguageManager.Instance.ChangeLanguageEvent += FrenchOrEnglish;
    }

    public void ChangeLanguageButton()
    {
        // if (MapManager.Instance.IsLoading) return;
        //
        // LanguageManager.Instance.ChangeToFrenchOrEnglish();
        // MapManager.Instance.RestartLevel();
    }

    private void FrenchOrEnglish()
    {
        if (LanguageManager.Instance.Tongue == Language.Francais)
            ChangeToFrench();
        else
            ChangeToEnglish();
    }

    private void ChangeToFrench()
    {
        _flagIcon.sprite = _flags[0];
    }

    private void ChangeToEnglish()
    {
        _flagIcon.sprite = _flags[1];
    }

    private void OnDisable()
    {
        LanguageManager.Instance.ChangeLanguageEvent -= FrenchOrEnglish;
    }
}
