using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class oGroundEditorManager : MonoBehaviour
{
    public char _symbol;
    public Vector2Int GroundCoords = Vector2Int.zero;
    [SerializeField] private Material[] _groundMats;
    [SerializeField] private GameObject _indicator;

    private bool _isSelected; // It's public for a security test -> Must be changed in the future
    private bool _isEntered;

    private const int MainScene = 0;
    private const int EditorScene = 1;

    private void Start()
    {
        oEditorManager.Instance.ChangeModeEvent += OnActivateIndicator;
        OnActivateIndicator();
        ResetMat();
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
        if (_isSelected) return;

        ChangeMat(_indicator, 1);
        _isEntered = true;
    }

    private void OnLeaved()
    {
        if (_isSelected) return;

        ChangeMat(_indicator, 0);
        _isEntered = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void OnSelected()
    {
        if (_isSelected) return;

        _isSelected = true;
        ChangeMat(_indicator, 3);
        oEditorManager.Instance.ChangeLastGroundSelected(gameObject);
    }

    public void ResetMat()
    {
        _isEntered = false;
        _isSelected = false;

        ChangeMat(_indicator, 0);
    }

    private void Update()
    {
        // Select Ground
        if (Input.GetMouseButtonDown(0) && _isEntered)
            OnSelected();
    }

    private void ChangeMat(GameObject which, int mat)
    {
        which.GetComponent<MeshRenderer>().material = _groundMats[mat];
    }

    private void OnActivateIndicator()
    {
       _indicator.SetActive(oEditorManager.Instance.IsEditMode);
    }

    public void EditorTransformTo(GameObject which, bool[] waterData)
    {
        GameObject go = Instantiate(which, transform.position, Quaternion.identity);
        // go.transform.parent = EditorLevelParent.Instance.gameObject.transform;
        
        if(go.GetComponent<oWaterEditorManager>())
            go.GetComponent<oWaterEditorManager>().ChangeWaterDir(waterData);
        
        oEditorManager.Instance.UpdateGridSwap(go, GroundCoords, go.GetComponent<oGroundEditorManager>()._symbol);
        
        Destroy(gameObject);
    }


    private void OnDisable()
    {
       oEditorManager.Instance.ChangeModeEvent -= OnActivateIndicator;
    }
}