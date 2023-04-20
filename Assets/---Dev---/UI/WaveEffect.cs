using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaveEffect : MonoBehaviour
{
    [SerializeField] private float _scaleMax;
    [SerializeField] private float _timeGrowOn;
    [SerializeField] private float _timeWaitReset;
    
    private Image _img;
    private bool _canMove;
    void Start()
    {
        _img = GetComponent<Image>();
        _canMove = true;
    }

    public void StartGrowOnAlways()
    {
        GrowOn();
    }
    public void StartGrowOneTime()
    {
        GrowOneTime();
    }

    public void StopGrownOn()
    {
        _canMove = false;
        _img.DOKill();
        _img.DOFade(0, 0);
        _canMove = true;
    }
    
    private void GrowOn()
    {
        if (!_canMove)
        {
            _canMove = true;
            return;
        }
        
        _img.DOFade(1, 0);
        _img.DOFade(0, _timeGrowOn);
        gameObject.transform.DOScale(_scaleMax, _timeGrowOn).OnComplete(Reset);
    }
    private void GrowOneTime()
    {
        _img.DOFade(1, 0);
        gameObject.transform.DOScale(1, 0);
        _img.DOFade(0, _timeGrowOn);
        gameObject.transform.DOScale(_scaleMax, _timeGrowOn);
    }

    private void Reset()
    {
        if(_canMove)
            gameObject.transform.DOScale(1, _timeWaitReset).OnComplete(GrowOn);
        else
        {
            gameObject.transform.DOScale(1, _timeWaitReset);
            _canMove = true;
        }
    }
}
