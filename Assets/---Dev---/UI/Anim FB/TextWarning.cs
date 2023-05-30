using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWarning : MonoBehaviour
{
    [SerializeField] private float _timeFadeOn;
    [SerializeField] private float _timeFadeOff;
    private TMP_Text _text;

    private void Awake()
    {
        // _text.DOFade(0, 0);
        _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        //UpdateLanguageText();
        ToFadeOn();
    }

    public void Init(string text)
    {
        _text.text = text;
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
