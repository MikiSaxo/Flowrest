using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Camera Cam;
    private RaycastHit _hit;
    private string groundTag = "Ground";
    
    [SerializeField] private bool _isMouseWhoControl = false;
    [SerializeField] private float _speed = 0f;
    private Rigidbody _rb;
    private NavMeshAgent _nav;
    private Vector2 _input = Vector2.zero;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        if (!_isMouseWhoControl)
        {
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out _hit, Mathf.Infinity))
                {
                    if (_hit.collider.CompareTag(groundTag)) 
                    {
                        _nav.SetDestination(_hit.point);
                    }
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (!_isMouseWhoControl)
        {
            Vector2 mov = new Vector2(_input.x, _input.y) * _speed;
            _rb.velocity = new Vector3(mov.x, 0, mov.y);
        }
        else
        {
        }
    }
}