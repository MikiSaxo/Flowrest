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

    private const float HOVERED_Y_POS = 1;
    private const float SELECTED_Y_POS = 2;
    
    private void Start()
    {
        _startYPos = _meshParent.transform.position.y;
        _hoveredYPos = _startYPos + HOVERED_Y_POS; 
        _selectedYPos = _startYPos + SELECTED_Y_POS;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;
        
        _mesh.enabled = true;
            
        if (_isSelected) return;
            
        MoveYMesh(_hoveredYPos, .3f);
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
        if (Input.GetMouseButtonDown(1)) // Right click to reset
        {
            ResetMat();
            n_MapManager.Instance.ResetGroundSelected();
        }
            
        if (!_mesh.enabled || !Input.GetMouseButtonUp(0)) return; // Block if not mouseEnter or not click up
        

        if (n_MapManager.Instance.LastButtonSelected == null ) // First case: select bloc for swap
        {
            if (_isSelected) return; // Block if click again on it
            
            _isSelected = true; // Useful to block Trigger enter and exit
            MoveYMesh(_selectedYPos, .3f); // Make animation
            _mesh.material = _mats[1]; // Change mat of indicator -> must disappear
            n_MapManager.Instance.IsGroundFirstSelected = true; // Avoid to transform the bloc by clicking on UI Ground Button after selected first
            _parent.GetComponent<GroundStateManager>().OnSelected(); // Call its parent to tell which one was selected to MapManager
        }
        else // Second case: Change state of pose with a new one
            PoseBloc(); // Transform the bloc with new state
    }

    public void ResetMat()
    {
        _isSelected = false;
        _mesh.material = _mats[0];
        _mesh.enabled = false;
        MoveYMesh(_startYPos, .1f);
        n_MapManager.Instance.IsGroundFirstSelected = false;
    }

    private void MoveYMesh(float height, float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(height, duration);
    }

    private void PoseBloc()
    {
        if (!n_MapManager.Instance.CanPoseBloc()) return; // Idk if really helpful but security
            
        if(gameObject.GetComponentInParent<GroundStateManager>().IdOfBloc == n_MapManager.Instance.LastNbButtonSelected) return; // Avoid to update by same ground
            
        gameObject.GetComponentInParent<GroundStateManager>().InitState(n_MapManager.Instance.LastNbButtonSelected); // Init the new State
        n_MapManager.Instance.DecreaseNumberButton(); // Decrease number on selected UI Ground Button 

        if (!n_MapManager.Instance.GetIsDragNDrop()) return; // Block if was not drag n drop
        
        n_MapManager.Instance.ResetGroundSelected(); // Reset to avoid problem with dnd
        n_MapManager.Instance.ResetButtonSelected();
    }
}