using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PointerMotion : MonoBehaviour
{
    [Header("Enter / Exit")] [SerializeField]
    private float _timeEnter;

    [SerializeField] private float _timeLeave, _scaleEnter;
    [SerializeField] private bool _canEnter;

    [Header("Bounce")] [SerializeField] private float _duration = 1;
    [SerializeField] private float _punchScale = .2f;
    [SerializeField] private int _vibrato = 4;

    private bool _isBouncing;
    private bool _isEnter;

    public void OnEnter()
    {
        if (!_canEnter || _isBouncing) return;

        if(MapManager.Instance != null && !MapManager.Instance.IsAndroid)
            AudioManager.Instance.PlaySFX("MouseOverButton");
        if(MainMenuManager.Instance != null && !MainMenuManager.Instance.IsAndroid)
            AudioManager.Instance.PlaySFX("MouseOverButton");

        _isEnter = true;

        transform.DOKill();
        transform.DOScale(_scaleEnter, _timeEnter).SetEase(Ease.InOutSine);
    }

    public void OnLeave()
    {
        if (!_canEnter || _isBouncing) return;

        ForceOnLeave();
    }

    private void ForceOnLeave()
    {
        if (!_isEnter) return;

        transform.DOKill();
        transform.DOScale(_scaleEnter, 0);
        transform.DOScale(1, _timeLeave);

        _isEnter = false;
    }

    public void OnClick()
    {
        AudioManager.Instance.PlaySFX("ClickButton");
    }

    public void UpdateCanEnter(bool state)
    {
        _canEnter = state;

        if (!_canEnter)
            ForceOnLeave();
    }

    public void Bounce()
    {
        _isBouncing = true;
        transform.DOPunchScale(Vector3.one * _punchScale, _duration, _vibrato).OnComplete(ReSize);
    }

    private void ReSize()
    {
        _isBouncing = false;
        transform.DOScale(1, _timeLeave);
    }
}