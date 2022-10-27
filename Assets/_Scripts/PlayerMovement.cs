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
    // private Vector2 _input = Vector2.zero;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void ChangeCoords(Vector3 _coords)
    {
        _nav = GetComponent<NavMeshAgent>();
        _nav.Move(_coords);
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        //HandleMovement();
    }

    private void HandleInput()
    {
        // {
        //     _input.x = Input.GetAxis("Horizontal");
        //     _input.y = Input.GetAxis("Vertical");
        // }
        // else
        if (!MapManager.IsEditMode)
        {
            if (Input.GetMouseButton(0))
            {
                HandleMovement();
            }
        }
    }

    private void HandleMovement()
    {
        // if (!_isEditMode)
        // {
        //     Vector2 mov = new Vector2(_input.x, _input.y) * _speed;
        //     _rb.velocity = new Vector3(mov.x, 0, mov.y);
        // }
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