using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private Material[] _mats;
    [SerializeField] private GameObject _meshParent;

    private float _startYPos;
    private float _hoveredYPos;
    private float _selectedYPos;
    private bool _isSelected;

    private void Start()
    {
        _startYPos = _meshParent.transform.position.y;
        _hoveredYPos = _startYPos + 1;
        _selectedYPos = _startYPos + 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _mesh.enabled = true;
            if (_isSelected) return;
            MoveYMesh(_hoveredYPos, .3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isSelected) return;
        
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _mesh.enabled = false;
            MoveYMesh(_startYPos, .1f);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetMat();
            n_MapManager.Instance.ResetGroundSelected();
        }
        
        if (_mesh.enabled && n_MapManager.Instance.LastButtonSelected != null && Input.GetMouseButtonUp(0))
        {
            PoseBloc();
        } 

        if (!_mesh.enabled || !Input.GetMouseButtonDown(0)) return;

        if (n_MapManager.Instance.LastButtonSelected != null)
        {
            PoseBloc();
        }

        else
        {
            if (_isSelected) return;
            
            _isSelected = true;
            MoveYMesh(_selectedYPos, .3f);
            _mesh.material = _mats[1];
            _parent.GetComponent<GroundStateManager>().OnSelected();
        }
    }

    public void ResetMat()
    {
        _isSelected = false;
        _mesh.material = _mats[0];
        _mesh.enabled = false;
        MoveYMesh(_startYPos, .1f);
    }

    private void MoveYMesh(float height, float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(height, duration);
    }

    private void PoseBloc()
    {
        if (!n_MapManager.Instance.CanPoseBloc()) return;
            
        if(gameObject.GetComponentInParent<GroundStateManager>().IdOfBloc == n_MapManager.Instance.LastNbButtonSelected) return;
            
        gameObject.GetComponentInParent<GroundStateManager>().InitState(n_MapManager.Instance.LastNbButtonSelected);
        n_MapManager.Instance.DecreaseNumberButton();

        if (!n_MapManager.Instance.IsDragNDrop) return;
        
        n_MapManager.Instance.ResetGroundSelected();
        n_MapManager.Instance.ResetButtonSelected();
    }
}