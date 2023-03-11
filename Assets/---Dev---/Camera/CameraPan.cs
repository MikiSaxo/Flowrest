using System;
using System.Collections;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _dragOrigin;
    private bool _canZoom;
    private float groundZ = 0;

    [SerializeField] private float _zoomSpeed;

    // [SerializeField] private float _rotationSpeed;
    [SerializeField] private Vector2Int _minMaxZoom;

    // [SerializeField] private Vector2Int _minMaxRotation;
    [SerializeField] private Vector2Int _maxMovX;
    [SerializeField] private Vector2Int _maxMovZ;

    private float _startPosX;
    private float _startPosZ;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _canZoom = true;

        _startPosX = transform.position.x;
        _startPosZ = transform.position.z;
    }

    private void Update()
    {
        PanCamera();
        if (_canZoom)
            Zoom();
    }

    private void PanCamera()
    {
        // Get the startPos of the mouse
        if (Input.GetMouseButtonDown(2))
        {
            _dragOrigin = GetWorldPosition(groundZ);
        }

        if (Input.GetMouseButton(2))
        {
            // Get the delta between startPos and ActualPos
            Vector3 dif = _dragOrigin - GetWorldPosition(groundZ);

            // Add the dif to the cam pos
            // if (_cam.transform.position.x + dif.x > _startPosX + _maxMovX.x
            //     || _cam.transform.position.x - dif.x < _startPosX - _maxMovX.y
            //     || _cam.transform.position.z + dif.y > _startPosZ + _maxMovZ.x
            //     || _cam.transform.position.z - dif.y < _startPosZ - _maxMovZ.y)
            //     return;

            _cam.transform.position = ClampCamera(_cam.transform.position + new Vector3(dif.x, 0, dif.y));
            // _cam.transform.position += new Vector3(dif.x, 0, dif.y)
        }
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = _cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(_cam.transform.forward, new Vector3(0, 0, z));
        //Plane ground = new Plane(Vector3.forward, new Vector3(0,0,z));
        ground.Raycast(mousePos, out var distance);
        return mousePos.GetPoint(distance);
    }

    private void Zoom()
    {
        if (Input.mouseScrollDelta.y == 0)
            return;

        // Zoom the camera in or out based on the mouse scroll wheel input
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        float scroll = (Input.mouseScrollDelta.y < 0) ? scroll = 1 : scroll = -1;

        // Clamp the zoom level to the min and max zoom values
        var pos = transform.position;
        float zoom = pos.magnitude + scroll * _zoomSpeed;
        zoom = Mathf.Clamp(zoom, _minMaxZoom.x, _minMaxZoom.y);

        // Update the camera's position based on the clamped zoom level
        var posX = pos.x;
        pos = pos.normalized * zoom;
        pos = new Vector3(posX, pos.y, pos.z);
        transform.position = pos;

        // Rotate the camera based on the mouse X axis input
        // float rotation = scroll * _rotationSpeed;
        //
        // if (transform.rotation.eulerAngles.x < _minMaxRotation.x && scroll < 0 ||
        //     transform.rotation.eulerAngles.x > _minMaxRotation.y && scroll > 0) return;
        //
        // transform.Rotate(Vector3.right, rotation, Space.World);

        //float newSize = _cam.orthographicSize + _zoomStep * whichZoom;
        // Change zoom and Block it if too much
        //_cam.orthographicSize = Mathf.Clamp(newSize, _camSizeEnds.x, _camSizeEnds.y);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float minX = _startPosX - _maxMovX.y;
        float maxX = _startPosX + _maxMovX.x;
        float minZ = _startPosZ - _maxMovZ.y;
        float maxZ = _startPosZ + _maxMovZ.x;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newZ = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        return new Vector3(newX, targetPosition.y, newZ);
    }

    public void HasEnterredScrollBar(bool which)
    {
        _canZoom = !which;
    }
}