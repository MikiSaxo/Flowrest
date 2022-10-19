using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Vector3 worldPosition;
    Plane _plane = new Plane(Vector3.up, 0);
    void Update()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (_plane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
            transform.position = worldPosition;
        }
    }
}
