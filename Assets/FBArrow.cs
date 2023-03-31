using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FBArrow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private float _baseYPos;
    [SerializeField] private float _timeCycle;

    public void UpdateArrow(bool state)
    {
        _sprRnd.enabled = state;

        if(state)
            Animation();
        else
            transform.DOKill();
    }

    private void Animation()
    {
        transform.DOMoveY(_baseYPos, _timeCycle).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
    }
}
