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

    // [SerializeField] private float _cooldownReset;
    [Header("Arrow FB")]
    [SerializeField] private GameObject _arrowIcon;
    [SerializeField] private float _timeRotateArrow;
    // private float _cooldownToClose;

    public bool ForcedOpen { get; set; }
    public bool IsMenuPauseOpen { get; set; }

    public void OpenAnim()
    {
        // _objToMove.transform.DOKill();
        // _objToMove.transform.DOMove(_tpPoints[1].position, _openDuration);

        if (_arrowIcon != null)
            _arrowIcon.transform.DORotate(new Vector3(0, 0, 180), _timeRotateArrow);
        
        _isClosed = false;
    }

    public void CloseAnim()
    {
        // _objToMove.transform.DOKill();
        // _objToMove.transform.DOMove(_tpPoints[0].position, _closeDuration);

        if (_arrowIcon != null)
            _arrowIcon.transform.DORotate(new Vector3(0, 0, 0), _timeRotateArrow);
        
        _isClosed = true;
    }

    public void MoveMenu()
    {
        if (ScreensManager.Instance != null)
        {
            if (ScreensManager.Instance.GetIsDialogTime()) return;
        }

        if (_isClosed)
            OpenAnim();
        else
            CloseAnim();
    }

    public void OnMouseEntered()
    {
        if (ScreensManager.Instance != null)
        {
            if (ScreensManager.Instance.GetIsDialogTime()) return;
        }
        
        if (!_isTriggered) return;

        // _cooldownToClose = _cooldownReset;
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

    public void CloseQuick()
    {
        _isClosed = true;
        _objToMove.transform.position = _tpPoints[0].position;
    }

    private void Update()
    {
        // if (!_isTriggered || ForcedOpen || IsMenuPauseOpen) return;
        //
        // if (_cooldownToClose > 0)
        //     _cooldownToClose -= Time.deltaTime;
        // else
        //     CloseAnim();
        
        if(!_isClosed)
            _objToMove.transform.position = Vector3.Lerp(_objToMove.transform.position, _tpPoints[1].position, Time.deltaTime * _openDuration);
        else
            _objToMove.transform.position = Vector3.Lerp(_objToMove.transform.position, _tpPoints[0].position, Time.deltaTime * _closeDuration);
    }
}