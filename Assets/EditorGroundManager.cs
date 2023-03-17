using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class EditorGroundManager : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject _cube;
    [SerializeField] private float _valueAnimSize;
    [SerializeField] private float _valueSize;
    [SerializeField] private float _valueMovingUp;

    private float _startPosY;
    private bool _isEntered;
    private GameObject _currentGround;
    private char _currentCharState;

    private void Start()
    {
        _startPosY = _indicator.transform.position.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;

        _isEntered = true;

        MoveUpObj();
        UpdateSizeIndicator(_valueSize);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;

        _isEntered = false;

        MoveUpObj();
        UpdateSizeIndicator(1);
    }

    private void InstantiateGround()
    {
        if (_currentGround != null)
        {
            _currentCharState = 'N';
            Destroy(_currentGround);
        }
        
        if (EditorMapManager.Instance.GetObjSelectedButton() == null) return;

        GameObject go = Instantiate(EditorMapManager.Instance.GetObjSelectedButton(), _indicator.transform);
        _currentGround = go;
        _currentCharState = EditorMapManager.Instance.GetCharSelectedButton();
    }

    private void UpdateSizeIndicator(float size)
    {
        _cube.transform.DOKill();
        _cube.transform.DOScale(Vector3.one * size, _valueAnimSize);
    }

    private void MoveUpObj()
    {
        _indicator.transform.DOKill();

        if (_isEntered)
            _indicator.transform.DOMoveY(_indicator.transform.position.y + _valueMovingUp, _valueAnimSize);
        else
            _indicator.transform.DOMoveY(_startPosY, _valueAnimSize/2);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && _isEntered)
            InstantiateGround();
    }
}