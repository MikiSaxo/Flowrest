using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    private Vector3 _startPos;
    private bool _isEntered;
    private bool _isSelected;

    private void OnTriggerEnter(Collider other)
    {
        OnEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        OnLeaved(false);
    }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void OnSelected()
    {
        if (!_isSelected)
        {
            _isSelected = true;
            transform.DOMoveY(transform.position.y + .5f, .2f);
            MapManager.Instance.DeselectedOldBloc(gameObject);
            // print("OnSelected");
        }
    }

    private void OnEntered()
    {
        if (_isSelected)
            return;
        transform.DOMoveY(transform.position.y + .2f, .2f);
        _isEntered = true;
    }

    public void OnLeaved(bool hasForced)
    {
        if (hasForced)
            _isSelected = false;
        if (_isSelected)
            return;
        transform.DOMove(_startPos, .5f);
        _isEntered = false;
        _isSelected = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isEntered)
        {
            OnSelected();
        }
    }
}