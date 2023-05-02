using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransiHexagone : MonoBehaviour
{
 

    private void Start()
    {
        // gameObject.transform.DOKill();
        // gameObject.transform.DOScale(0, 0);
    }

    public void GrowOn(float timeGrowOn)
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(0, 0);
        gameObject.transform.DOScale(.25f, timeGrowOn);
    }

    public void Shrink(float timeShrink)
    {
        gameObject.transform.DOKill();
        gameObject.transform.DOScale(.25f, 0);
        gameObject.transform.DOScale(0, timeShrink);
    }
}
