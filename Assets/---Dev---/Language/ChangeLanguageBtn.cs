using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeLanguageBtn : MonoBehaviour
{
    private Language _currentLanguage;
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

    public void ChangeToFrench()
    {
        if (MapManager.Instance.IsLoading || LanguageManager.Instance.Tongue == Language.Francais) return;

        _currentLanguage = Language.Francais;
        StartCoroutine(WaitToChangeLanguage());
    }

    public void ChangeToEnglish()
    {
        if (MapManager.Instance.IsLoading || LanguageManager.Instance.Tongue == Language.English) return;

        _currentLanguage = Language.English;
        StartCoroutine(WaitToChangeLanguage());
    }


    IEnumerator WaitToChangeLanguage()
    {
        MapManager.Instance.RestartLevel();
        
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());
        
        if(_currentLanguage == Language.Francais)
            LanguageManager.Instance.ChangeToFrench();
        else
            LanguageManager.Instance.ChangeToEnglish();
    }
}
