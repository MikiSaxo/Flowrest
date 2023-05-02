using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransiHexagone : MonoBehaviour
{
    [SerializeField] private float _timeGrowOn;
    [SerializeField] private float _timeShrink;

    private void Start()
    {
        // gameObject.transform.DOKill();
        // gameObject.transform.DOScale(0, 0);
    }

    public void GrowOn()
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(0, 0);
        gameObject.transform.DOScale(.25f, _timeGrowOn);
    }

    public void Shrink()
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(.25f, 0);
        gameObject.transform.DOScale(0, _timeShrink);
    }
}
