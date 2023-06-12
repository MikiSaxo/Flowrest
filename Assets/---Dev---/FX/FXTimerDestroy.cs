using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;

public class FXTimerDestroy : MonoBehaviour
{
    [SerializeField] private float _timeGrow;
    [SerializeField] private float _timeShrink;
    void Start()
    {
        gameObject.transform.DOScale(1, _timeGrow).OnComplete(ShrinkBeforeDestroy);
    }

    private void ShrinkBeforeDestroy()
    {
        gameObject.transform.DOScale(0, _timeShrink).OnComplete(DestroyFx);
    }

    private void DestroyFx()
    {
        Destroy(gameObject);
    }
}