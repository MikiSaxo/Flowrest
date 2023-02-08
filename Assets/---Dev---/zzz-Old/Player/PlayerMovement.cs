using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Camera Cam;

    private RaycastHit _hit;
    private string _groundTag = "Ground";
    private NavMeshAgent _nav;
    private Rigidbody _rb;
    private bool _hasStopped = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ChangeCoords(Vector3 coords)
    {
        // Get the NavMeshAgent component
        _nav = GetComponent<NavMeshAgent>();
        // Update the coords for the spawn point
        _nav.Move(coords);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!MapManager.Instance.IsEditMode)
        {
            if (Input.GetMouseButton(0))
                HandleMovement();
            if (_hasStopped)
                _hasStopped = false;
        }
        else
        {
            if (!_hasStopped) // Avoid to call it every tic
            {
                // Get the last pos of the player and stop it
                _nav.destination = transform.position;
                _hasStopped = true;
            }
        }
    }

    private void HandleMovement()
    {
        // Get a Raycast of the mouse's pos in the screen 
        Ray ray = Cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        // Check if touch something
        if (Physics.Raycast(ray, out _hit, Mathf.Infinity))
        {
            // Check if collider with the _groundTag 
            if (_hit.collider.CompareTag(_groundTag))
            {
                // Update the nav destination
                _nav.SetDestination(_hit.point);
            }
        }
    }
}