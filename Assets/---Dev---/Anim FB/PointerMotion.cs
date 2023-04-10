using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PointerMotion : MonoBehaviour
{
    [SerializeField] private float _timeEnter, _timeLeave, _punchForce, _scaleEnter;
    [SerializeField] private int _vibrato;

    [SerializeField] private bool _canEnter;
    
    public void OnEnter()
    {
        if (!_canEnter) return;
        
        transform.DOKill();
        transform.DOScale(_scaleEnter, _timeEnter).SetEase(Ease.InOutSine);
    }

    public void OnLeave()
    {
        if (!_canEnter) return;

        transform.DOKill();
        transform.DOScale(_scaleEnter, 0);
        transform.DOScale(1, _timeLeave);

        // transform.DOScale(1f, 0);
        // transform.DOPunchScale(Vector3.one * _punchForce, _timeLeave, _vibrato);
    }

    public void UpdateCanEnter(bool state)
    {
        if (!state)
            OnLeave();
        
        _canEnter = state;
    }
}
