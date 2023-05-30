using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class CreditsMovement : MonoBehaviour
{
    [SerializeField] private ScenesManager _scenesManager;
    [SerializeField] private GameObject _elementsToMove;
    //[SerializeField] private float _timeToEnd;
    [SerializeField] private float _endPos;
    [SerializeField] private float _speed;
    
    private bool _canGo;
    private bool _hasReachEndPos;
    private float _moreSpeed;

    private void Start()
    {
        _moreSpeed = 1;
        _canGo = false;
        _hasReachEndPos = false;
        Init();
    }

    public void LaunchMovement()
    {
        //_elementsToMove.transform.DOMoveY(_endPos, _timeToEnd).SetEase(Ease.Linear).OnComplete(_scenesManager.GoToMainMenu);
    }

    public void Init()
    {
        _canGo = true;
    }

    private void Update()
    {
        if (!_canGo) return;

        _moreSpeed = Input.GetMouseButton(0) ? 2 : 1;
        
        _elementsToMove.transform.position += Vector3.up * _speed * _moreSpeed;

        if (_elementsToMove.transform.position.y > _endPos && !_hasReachEndPos)
        {
            _hasReachEndPos = true;
            _scenesManager.GoToMainMenu();
        }
    }
}
