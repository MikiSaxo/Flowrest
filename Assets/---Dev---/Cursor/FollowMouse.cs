using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public static FollowMouse Instance;

    public bool IsOnGround { get; set; }
    
    private Vector3 _worldPosition;
    private Plane _plane = new Plane(Vector3.up, 0);
    private bool _isOnIndicator;
    private bool _isOnUI;
    private bool _isBlocked;

    private Vector2Int _lastCoordsHit;
    private GroundIndicator _lastGroundHit;
    
    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if(_isBlocked) return;
        
         if (Camera.main != null)
         {
             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
             if (!_plane.Raycast(ray, out var distance)) return;
        
             _worldPosition = ray.GetPoint(distance);
         }
        
         transform.position = _worldPosition;
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
