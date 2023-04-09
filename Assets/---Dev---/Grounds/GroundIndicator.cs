using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;

public class GroundIndicator : MonoBehaviour
{
    [SerializeField] private GroundStateManager _parent;

    // [SerializeField] private MeshRenderer _mesh;
    // [SerializeField] private Material[] _mats;
    [SerializeField] private GameObject _meshParent;
    
    public bool IsSwapping { get; set; }
    
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

    public void SetStartYPos(float value)
    {
        _startYPos = value;
    }

    private void Start()
    {
        _startYPos = 0;
        _hoveredYPos = _startYPos + HOVERED_Y_POS;
        _selectedYPos = _startYPos + SELECTED_Y_POS;
    }

    // public void ForceEntered()
    // {
    //     _isEntered = true;
    //     MoveYMesh(_hoveredYPos, .3f);
    // }

    private void OnTriggerEnter(Collider other)
    {
        OnEnterPointer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnExitPointer(other);
    }

    private void OnEnterPointer(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;

        if (_parent.GetCurrentStateEnum() == AllStates.Mountain) return;

        if (_parent.IsPlayerForceSwapBlocked) return;

        if (MapManager.Instance.LastObjButtonSelected != null)
        {
            if (_parent.GetCurrentStateEnum() == MapManager.Instance.GetLastStateSelected()) return;
        }

        if (MapManager.Instance.LastObjButtonSelected == null && EnergyManager.Instance.IsEnergyInferiorToCostSwap())
            return;

        if (MapManager.Instance.LastObjButtonSelected != null &&
            EnergyManager.Instance.IsEnergyInferiorToCostLandingGround())
            return;

        if (_parent.JustBeenSwaped && MapManager.Instance.LastObjButtonSelected == null)
        {
            _parent.UpdateNoSwap(true);
            return;
        }
        
        if (MapManager.Instance.IsSwapping) return;

        other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(true);
        _isEntered = true;


        if (_isSelected || IsSwapping)
        {
            MapManager.Instance.ResetPreview();
            return;
        }

        if (MapManager.Instance.GetHasGroundSelected())
        {
            // print("call ground swap previsu");
            MapManager.Instance.GroundSwapPrevisu(_parent.gameObject);
        }
        else if (MapManager.Instance.LastObjButtonSelected)
        {
            MapManager.Instance.GroundSwapPrevisuButton(_parent.gameObject, MapManager.Instance.LastStateButtonSelected);
        }

        OnEnterAnim(.2f);
    }

    private void OnExitPointer(Collider other)
    {
        _parent.UpdateFbNoSwap(false);

        if (_isSelected ||IsSwapping || !other.gameObject.GetComponentInParent<FollowMouse>() || MapManager.Instance.IsSwapping) return;


        other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(false);
        _isEntered = false;
        _parent.IsProtectedPrevisu = false;

        MapManager.Instance.ResetPreview();
        
        OnLeaveAnim(.75f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && MapManager.Instance.GetHasFirstSwap()) // Right click to reset
        {
            ResetIndicator();
            MapManager.Instance.ResetGroundSelected();

            // ResetAllAroundPrevisu();
        }
        
        // Block if not mouseEnter or not click up
        if (!_isEntered || !Input.GetMouseButtonUp(0)) return; 

        
        // First case: select bloc for swap
        if (MapManager.Instance.LastObjButtonSelected == null) 
        {
            // Block if click again on it
            if (_isSelected || MapManager.Instance.IsSwapping) return;

            // Useful to block Trigger enter and exit
            _isSelected = true;

            // Make animation
            if (!MapManager.Instance.IsGroundFirstSelected)
                MoveYMesh(_selectedYPos, .3f);

            // Avoid to transform the bloc by clicking on UI Ground Button after selected first
            MapManager.Instance.IsGroundFirstSelected = true;
            // Call its parent to tell which one was selected to MapManager
            _parent.OnSelected(); 

            // If Player forced swap
            if (!_parent.IsPlayerForceSwapBlocked)
            {
                MapManager.Instance.UpdateSecondBlocForce();

                if (!MapManager.Instance.GetHasFirstSwap())
                    _parent.GetFbArrow().UpdateArrow(false);
            }
        }
        else // Second case: Change state of pose with a new one
            PoseBloc();
    }

    private void MoveYMesh(float height, float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(height, duration);
    }

    private void OnEnterAnim(float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_hoveredYPos, duration);
    }

    private void OnLeaveAnim(float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_startYPos, duration).SetEase(Ease.OutElastic);
    }

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

        // Spend energy
        EnergyManager.Instance.ReduceEnergyByLandingGround();

        // Disable Trash
        //RecyclingManager.Instance.UpdateRecycling(false);

        // Launch Quest
        MapManager.Instance.QuestsManager.CheckQuest();

        // Reset Two last grounds swapped
        MapManager.Instance.ResetTwoLastSwapped();

        // Check if Game Over
        MapManager.Instance.CheckIfGameOver();

        // Make Anim
        // MoveYMesh(15, 0);
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(15, 0).OnComplete(() => { OnLeaveAnim(.75f); });
        // OnLeaveAnim(2);
        
        // Reset
        MapManager.Instance.ResetPreview();
        ResetForNextChange();
    }

    public void ResetIndicator()
    {
        _isSelected = false;
        // _mesh.material = _mats[0];
        // _mesh.enabled = false;
        _isEntered = false;
        //CheckHasWaterMesh();
        // MoveYMesh(_startYPos, .1f);
        OnLeaveAnim(.75f);
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
}