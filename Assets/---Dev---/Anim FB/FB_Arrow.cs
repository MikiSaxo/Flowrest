using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FB_Arrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private Image _imgRnd;
    [SerializeField] private float _baseYPos;
    [SerializeField] private float _timeCycle;

    public void UpdateArrow(bool state)
    {
        if (_sprRnd != null)
            _sprRnd.enabled = state;

        if (_imgRnd != null)
            _imgRnd.enabled = state;

        if (state)
            Animation();
        else
            transform.DOKill();
    }

    private void Animation()
    {
        transform.DOMoveY(_baseYPos, _timeCycle).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
    }
}