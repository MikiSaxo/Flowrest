using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")]
    [SerializeField] private GameObject _fBDnd;
    [SerializeField] private GameObject _buttonBackwards;
    [SerializeField] private InventoryTilePreview _inventoryTilePreview;
    // public OpenCloseMenu GroundStockage;

    [Header("Inventory")] [SerializeField] private GameObject _bigParentInventory;
    [SerializeField] private GameObject _gridParent;
    [SerializeField] private GameObject _prefabTileButton;
    [SerializeField] private GameObject _bgInventory;
    [SerializeField] private float _durationCloseOpen;

    // [SerializeField] private GameObject[] _groundButtons;
    // [SerializeField] private float _timeSpawnButton;
    // [SerializeField] private GameObject[] _UITemperature;

    [Header("Ground Data")] [SerializeField]
    private GroundUIData[] _groundData;

    private bool _hasRecycling;
    private List<GameObject> _stockTileButton = new List<GameObject>();
    private Vector2 _widthBG;
    private Vector2 _widthBGsaveStart;
    private Vector2 _widthBGCurrent;
    private Vector2 _widthIcon;

    private void Awake()
    {
        Instance = this;

        var bgSize = _bgInventory.GetComponent<RectTransform>().rect;
        _widthBGsaveStart = new Vector2(bgSize.width, bgSize.height);
        _widthBGCurrent = new Vector2(bgSize.width, bgSize.height);

        GameObject go = Instantiate(_prefabTileButton, _gridParent.transform);
        _widthIcon = new Vector2(go.GetComponent<InventoryButton>().GetWidthIcon(), 0);
        Destroy(go);
        UpdateOpacityInventory(0);
    }

    public void SetIfHasRecycling(bool state)
    {
        _hasRecycling = state;
    }

    public void UpdateFbGround(int whichState, GameObject button) // Use by Ground buttons
    {
        UpdateFB((AllStates)whichState, button);
    }

    private void UpdateFB(AllStates state, GameObject button)
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        MapManager.Instance.ResetButtonSelected();
        MapManager.Instance.ResetGroundSelected();

        if (_hasRecycling && MapManager.Instance.NbOfRecycling > 0)
            RecyclingManager.Instance.UpdateRecycling(true);

        _fBDnd.GetComponent<FollowMouseDND>().UpdateObject(_groundData[(int)state].Icon,
            _groundData[(int)state].ColorIcon, _groundData[(int)state].Name);
        MapManager.Instance.LastObjButtonSelected = button;

        if (MapManager.Instance.LastObjButtonSelected.GetComponent<InventoryButton>().GetNumberLeft() <= 0)
        {
            MapManager.Instance.LastObjButtonSelected = null;
            return;
        }

        _fBDnd.SetActive(true);
        _fBDnd.GetComponent<FollowMouseDND>().CanMove = true;
        MapManager.Instance.LastStateButtonSelected = state;
        MapManager.Instance.ChangeActivatedButton(button);
        //GroundStockage.ForcedOpen = true;
    }

    public void UpdateOpacityInventory(int alpha)
    {
        if(alpha == 0)
            _bigParentInventory.GetComponent<CanvasGroup>().alpha = alpha;
        else
            _bigParentInventory.GetComponent<CanvasGroup>().DOFade(alpha, .5f);
    }
    
    public void EndFb() // Use by Ground buttons
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        AudioManager.Instance.PlaySFX("EndDragWithoutPose");

        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();
    }

    public void AddNewGround(int stateNb, bool isStart)
    {
        foreach (var tile in _stockTileButton)
        {
            var currentTile = tile.GetComponent<InventoryButton>();

            if ((int)currentTile.GetStateButton() == stateNb)
            {
                currentTile.UpdateNumberLeft(1);
                currentTile.GetComponent<PointerMotion>().OnLeave();
                if (!isStart)
                {
                    _bigParentInventory.GetComponent<PointerMotion>().Bounce();
                    AudioManager.Instance.PlaySFX("AddTileToInventory");
                }

                return;
            }
        }

        GameObject go = Instantiate(_prefabTileButton, _gridParent.transform);
        go.GetComponent<InventoryButton>().Setup(_groundData[stateNb].ColorIcon, _groundData[stateNb].Icon,
            _groundData[stateNb].GroundState);
        go.GetComponent<PointerMotion>().OnLeave();
        _stockTileButton.Add(go);
        if(!isStart)
            _bigParentInventory.GetComponent<PointerMotion>().Bounce();

        if (isStart)
            ChangeSizeBGBeforeNewGround(stateNb, isStart);
    }

    public void ChangeSizeBGBeforeNewGround(int stateNb, bool isStart)
    {
        if (!isStart)
        {
            foreach (var tile in _stockTileButton)
            {
                var currentTile = tile.GetComponent<InventoryButton>();

                if ((int)currentTile.GetStateButton() == stateNb)
                {
                    return;
                }
            }
        }

        var bgSize = _bgInventory.GetComponent<RectTransform>().rect;
        _widthBG = new Vector2(bgSize.width, bgSize.height);
        _widthBGCurrent += _widthIcon;

        if (isStart)
            ReSizeBgInventory(_widthBGCurrent, 0);
        else
            ReSizeBgInventory(_widthIcon + _widthBG, _durationCloseOpen);
    }

    public void SetActiveBackwardsButton(bool state)
    {
        _buttonBackwards.SetActive(state);
    }

    public void UpdateBackwardButton(bool state)
    {
        _buttonBackwards.GetComponent<Button>().interactable = state;
        _buttonBackwards.GetComponent<PointerMotion>().UpdateCanEnter(state);
    }

    public void UpdatePreviewInventory(bool activate, AllStates state)
    {
        if (activate)
        {
            var grndData = GetGroundUIData((int)state);
            _inventoryTilePreview.InitPreviewTile(grndData.Icon, grndData.ColorIcon);
        }
        else
        {
            _inventoryTilePreview.DeactivatePreviewTile();
        }
    }

    public void GroundEmpty(GameObject button)
    {
        _stockTileButton.Remove(button);

        var bgSize = _bgInventory.GetComponent<RectTransform>().rect;
        _widthBG = new Vector2(bgSize.width, bgSize.height);
        ReSizeBgInventory(_widthBG - _widthIcon, _durationCloseOpen);

        Destroy(button);
    }

    private void ReSizeBgInventory(Vector2 newSize, float duration)
    {
        _bgInventory.GetComponent<RectTransform>().DOKill();
        _bgInventory.GetComponent<RectTransform>().DOSizeDelta(newSize, duration).SetEase(Ease.OutSine);
    }
    
    public bool CheckIfStillGround()
    {
        return _stockTileButton.Count > 0;
    }

    public void ResetAllButtons()
    {
        foreach (var but in _stockTileButton)
        {
            Destroy(but);
        }

        _stockTileButton.Clear();

        _widthBGCurrent = _widthBGsaveStart;
        ReSizeBgInventory(_widthBGsaveStart, 0);
    }

    public void FollowDndDeactivate()
    {
        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();
    }

    public GroundUIData GetGroundUIData(int index)
    {
        return _groundData[index];
    }

    public List<GameObject> GetStockTileButton()
    {
        return _stockTileButton;
    }
}