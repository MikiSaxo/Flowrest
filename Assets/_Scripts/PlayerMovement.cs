using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Camera Cam;

    private RaycastHit _hit;
    private string _groundTag = "Ground";
    private NavMeshAgent _nav;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ChangeCoords(Vector3 coords)
    {
        _nav = GetComponent<NavMeshAgent>();
        _nav.Move(coords);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!MapManager.IsEditMode)
        {
            if (Input.GetMouseButton(0))
                HandleMovement();
        }
    }

    private void HandleMovement()
    {
        Ray ray = Cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity))
        {
            if (_hit.collider.CompareTag(_groundTag))
            {
                _nav.SetDestination(_hit.point);
            }
        }
    }
}