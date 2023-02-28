using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _parent;

    // [SerializeField] private MeshRenderer _mesh;
    // [SerializeField] private Material[] _mats;
    [SerializeField] private GameObject _meshParent;

    private float _startYPos;
    private float _hoveredYPos;
    private float _selectedYPos;
    private bool _isSelected;
    private bool _isEntered;
    private Vector2Int _coords;
    private List<GameObject> _tempEntered = new List<GameObject>();
    private List<GroundStateManager> _stockPrevisu = new List<GroundStateManager>();

    private readonly Vector2Int[] _hexOddDirections = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, 1), new(1, 1) };

    private readonly Vector2Int[] _hexPeerDirections = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(1, -1), new(-1, -1) };

    private const float HOVERED_Y_POS = 1;
    private const float SELECTED_Y_POS = 2;

    private void Start()
    {
        _startYPos = _meshParent.transform.position.y;
        _hoveredYPos = _startYPos + HOVERED_Y_POS;
        _selectedYPos = _startYPos + SELECTED_Y_POS;
    }

    public void ForceEntered()
    {
        _isEntered = true;
        MoveYMesh(_hoveredYPos, .3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;

        other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(true);
        _isEntered = true;

        //CheckHasWaterMesh();
        CallAllAroundForPrevisu();
        CallSelectedPrevisu();
        //MapManager.Instance.SetCurrentEntered(_parent.GetComponent<GroundStateManager>());
        // CheckIfTemperatureSelected();

        if (_isSelected) return;

        MoveYMesh(_hoveredYPos, .3f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isSelected || !other.gameObject.GetComponentInParent<FollowMouse>()) return;

        other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(false);
        _isEntered = false;

        // ValuesSignForGround.Instance.NoValue();
        //CheckHasWaterMesh();
        ResetAllAroundPrevisu();
        // MapManager.Instance.ResetAroundSelectedPrevisu();
        //MapManager.Instance.ResetCurrentEntered();
        // ResetTemperatureSelected();

        MoveYMesh(_startYPos, .1f);
    }

    // private void CheckHasWaterMesh()
    // {
    //     if (_parent.GetComponent<GroundStateManager>().IdOfBloc == 2)
    //         _parent.GetComponent<GroundStateManager>().EnabledWaterCubes(_isEntered);
    // }

    // private void CheckIfTemperatureSelected()
    // {
    //     _coords = _parent.GetComponent<GroundStateManager>().GetCoords();
    //
    //     if (MapManager.Instance.TemperatureSelected != 0)
    //     {
    //         Vector2Int[] hexDirections = new Vector2Int[6];
    //         // Important for the offset with hex coords
    //         hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;
    //
    //         foreach (var hexPos in hexDirections)
    //         {
    //             Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
    //             // Check if inside of array
    //             if (newPos.x < 0 || newPos.x >= MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
    //                 newPos.y >= MapManager.Instance.MapGrid.GetLength(1)) continue;
    //             // Check if something exist
    //             if (MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
    //             // Check if has GroundManager
    //             if (!MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>())
    //                 continue;
    //
    //             MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()
    //                 .ForceEnteredIndicator();
    //
    //             _tempEntered.Add(MapManager.Instance.MapGrid[newPos.x, newPos.y]);
    //         }
    //     }
    // }

    // private void ResetTemperatureSelected()
    // {
    //     foreach (var ground in _tempEntered)
    //     {
    //         ground.GetComponent<GroundStateManager>().ResetIndicator();
    //     }
    //
    //     _tempEntered.Clear();
    // }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click to reset
        {
            ResetIndicator();
            MapManager.Instance.ResetGroundSelected();
        }

        if (!_isEntered || !Input.GetMouseButtonUp(0)) return; // Block if not mouseEnter or not click up

        if (MapManager.Instance.LastObjButtonSelected == null) // First case: select bloc for swap
        {
            if (_isSelected) return; // Block if click again on it

            _isSelected = true; // Useful to block Trigger enter and exit
            // Make animation
            MoveYMesh(_selectedYPos, .3f);
            // Avoid to transform the bloc by clicking on UI Ground Button after selected first
            MapManager.Instance.IsGroundFirstSelected = true;
            _parent.GetComponent<GroundStateManager>()
                .OnSelected(); // Call its parent to tell which one was selected to MapManager
        }
        else // Second case: Change state of pose with a new one
            PoseBloc();
        // ChangeBlocOrTemperature(); // Transform the bloc with new state
    }

    private void MoveYMesh(float height, float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(height, duration);
    }

    // private void ChangeBlocOrTemperature()
    // {
    // if (MapManager.Instance.TemperatureSelected == 0)
    // PoseBloc();
    // else
    // ChangeTemperature();
    // }

    // private void ChangeTemperature()
    // {
    //     //gameObject.GetComponentInParent<GroundStateManager>().ChangeTemperature(n_MapManager.Instance.TemperatureSelected);
    //     // gameObject.GetComponentInParent<GroundStateManager>().GetValuesAround();
    //
    //      gameObject.GetComponentInParent<GroundFeedbackTemperature>()
    //      .LaunchFB(MapManager.Instance.TemperatureSelected > 0);
    //
    //     ResetForNextChange();
    // }

    private void PoseBloc()
    {
        // Idk if really helpful but security
        if (!MapManager.Instance.CanPoseBloc()) return;

        // Avoid to update by same ground
        if (gameObject.GetComponentInParent<GroundStateManager>().IdOfBloc ==
            (int)MapManager.Instance.LastStateButtonSelected) return;

        // Init the new State
        gameObject.GetComponentInParent<GroundStateManager>().InitState(MapManager.Instance.LastStateButtonSelected);
        gameObject.GetComponentInParent<GroundStateManager>().UpdateGroundsAround();

        ResetAllAroundPrevisu();
        ResetForNextChange();
    }

    private void CallAllAroundForPrevisu()
    {
        // print("hello");
        _coords = _parent.GetComponent<GroundStateManager>().GetCoords();

        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= MapManager.Instance.MapGrid.GetLength(1)) continue;
            // Check if something exist
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>())
                continue;

            var ground = MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            ground.LookingActivateIconPrevisu();

            _stockPrevisu.Add(ground);
        }
    }

    private void CallSelectedPrevisu()
    {
        MapManager.Instance.PrevisuAroundSelected(_parent.GetComponent<GroundStateManager>().GetCurrentStateEnum());
    }

    public void ResetIndicator()
    {
        _isSelected = false;
        // _mesh.material = _mats[0];
        // _mesh.enabled = false;
        _isEntered = false;
        //CheckHasWaterMesh();
        MoveYMesh(_startYPos, .1f);
        MapManager.Instance.IsGroundFirstSelected = false;
    }

    private void ResetForNextChange()
    {
        MapManager.Instance.DecreaseNumberButton(); // Decrease number on selected UI Button 
        //MapManager.Instance.CheckForBiome();

        // Block if was not drag n drop or if the button is empty
        if (MapManager.Instance.CheckIfButtonIsEmpty())
            MapManager.Instance.ResetAllSelection();

        if (!MapManager.Instance.GetIsDragNDrop() && !MapManager.Instance.CheckIfButtonIsEmpty()) return;

        MapManager.Instance.ResetGroundSelected(); // Reset to avoid problem with dnd
        MapManager.Instance.ResetButtonSelected();
    }

    public void ResetAllAroundPrevisu()
    {
        foreach (var previsu in _stockPrevisu)
        {
            previsu.DeactivateIconPrevisu();
        }

        _stockPrevisu.Clear();
    }
}