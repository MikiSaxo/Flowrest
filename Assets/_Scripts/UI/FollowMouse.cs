using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector3 _worldPosition;
    Plane _plane = new Plane(Vector3.up, 0);
    void Update()
    {
        if (Camera.main != null)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (!_plane.Raycast(ray, out var distance)) return;
            
            _worldPosition = ray.GetPoint(distance);
        }

        transform.position = _worldPosition;
    }
}
