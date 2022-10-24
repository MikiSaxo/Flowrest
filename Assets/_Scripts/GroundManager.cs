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

    private bool _isEntered;
    [HideInInspector] public bool IsSelected; //It's public for a security test
    private bool _isArounded;
    private Material _mat;


    private void Start()
    {
        // _mat = _indicator.GetComponent<MeshRenderer>().material;
        _mat = _groundMats[0];
        ResetMat();
    }

    private void OnTriggerEnter(Collider other)
    {
        //detect if the trigger has entered
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        //detect if the trigger has quit
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnLeaved();
    }

    public void ChangeCoords(Vector2Int newCoords)
    {
        GroundCoords = newCoords;
    } //Change the coords of the ground

    private void OnEntered()
    {
        //Prevent to change the mat if its actually selected
        if (IsSelected) return;
        //Change mat and _isEntered
        _indicator.GetComponent<MeshRenderer>().material = _groundMats[1];
        _isEntered = true;
    }

    public void ResetMat()
    {
        _isArounded = false;
        _isEntered = false;
        IsSelected = false;
        _indicator.GetComponent<MeshRenderer>().material = _mat;
    }

    private void OnLeaved()
    {
        //Prevent to change the mat if its actually selected
        if (IsSelected) return;
        //Put the aroundedMat if it was arounded else base mat
        _indicator.GetComponent<MeshRenderer>().material = _isArounded ? _groundMats[2] : _mat;
        //Reset _isEntered
        _isEntered = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnSelected()
    {
        //Don't need to recall this function if its actually selected
        if (IsSelected) return;
        //Change the mat and call CheckIfSelected in the MapManager to swap or not        
        IsSelected = true;
        _indicator.GetComponent<MeshRenderer>().material = _groundMats[3];
        MapManager.Instance.CheckIfSelected(gameObject, GroundCoords);
    }

    public void OnArounded()
    {
        _isArounded = true;
        //Security if not already entered or selected
        if (_isArounded && !_isEntered && !IsSelected)
            _indicator.GetComponent<MeshRenderer>().material = _groundMats[2];
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isEntered)
        {
            OnSelected();
        }
    }
}