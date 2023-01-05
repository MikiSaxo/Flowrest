using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundFeedbackTemperature : MonoBehaviour
{
    [SerializeField] private GameObject _fbTemperature;
    [SerializeField] private Sprite[] _hotNCold;
    [SerializeField] private float _heightMax;
    [SerializeField] private float _timeToHeightMax;

    private float _startYPos;
    private float _endYPos;

    private void Start()
    {
        _startYPos = transform.position.y;
        _endYPos = _startYPos + _heightMax;
    }

    public void LaunchFB(bool isHot)
    {
        _fbTemperature.GetComponent<SpriteRenderer>().sprite = isHot ? _hotNCold[0] : _hotNCold[1];

        _fbTemperature.transform.DOKill();
        _fbTemperature.GetComponent<SpriteRenderer>().DOKill();
        
        Reset();
        
        _fbTemperature.transform.DOMoveY(_endYPos, _timeToHeightMax);
        _fbTemperature.GetComponent<SpriteRenderer>().DOFade(0, _timeToHeightMax + 1).OnComplete(Reset);
    }

    private void Reset()
    {
        _fbTemperature.transform.DOMoveY(_startYPos, 0);
        _fbTemperature.GetComponent<SpriteRenderer>().DOFade(1, 0);
    }
}
