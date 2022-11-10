using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _dragOrigin;

    [SerializeField] private float zoomStep, minCamSize, maxCamSize;

    private void Start()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        PanCamera();
        Zoom();
    }

    private void PanCamera()
    {
        // Get the startPos of the mouse
        if (Input.GetMouseButtonDown(0))
            _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            // Get the delta between startPos and ActualPos
            Vector3 dif = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
            // Add the dif to the cam pos
            _cam.transform.position += dif;
            //print("origin " + dragOrigin + " newPos " + cam.ScreenToWorldPoint(Input.mousePosition) + " = dif " + dif);
        }
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y == 0)
            return;
        
        var whichZoom = 0;
        // Get the good scroll 
        whichZoom = (Input.mouseScrollDelta.y < 0) ? whichZoom = 1 : whichZoom = -1;

        float newSize = _cam.orthographicSize + zoomStep * whichZoom;
        // Change zoom and Block it if too much
        _cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    } 
}