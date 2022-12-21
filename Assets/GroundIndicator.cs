using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private Material[] _mats;
    private bool _isSelected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _mesh.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isSelected) return;
        
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _mesh.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ResetMat();
            n_MapManager.Instance.ResetGroundSelected();
        }
        
        if (!_mesh.enabled || !Input.GetMouseButtonDown(0)) return;

        if (n_MapManager.Instance.LastNbButtonSelected >= 0)
            gameObject.GetComponentInParent<GroundStateManager>().InitState(n_MapManager.Instance.LastNbButtonSelected);

        else
        {
            if (_isSelected) return;
            
            _isSelected = true;
            _mesh.material = _mats[1];
            _parent.GetComponent<GroundStateManager>().OnSelected();
        }

    }

    public void ResetMat()
    {
        _isSelected = false;
        _mesh.material = _mats[0];
        _mesh.enabled = false;
    }
}