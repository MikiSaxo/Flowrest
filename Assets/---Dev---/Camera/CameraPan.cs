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
    [Header("Pan Speed")] [SerializeField] private float _panSpeed;

    [Header("Anim Rotation")] [SerializeField]
    private Vector2Int _rotaXStartEnd;

    [SerializeField] private float _timeLaunchAnimRota;
    [SerializeField] private float _durationAnimRota;

    [Header("Block Cam")] [Tooltip("It represents the min and max value for the X position")] [SerializeField]
    private Vector2Int _minMaxPosX;

    [Tooltip("It represents the min and max value for the Y position")] [SerializeField]
    private Vector2Int _minMaxPosY;

    [Tooltip("It represents the min and max value for the Z position")] [SerializeField]
    private Vector2Int _minMaxPosZ;

    private Vector3 _camPosStartDrag;
    private Vector3 _lastdiffPan;
    private float _timeToAndroidPan;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _canZoom = true;
        
        _cam.transform.DORotate(new Vector3(_rotaXStartEnd.x, 0, 0), 0);
    }

    public void RotationAnimation(float durationRota)
    {
        if (durationRota == 0)
            durationRota = _durationAnimRota;
        
        _cam.transform.DORotate(new Vector3(_rotaXStartEnd.y, 0, 0), durationRota).SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        if (_panSpeed == 0) return;
        
        PanCamera();
        if (_canZoom)
            Zoom();
    }

    private void PanCamera()
    {
        // Change to the good click depending of if is Android
        var mouseIndex = 2;
        if(MapManager.Instance != null)
            mouseIndex = MapManager.Instance.IsAndroid ? 0 : 2;

        // Get the startPos of the mouse
        if (Input.GetMouseButtonDown(mouseIndex))
        {
            groundZ = _cam.transform.position.y;

            var getPos = GetWorldPosition(groundZ);
            // getPos = new Vector3(getPos.x, 10, getPos.z);

            _dragOrigin = getPos;
        }

        if (Input.GetMouseButton(mouseIndex))
        {
            // Get the delta between startPos and ActualPos
            Vector3 dif = _dragOrigin - GetWorldPosition(groundZ);
            // print(dif);
            //
            _timeToAndroidPan += Time.deltaTime;
            if (MapManager.Instance != null  && dif != _lastdiffPan && MapManager.Instance.IsAndroid && _timeToAndroidPan > .15f && MapManager.Instance.LastObjButtonSelected == null)
            {
                MouseHitRaycast.Instance.IsOnGround = true;
                
                if(MapManager.Instance.LastGroundEntered != null)
                    MapManager.Instance.LastGroundEntered.GetComponent<GroundIndicator>().OnExitPointer();
                
                MapManager.Instance.ResetBig();
            }
            
            _lastdiffPan = dif;
            // Add the dif to the cam pos and check if it's clamped
            _cam.transform.position = ClampCamera(_cam.transform.position + new Vector3(dif.x, 0, dif.z));
        }

        if (Input.GetMouseButtonUp(mouseIndex))
        {
            if(MouseHitRaycast.Instance != null)
                MouseHitRaycast.Instance.IsOnGround = false;
            _timeToAndroidPan = 0;
        }
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = _cam.ScreenPointToRay(Input.mousePosition);
        
        Plane ground = new Plane(_cam.transform.forward, new Vector3(0, 0, z));
        ground.Raycast(mousePos, out var distance);
        
        return mousePos.GetPoint(distance * _panSpeed);
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

        if (_cam.transform.position.y >= _minMaxPosY.y && pos.y > _minMaxPosY.y) return;
        
        if (_cam.transform.position.y <= _minMaxPosY.x && pos.y < _minMaxPosY.x) return;

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