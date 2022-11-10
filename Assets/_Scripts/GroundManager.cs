using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GroundManager : MonoBehaviour
{
    public bool CanBeMoved = false;
    public Vector2Int GroundCoords = Vector2Int.zero;
    [SerializeField] private Material[] _groundMats;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject _indicatorPlayerArounded;

    public bool IsSelected; // It's public for a security test -> Must be changed in the future
    private bool _isEntered;
    private bool _isArounded;
    private bool _isAroundedPlayer;

    private void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            MapManager.Instance.ChangeModeEvent += OnActivateIndicator;
        ResetMat();
        if(CanBeMoved)
            ResetBaseMat();
    }


    private void OnTriggerEnter(Collider other)
    {
        // Detect if the trigger has entered
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnEntered();
    }

    private void OnTriggerExit(Collider other)
    {
        // Detect if the trigger has quit
        if (other.gameObject.GetComponentInParent<FollowMouse>())
            OnLeaved();
    }

    public void ChangeCoords(Vector2Int newCoords) // Change the coords of the ground
    {
        GroundCoords = newCoords;
    } 
    private void OnEntered()
    {
        // Prevent to change the mat if its actually selected
        if (IsSelected || !MapManager.IsEditMode || !CanBeMoved || !_isAroundedPlayer && !_isArounded) return;
        // Change mat and _isEntered
        ChangeMat(_indicator, 1);
        _isEntered = true;
    }

    public void ResetMat()
    {
        _isArounded = false;
        _isEntered = false;
        IsSelected = false;
        _isAroundedPlayer = false;
        //if (GetComponent<WaterFlowing>())
        //{
            //if (GetComponent<WaterFlowing>().IsWater)
            //    GetComponent<WaterFlowing>().ActivateWater();
            //else
             //   GetComponent<WaterFlowing>().DesactivateWater();
        //}
       // else
            ChangeMat(_indicator, 0);

    }

    public void ResetBaseMat()
    {
        _isAroundedPlayer = false;
        // if (GetComponent<WaterFlowing>())
        // {
        //     if (GetComponent<WaterFlowing>().IsWater)
        //         GetComponent<WaterFlowing>().ActivateWater();
        //     else
        //         GetComponent<WaterFlowing>().DesactivateWater();
        // }
        // else
        //     _indicatorPlayerArounded.GetComponent<MeshRenderer>().material = _mat;
        _indicatorPlayerArounded.SetActive(false);
    }

    private void OnLeaved()
    {
        // Prevent to change the mat if its actually selected
        if (IsSelected) return;
        // Put the aroundedMat if it was arounded else base mat
        //_indicator.GetComponent<MeshRenderer>().material = _isArounded ? _groundMats[2] : _mat;
        ChangeMat(_indicator, _isArounded ? 2 : 0);

        // Reset _isEntered
        _isEntered = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnSelected()
    {
        // Don't need to recall this function if its actually selected
        if (IsSelected) return;
        // Change the mat and call CheckIfSelected in the MapManager to swap or not        
        IsSelected = true;
        ChangeMat(_indicator, 3);
        MapManager.Instance.CheckIfGroundSelected(gameObject, GroundCoords);
    }

    public void OnAroundedSelected()
    {
        _isArounded = true;
        // Security if not already entered or selected
        if (_isArounded && !_isEntered && !IsSelected)
            ChangeMat(_indicator, 2);
    }

    public void OnAroundedPlayer()
    {
        _isAroundedPlayer = true;
        // ChangeMat(_indicatorPlayerArounded, 4);
        _indicatorPlayerArounded.SetActive(true);
    }

    private void Update()
    {
        // Select Ground
        if (Input.GetMouseButtonDown(0) && _isEntered)
            OnSelected();
        // See if can't be in Update 
        if (_isEntered && !MapManager.IsEditMode)
            ResetMat();
    }

    private void ChangeMat(GameObject which, int mat)
    {
        which.GetComponent<MeshRenderer>().material = _groundMats[mat];
    }

    private void OnActivateIndicator()
    {
        _indicator.SetActive(MapManager.IsEditMode);
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            MapManager.Instance.ChangeModeEvent -= OnActivateIndicator;
    }
}