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

    [Header("Anim Values")] [SerializeField]
    private float _timeEnter;

    [SerializeField] private float _timeExit;

    public bool IsSwapping { get; set; }

    private float _startYPos;
    private float _hoveredYPos;
    private float _selectedYPos;
    private bool _isSelected;

    private bool _isEnteredLimited;
    private bool _isEnteredFree;
    // private Vector2Int _coords;

    // private List<GameObject> _tempEntered = new List<GameObject>();
    // private List<GroundStateManager> _stockPrevisu = new List<GroundStateManager>();

    // private readonly Vector2Int[] _hexOddDirections = new Vector2Int[]
    //     { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, 1), new(1, 1) };
    //
    // private readonly Vector2Int[] _hexPeerDirections = new Vector2Int[]
    //     { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(1, -1), new(-1, -1) };

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
        // OnEnterPointer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        // OnExitPointer(other);
    }

    public void OnEnterPointer()
    {
        _isEnteredFree = true;
        // if (!other.gameObject.GetComponentInParent<FollowMouse>()) return;

        if (_parent.GetCurrentStateEnum() == AllStates.Mountain) return;

        if (_parent.IsPlayerForceSwapBlocked) return;

        if (MapManager.Instance.IsPosing) return;

        if (_parent.IsPlayerNotForcePose && MapManager.Instance.LastObjButtonSelected != null) return;

        // if (MapManager.Instance.LastObjButtonSelected != null)
        // {
        //     if (_parent.GetCurrentStateEnum() == MapManager.Instance.GetLastStateSelected()) return;
        // }

        if (MapManager.Instance.LastObjButtonSelected == null && EnergyManager.Instance.IsEnergyInferiorToCostSwap())
        {
            return;
        }

        if (MapManager.Instance.LastObjButtonSelected != null &&
            EnergyManager.Instance.IsEnergyInferiorToCostLandingGround())
        {
            return;
        }

        if (_parent.JustBeenSwaped && MapManager.Instance.LastObjButtonSelected == null)
        {
            _parent.UpdateNoSwap(true);
            UpdateTileState(TileState.Selected, true);
            return;
        }

        if (MapManager.Instance.IsSwapping) return;

        //other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(true);
        _isEnteredLimited = true;


        if (_isSelected || IsSwapping)
        {
            MapManager.Instance.ResetPreview();
            return;
        }

        if (MapManager.Instance.GetHasGroundSelected())
        {
            // print("call ground swap previsu");
            MapManager.Instance.GroundSwapPreview(_parent.gameObject);
        }
        else if (MapManager.Instance.LastObjButtonSelected)
        {
            MapManager.Instance.GroundSwapPreviewButton(_parent.gameObject,
                MapManager.Instance.LastStateButtonSelected);
        }

        OnEnterAnim(_timeEnter);
    }

    public void OnExitPointer()
    {
        _parent.UpdateFbNoSwap(false);

        _isEnteredLimited = false;
        _isEnteredFree = false;

        if (_isSelected || IsSwapping ||
            MapManager.Instance.IsSwapping) return;


        // other.gameObject.GetComponentInParent<FollowMouse>().IsOnIndicator(false);

        _parent.IsProtectedPrevisu = false;

        MapManager.Instance.ResetPreview();

        OnLeaveAnim(_timeExit);
    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(1) && MapManager.Instance.GetHasFirstSwap()) // Right click to reset
        // {
        //     ResetIndicator();
        //     MapManager.Instance.ResetGroundSelected();
        //
        //     // ResetAllAroundPrevisu();
        // }
        
        if(EnergyManager.Instance.GetCurrentEnergy() <= 0 && Input.GetMouseButtonDown(0) && _isEnteredFree && !MapManager.Instance.IsVictory)
            EnergyManager.Instance.SpawnNoEnergyText();

        // Block
        if (!_isEnteredLimited || !Input.GetMouseButtonUp(0) || MapManager.Instance.IsPosing) return;


        // First case: select bloc for swap
        if (MapManager.Instance.LastObjButtonSelected == null)
        {
            if (MapManager.Instance.IsSwapping) return;

            // Reset if click again on it
            if (_isSelected && _isEnteredLimited && !MapManager.Instance.IsTuto)
            {
                // _parent.IsProtectedPrevisu = false;
                //
                // MapManager.Instance.ResetPreview();
                //
                // OnLeaveAnim(_timeExit);
                ResetIndicator();
                StartCoroutine(WaitALittleToReset());
                return;
            }

            if (_isSelected && _isEnteredLimited && MapManager.Instance.IsTuto)
                return;

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

    IEnumerator WaitALittleToReset()
    {
        yield return new WaitForSeconds(.01f);
        MapManager.Instance.ForceResetBig();
    }

    private void MoveYMesh(float height, float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(height, duration);
    }

    public void OnEnterAnim(float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_hoveredYPos, duration);

        if (_parent.CurrentMeshManager != null)
        {
            // print(_parent.CurrentMeshManager);
            UpdateTileState(TileState.Selected, false);
        }
    }

    private void OnLeaveAnim(float duration)
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_startYPos, duration).SetEase(Ease.OutSine);

        if (_parent.CurrentMeshManager != null)
        {
            UpdateTileState(TileState.Normal, false);
        }
    }

    public void UpdateTileState(TileState state, bool isReset)
    {
        if (_parent.JustBeenSwaped && state != TileState.Bored) return;

        // print("blabla blaa bored " + _parent.GetCoords() + " / " + state);
        _parent.CurrentMeshManager.UpdateTexture(state, isReset);
    }

    private void PoseBloc()
    {
        // Idk if really helpful but security
        if (!MapManager.Instance.CanPoseBloc()) return;

        // Avoid to update by same ground
        // if (gameObject.GetComponentInParent<GroundStateManager>().IdOfBloc ==
        //     (int)MapManager.Instance.LastStateButtonSelected) return;

        MapManager.Instance.IsPosing = true;

        StartCoroutine(PoseBlocTime());
    }

    IEnumerator PoseBlocTime()
    {
        // Reset Preview
        MapManager.Instance.ResetPreview();

        // Init the new State
        _parent.InitState(MapManager.Instance.LastStateButtonSelected);

        // Make Anim
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(15, 0).OnComplete(() => { OnLeaveAnim(_timeExit); });

        // Decrease number on selected UI Button 
        MapManager.Instance.DecreaseNumberButton();

        MapManager.Instance.ResetButtonSelected();

        yield return new WaitForSeconds(_timeExit);

        // Change State around
        gameObject.GetComponentInParent<GroundStateManager>().UpdateGroundsAround(_parent.GetCurrentStateEnum());
        gameObject.GetComponentInParent<GroundStateManager>().LaunchDropFX();

        // Spend energy
        EnergyManager.Instance.ReduceEnergyByLandingGround();

        // Has Crystal
        gameObject.GetComponentInParent<CrystalsGround>().UpdateCrystals(false, false);

        yield return new WaitForSeconds(1.5f);

        // Launch Quest
        MapManager.Instance.QuestsManager.CheckQuest();

        // Reset Two last grounds swapped
        MapManager.Instance.ResetTwoLastSwapped();

        // Check if Game Over
        MapManager.Instance.CheckIfGameOver();

        // Save all actions
        LastMoveManager.Instance.SaveNewMap();

        // Reset
        MapManager.Instance.IsPosing = false;

        yield return new WaitForSeconds(.01f);
        ResetForNextChange();
        // MapManager.Instance.ResetBig();

        if (MapManager.Instance.IsPlayerForcePoseBlocAfterSwap)
            MapManager.Instance.UpdateAllGroundTutoForcePose(false);
    }

    public void ResetIndicator()
    {
        _isSelected = false;
        _isEnteredLimited = false;

        OnLeaveAnim(_timeExit);
        MapManager.Instance.IsGroundFirstSelected = false;
    }

    private void ResetForNextChange()
    {
        // Block if was not drag n drop or if the button is empty
        if (MapManager.Instance.CheckIfButtonIsEmpty())
            MapManager.Instance.ResetAllSelection();


        if (!MapManager.Instance.GetIsDragNDrop() && !MapManager.Instance.CheckIfButtonIsEmpty()) return;

        // Reset to avoid problem with dnd
        MapManager.Instance.ResetGroundSelected();
    }

    public Vector2Int GetParentCoords()
    {
        return _parent.GetCoords();
    }
}