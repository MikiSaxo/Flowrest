using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundManager : MonoBehaviour
{
    public bool CanBeMoved = false;
    public Vector2Int GroundCoords = Vector2Int.zero;
    [SerializeField] private Material[] _groundMats;
    [SerializeField] private GameObject _indicator;

    private float _startYPos;
    private bool _isEntered;
    public bool IsSelected;
    private bool _isArounded;
    private Material _mat;


    private void Start()
    {
        _startYPos = transform.position.y;
        _mat = _indicator.GetComponent<MeshRenderer>().material;
        OnLeaved(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnLeaved(false);
    }

    public void ChangeCoords(Vector2Int newCoords)
    {
        GroundCoords = newCoords;
    }

    private void OnEntered()
    {
        if (IsSelected) return;

        _indicator.GetComponent<MeshRenderer>().material = _groundMats[0];
        _isEntered = true;
    }

    public void ResetMat()
    {
        _isArounded = false;
        _isEntered = false;
        IsSelected = false;
        _indicator.GetComponent<MeshRenderer>().material = _mat;
    }
    public void OnLeaved(bool hasForced)
    {
        if (hasForced) IsSelected = false;

        if (IsSelected) return;

        _indicator.GetComponent<MeshRenderer>().material = _isArounded ? _groundMats[1] : _mat;

        _isEntered = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnSelected()
    {
        if (IsSelected) return;
        
        IsSelected = true;
        _indicator.GetComponent<MeshRenderer>().material = _groundMats[2];
        MapManager.Instance.CheckIfSelected(gameObject, GroundCoords);
        // print("OnSelected");
    }

    public void OnArounded()
    {
        _isArounded = true;
        if (_isArounded && !_isEntered && !IsSelected)
            _indicator.GetComponent<MeshRenderer>().material = _groundMats[1];
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isEntered)
        {
            OnSelected();
        }
    }
}