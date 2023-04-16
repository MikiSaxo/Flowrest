using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using DG.Tweening;
using UnityEngine;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")] [SerializeField] private GameObject _fBDnd;
    // public OpenCloseMenu GroundStockage;

    [Header("Inventory")] [SerializeField] private GameObject _gridParent;
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
    private Vector2 _widthBGTest;
    private Vector2 _widthIcon;

    private void Awake()
    {
        Instance = this;
        
        var bgSize = _bgInventory.GetComponent<RectTransform>().rect;
        _widthBGsaveStart = new Vector2(bgSize.width, bgSize.height);
        _widthBGTest = new Vector2(bgSize.width, bgSize.height);
    }

    private void Start()
    {
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

        if (MapManager.Instance.LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() <= 0)
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

    public void EndFb() // Use by Ground buttons
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();

        //RecyclingManager.Instance.UpdateRecycling(false);

        // n_MapManager.Instance.ResetButtonSelected();
        // n_MapManager.Instance.ResetGroundSelected();
    }

    public void AddNewGround(int stateNb, bool isStart)
    {
        foreach (var tile in _stockTileButton)
        {
            var currentTile = tile.GetComponent<UIButton>();

            if ((int)currentTile.GetStateButton() == stateNb)
            {
                currentTile.UpdateNumberLeft(1);
                currentTile.GetComponent<PointerMotion>().OnLeave();

                return;
            }
        }

        GameObject go = Instantiate(_prefabTileButton, _gridParent.transform);
        go.GetComponent<UIButton>().Setup(_groundData[stateNb].ColorIcon, _groundData[stateNb].Icon,
            _groundData[stateNb].GroundState);
        go.GetComponent<PointerMotion>().OnLeave();
        _stockTileButton.Add(go);


        _widthIcon = new Vector2(go.GetComponent<UIButton>().GetWidthIcon(), 0);
        var bgSize = _bgInventory.GetComponent<RectTransform>().rect;
        _widthBG = new Vector2(bgSize.width, bgSize.height);
        _widthBGTest += _widthIcon;
        
        if (isStart)
            ReSizeBgInventory(_widthBGTest, 0);
        else
            ReSizeBgInventory(_widthIcon + _widthBG, _durationCloseOpen);
    }

    public void AddTempGround()
    {
        GameObject go = Instantiate(_fBDnd, gameObject.transform);
        go.transform.DOMove(Vector3.one * 10000, 0);
        _stockTileButton.Add(go);
        Destroy(go, .1f);
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

    public void UpdateInventory(bool state)
    {
        //GroundStockage.gameObject.SetActive(state);
        _bgInventory.SetActive(state);
    }

    public bool CheckIfStillGround()
    {
        // foreach (var tileButton in _stockTileButton)
        // {
        //     if (tileButton.gameObject.activeSelf)
        //         return true;
        // }
        //
        // return false;
        return _stockTileButton.Count != 0;
    }

    public void ResetAllButtons()
    {
        foreach (var but in _stockTileButton)
        {
            Destroy(but);
        }

        _stockTileButton.Clear();

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
}