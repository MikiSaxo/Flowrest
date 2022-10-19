using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundManager : MonoBehaviour
{
    private float _startYPos;
    private bool _isEntered;
    private bool _isSelected;
    private bool _isArounded;
    public Vector2 GroundCoords = Vector2.zero;
    private Material _mat;
    [SerializeField] Material[] _groundMats;
    [SerializeField] private GameObject _indicator;


    private void Start()
    {
        _startYPos = transform.position.y;
        _mat = gameObject.GetComponent<MeshRenderer>().material;
        OnLeaved(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnEntered();
        if (other.gameObject.GetComponent<TriggerAroundSelected>())
            OnArounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnLeaved(false);
        if (other.gameObject.GetComponent<TriggerAroundSelected>())
            OnArounded(false);
    }

    public void ChangeCoords(Vector2 newCoords)
    {
        GroundCoords = newCoords;
    }

    public void OnSelected()
    {
        if (!_isSelected)
        {
            _isSelected = true;
            // transform.DOMoveY(transform.position.y + .5f, .2f);
            _indicator.GetComponent<MeshRenderer>().material = _groundMats[2];
            MapManager.Instance.LookIfNextTo(gameObject, GroundCoords);
            // print("OnSelected");
        }
    }

    private void OnEntered()
    {
        if (_isSelected)
            return;
        // transform.DOMoveY(transform.position.y + .2f, .2f);
        _indicator.GetComponent<MeshRenderer>().material = _groundMats[0];
        _isEntered = true;
    }

    public void OnLeaved(bool hasForced)
    {
        if (hasForced)
            _isSelected = false;

        if (_isSelected)
            return;

        if (_isArounded)
            _indicator.GetComponent<MeshRenderer>().material = _groundMats[1];
        else
            _indicator.GetComponent<MeshRenderer>().material = _mat;
        // transform.DOMoveY(_startYPos, 0.5f);
        _isEntered = false;
        _isSelected = false;
    }

    private void OnArounded(bool which)
    {
        _isArounded = which;
        if (_isArounded && !_isEntered && !_isSelected)
            _indicator.GetComponent<MeshRenderer>().material = _groundMats[1];
        else if(!_isArounded && !_isEntered && !_isSelected)
            _indicator.GetComponent<MeshRenderer>().material = _mat;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isEntered)
        {
            OnSelected();
        }
    }
}