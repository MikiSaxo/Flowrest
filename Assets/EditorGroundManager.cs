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
    [SerializeField] private GameObject _energy;
    [SerializeField] private float _valueAnimSize;
    [SerializeField] private float _valueSize;
    [SerializeField] private float _valueMovingUp;

    private float _startPosY;
    private bool _isEntered;
    private GameObject _currentGround;
    private char _currentCharState;
    private Vector2Int _coords;

    private const char NONE = 'N';

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
            _indicator.transform.DOMoveY(_startPosY, _valueAnimSize / 2);
    }

    private void Update()
    {
        if (!_isEntered) return;
        
        if (Input.GetMouseButton(0))
        {
            if (EditorMapManager.Instance.GetCharSelectedButton() != 'C')
                InstantiateGround();
            else
                InstantiateEnergy();
        }

        if (Input.GetMouseButton(1))
        {
            if (EditorMapManager.Instance.GetCharSelectedButton() == 'C')
                DestroyEnergy();
            else
                DestroyGround();
        }
    }

    public void UpdateCoords(int x, int y)
    {
        _coords = new Vector2Int(x, y);
    }

    private void InstantiateGround()
    {
        if (_currentGround != null)
        {
            _currentCharState = NONE;
            Destroy(_currentGround);
        }

        if (EditorMapManager.Instance.GetObjSelectedButton() == null) return;

        GameObject go = Instantiate(EditorMapManager.Instance.GetObjSelectedButton(), _indicator.transform);
        _currentGround = go;
        _currentCharState = EditorMapManager.Instance.GetCharSelectedButton();

        EditorMapManager.Instance.UpdateMap(_currentCharState, _coords);
    }

    private void InstantiateEnergy()
    {
        if (_currentGround == null) return;

        EditorSaveMap.Instance.UpdateCoordsEnergy(_coords);
        _energy.SetActive(true);
    }

    private void DestroyEnergy()
    {
        _energy.SetActive(false);
    }

    public void DestroyGround()
    {
        DestroyEnergy();
        _currentCharState = NONE;
        EditorMapManager.Instance.UpdateMap(_currentCharState, _coords);
        Destroy(_currentGround);
    }
}