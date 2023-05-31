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
    [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _fbTextWarning;

    [Header("Anim Values")] [SerializeField]
    private float _timeEnter;

    [SerializeField] private float _timeExit;

    public bool IsSwapping { get; set; }

    private float _startYPos;
    private float _hoveredYPos;
    private float _selectedYPos;
    private bool _isSelected;
    private int _twoClick;

    private bool _isEnteredLimited;
    private bool _isEnteredFree;
    private GameObject _currentFbBoredText;


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

        if (_parent.GetCurrentStateEnum() == AllStates.__Pyreneos__) return;

        if (_parent.IsPlayerForceSwapBlocked) return;

        if (MapManager.Instance.IsPosing) return;

        if (_parent.IsPlayerNotForcePose && MapManager.Instance.LastObjButtonSelected != null) return;


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

        _isEnteredLimited = true;
        AudioManager.Instance.PlaySFX("TileEntered");

        if (_isSelected || IsSwapping)
        {
            MapManager.Instance.ResetPreview();
            return;
        }

        if (MapManager.Instance.GetHasGroundSelected())
        {
            MapManager.Instance.GroundSwapPreview(_parent.gameObject);
        }
        else if (MapManager.Instance.LastObjButtonSelected)
        {
            MapManager.Instance.GroundSwapPreviewButton(_parent.gameObject,
                MapManager.Instance.LastStateButtonSelected);
        }

        _twoClick++;

        OnEnterAnim(_timeEnter);
    }

    public void OnExitPointer()
    {
        _parent.UpdateFbNoSwap(false);

        _isEnteredLimited = false;
        _isEnteredFree = false;

        if (_isSelected || IsSwapping ||
            MapManager.Instance.IsSwapping) return;

        _parent.IsProtectedPrevisu = false;

        MapManager.Instance.ResetPreview();

        OnLeaveAnim(_timeExit);
    }

    private void Update()
    {
        // If no energy
        if (EnergyManager.Instance.GetCurrentEnergy() <= 0 && Input.GetMouseButtonDown(0) && _isEnteredFree &&
            !MapManager.Instance.IsVictory)
        {
            EnergyManager.Instance.SpawnNoEnergyText();
            
            AudioManager.Instance.PlaySFX("BlockedTileWhenClicked");
            
            return;
        }

        if (_parent.IsBored && Input.GetMouseButtonDown(0) && _isEnteredFree)
        {
            // EnergyManager.Instance.SpawnNoEnergyText();
            if (_currentFbBoredText != null) return;

            GameObject go = Instantiate(_fbTextWarning, EnergyManager.Instance.transform);
            go.GetComponent<TextWarning>().Init(LanguageManager.Instance.GetBoredText());
            _currentFbBoredText = go;
            
            AudioManager.Instance.PlaySFX("BlockedTileWhenClicked");
            
            return;
        }

        // Block
        if (!_isEnteredLimited || !Input.GetMouseButtonUp(0) || MapManager.Instance.IsPosing) return;

        if (MapManager.Instance.IsAndroid)
        {
            if (MapManager.Instance.LastGroundSelected != null && MapManager.Instance.LastObjButtonSelected == null &&
                _twoClick < 2) return;

            if (MapManager.Instance.LastObjButtonSelected != null && MapManager.Instance.LastGroundSelected == null &&
                _twoClick < 2) return;
        }

        // First case: select bloc for swap
        if (MapManager.Instance.LastObjButtonSelected == null)
        {
            if (MapManager.Instance.IsSwapping) return;

            // Reset if click again on it
            if (_isSelected && _isEnteredLimited && !MapManager.Instance.IsTuto)
            {
                ResetIndicator();
                StartCoroutine(WaitALittleToReset());
                AudioManager.Instance.PlaySFX("Unselect");
                return;
            }

            if (_isSelected && _isEnteredLimited && MapManager.Instance.IsTuto)
                return;

            // Useful to block Trigger enter and exit
            _isSelected = true;
            AudioManager.Instance.PlaySFX("TileSelected");

            // Reset Recycle if was selected
            MapManager.Instance.ResetWantToRecycle();
            RecyclingManager.Instance.DeselectRecycle();

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
        // print("ça leave");

        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_startYPos, duration).SetEase(Ease.OutSine);

        if (_parent.CurrentMeshManager != null)
        {
            UpdateTileState(TileState.Normal, false);
        }

        _twoClick = 0;
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

        if (MapManager.Instance.IsPlayerForcePoseBlocAfterSwap)
            MapManager.Instance.UpdateAllGroundTutoForcePose(false);

        yield return new WaitForSeconds(_timeExit);

        // Change State around
        gameObject.GetComponentInParent<GroundStateManager>().UpdateGroundsAround(_parent.GetCurrentStateEnum());
        gameObject.GetComponentInParent<GroundStateManager>().LaunchDropFX();

        // Spend energy
        EnergyManager.Instance.ReduceEnergyByLandingGround();

        // Has Crystal
        if (gameObject.GetComponentInParent<CrystalsGround>().GetIfHasCrystal())
        {
            ItemCollectedManager.Instance.SpawnFBEnergyCollected(1, _parent.transform.position);
            gameObject.GetComponentInParent<CrystalsGround>().UpdateCrystals(false, false);
        }

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
    }

    public void ResetIndicator()
    {
        _isSelected = false;
        _isEnteredLimited = false;
        _twoClick = 0;

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