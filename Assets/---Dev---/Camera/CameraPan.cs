using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _dragOrigin;
    private bool _canZoom;
    private float groundZ = 0;

    [Header("Zoom")] [SerializeField] private float _zoomSpeed;

    [Header("Anim Rotation")] [SerializeField]
    private Vector2Int _rotaXStartEnd;

    [SerializeField] private float _durationAnimRota;

    [Header("Block Cam")] [Tooltip("It represents the min and max value for the X position")] [SerializeField]
    private Vector2Int _minMaxPosX;

    [Tooltip("It represents the min and max value for the Y position")] [SerializeField]
    private Vector2Int _minMaxPosY;

    [Tooltip("It represents the min and max value for the Z position")] [SerializeField]
    private Vector2Int _minMaxPosZ;

    private Vector3 _camPosStartDrag;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _canZoom = true;


        _cam.transform.DORotate(new Vector3(_rotaXStartEnd.x, 0, 0), 0);
        _cam.transform.DORotate(new Vector3(_rotaXStartEnd.y, 0, 0), _durationAnimRota);
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
            groundZ = _cam.transform.position.z;
            // _camPosStartDrag = _cam.transform.position;
            var getPos = GetWorldPosition(groundZ);
            // if (getPos.y < 0)
            getPos = new Vector3(getPos.x, 10, getPos.z);

            _dragOrigin = getPos;
        }

        if (Input.GetMouseButton(2))
        {
            // Get the delta between startPos and ActualPos
            Vector3 dif = _dragOrigin - GetWorldPosition(groundZ);
            // Add the dif to the cam pos and check if it's clamped
            _cam.transform.position = ClampCamera(_cam.transform.position + new Vector3(dif.x, 0, dif.z));
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
        // zoom = Mathf.Clamp(zoom, _minMaxZoom.x, _minMaxZoom.y);

        // Update the camera's position based on the clamped zoom level
        var posX = pos.x;
        pos = pos.normalized * zoom;
        pos = new Vector3(posX, pos.y, pos.z);

        _cam.transform.position = ClampCamera(pos);

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
        float newX = Mathf.Clamp(targetPosition.x, _minMaxPosX.x, _minMaxPosX.y);
        float newY = Mathf.Clamp(targetPosition.y, _minMaxPosY.x, _minMaxPosY.y);
        float newZ = Mathf.Clamp(targetPosition.z, _minMaxPosZ.x, _minMaxPosZ.y);

        return new Vector3(newX, newY, newZ);
    }

    public void HasEnterredScrollBar(bool which)
    {
        _canZoom = !which;
    }
}