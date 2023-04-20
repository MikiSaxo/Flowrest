using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHitRaycast : MonoBehaviour
{
    public static MouseHitRaycast Instance;

    public bool IsOnGround { get; set; }
    
    private Vector3 _worldPosition;
    private Plane _plane = new Plane(Vector3.up, 0);
    private bool _isOnIndicator;
    private bool _isOnUI;
    private bool _isBlocked;

    [SerializeField] private float _maxDistance = 1000;
    [SerializeField] private LayerMask _layerToHit;

    private Vector2Int _lastCoordsHit;
    private GroundIndicator _lastGroundHit;
    
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(_isBlocked || MapManager.Instance.IsPosing) return;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layerToHit))
        {
            var newBloc = hit.collider.gameObject.GetComponentInParent<GroundIndicator>();

            if (newBloc == null)
            {
                if(_lastGroundHit != null)
                    _lastGroundHit.OnExitPointer();

                if(Input.GetMouseButtonDown(0))
                    MapManager.Instance.ResetBig();
                
                _lastCoordsHit = new Vector2Int(-1000, -1000);
                IsOnGround = false;
                
                return;
            }

            if (newBloc.GetParentCoords() != _lastCoordsHit)
            {
                if(_lastGroundHit != null)
                    _lastGroundHit.OnExitPointer();
                
                _lastCoordsHit = newBloc.GetParentCoords();
                _lastGroundHit = newBloc;
                _lastGroundHit.OnEnterPointer();
                IsOnGround = true;
            }
        }
        
        //if(_isBlocked) return;
        
        // if (Camera.main != null)
        // {
        //     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //
        //     if (!_plane.Raycast(ray, out var distance)) return;
        //
        //     _worldPosition = ray.GetPoint(distance);
        // }
        //
        // transform.position = _worldPosition;
    }

    public void IsBlockMouse(bool yesOrNot)
    {
        if (yesOrNot)
        {
            _isBlocked = true;
            transform.position = Vector3.one * -100;
        }
        else
            _isBlocked = false;
    }

    public void IsOnIndicator(bool yesOrNot) // Called in GroundIndicator
    {
        _isOnIndicator = yesOrNot;
    }
    public void IsOnUI(bool yesOrNot) // Called in Content Pointer Enter/Exit
    {
        _isOnUI = yesOrNot;
    }
}
