using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;

public class OpenCloseMenu : MonoBehaviour
{
    [SerializeField] private Transform[] _tpPoints;
    [SerializeField] private float _openDuration;
    [SerializeField] private float _closeDuration;
    [SerializeField] private GameObject _objToMove;

    [SerializeField] private bool _isClosed;
    [SerializeField] private bool _isTriggered;

    [SerializeField] private float _cooldownReset;
    private float _cooldownToClose;

    public bool ForcedOpen { get; set; }
    public bool IsMenuPauseOpen { get; set; }

    private void OpenAnim()
    {
        _objToMove.transform.DOKill();
        _objToMove.transform.DOMove(_tpPoints[1].position, _openDuration);
    }

    private void CloseAnim()
    {
        _objToMove.transform.DOKill();
        _objToMove.transform.DOMove(_tpPoints[0].position, _closeDuration);
    }

    public void MoveMenu()
    {
        if (ScreensManager.Instance.GetIsDialogTime()) return;

        if (_isClosed)
        {
            OpenAnim();
            _isClosed = false;
        }
        else
        {
            CloseAnim();
            _isClosed = true;
        }
    }

    public void OnMouseEntered()
    {
        if (ScreensManager.Instance.GetIsDialogTime()) return;
        
        if (!_isTriggered) return;

        _cooldownToClose = _cooldownReset;
        OpenAnim();
    }

    public void OpenMenuQuest()
    {
        OpenAnim();
        _isClosed = false;
    }

    public void KeepOpen()
    {
        ForcedOpen = true;
    }

    public void LeaveKeepOpen()
    {
        if (!IsMenuPauseOpen)
            ForcedOpen = false;
    }

    private void Update()
    {
        if (!_isTriggered || ForcedOpen || IsMenuPauseOpen) return;

        if (_cooldownToClose > 0)
            _cooldownToClose -= Time.deltaTime;
        else
            CloseAnim();
    }
}