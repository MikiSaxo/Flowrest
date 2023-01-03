using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Vector3 _worldPosition;
    private Plane _plane = new Plane(Vector3.up, 0);
    private bool _isOnIndicator;
    private bool _isOnUI;

    void Update()
    {
        if (Camera.main != null)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!_plane.Raycast(ray, out var distance)) return;

            _worldPosition = ray.GetPoint(distance);
        }

        transform.position = _worldPosition;

        // if (Input.GetMouseButtonUp(0) && !_isOnIndicator && !_isOnUI)
            // n_MapManager.Instance.ResetButtonSelected();
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
