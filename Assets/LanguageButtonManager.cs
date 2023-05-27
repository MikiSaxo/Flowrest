using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButtonManager : MonoBehaviour
{
    [SerializeField] private Button[] _languageButtons;
    
    public void ActivateCurrentLanguageButton()
    {
        if(LanguageManager.Instance.Tongue == Language.Francais)
            _languageButtons[0].Select();
        else
            _languageButtons[1].Select();
    }
}
