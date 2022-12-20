using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private MeshRenderer _indicator;
    private bool _isSelected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _indicator.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<FollowMouse>())
        {
            _indicator.enabled = false;
        }
    }

    private void Update()
    {
        if (!_indicator.enabled || !Input.GetMouseButtonDown(0)) return;

        gameObject.GetComponentInParent<GroundStateManager>().InitState(n_MapManager.Instance.LastNbButtonSelected);
    }
}