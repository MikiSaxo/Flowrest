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
    private bool _isEnd;
    private bool _hasReachEndPos;
    private float _moreSpeed;

    private void Start()
    {
        _moreSpeed = 1;
        _hasReachEndPos = false;
    }

    public void Init(bool isEnd)
    {
        _canGo = true;
        _isEnd = isEnd;
    }

    private void Update()
    {
        if (!_canGo) return;

        _moreSpeed = Input.GetMouseButton(0) ? 5 : 1;

        _elementsToMove.transform.position += Vector3.up * _speed * _moreSpeed;

        if (_elementsToMove.transform.position.y > _endPos && !_hasReachEndPos)
        {
            if (_isEnd)
            {
                _hasReachEndPos = true;
                _scenesManager.GoToMainMenu();
            }
            else
            {
                ScreensManager.Instance.HasOutro = true;
                DialogManager.Instance.LaunchAfterOutro();
                ScreensManager.Instance.GoLevelSupp();
            }
        }
    }
}