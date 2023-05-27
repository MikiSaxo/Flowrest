using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoEnergyTextEffect : MonoBehaviour
{
    [SerializeField] private float _timeFadeOn;
    [SerializeField] private float _timeFadeOff;
    private TMP_Text _text;

    private void Awake()
    {
        _text.DOFade(0, 0);
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        UpdateLanguageText();
        ToFadeOn();
    }

    private void UpdateLanguageText()
    {
        _text.text = LanguageManager.Instance.GetNoEnergyText();
    }

    private void ToFadeOn()
    {
        _text.DOFade(1, _timeFadeOn).OnComplete(ToFadeOff);
    }
    
    private void ToFadeOff()
    {
        _text.DOFade(0, _timeFadeOff).OnComplete(DestroyText).SetEase(Ease.InSine);
    }

    private void DestroyText()
    {
        _text.DOKill();
        Destroy(gameObject);
    }

   
}
