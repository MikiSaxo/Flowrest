using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector3 _worldPosition;
    Plane _plane = new Plane(Vector3.up, 0);
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out var distance))
        {
            _worldPosition = ray.GetPoint(distance);
            transform.position = _worldPosition;// + new Vector3(0,3,0);
        }
    }
}
