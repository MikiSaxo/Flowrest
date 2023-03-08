using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")] [SerializeField] private GameObject _fBDnd;
    public OpenCloseMenu GroundStockage;

    [Header("Ground Buttons")] [SerializeField]
    private GameObject[] _groundButtons;
    // [SerializeField] private GameObject[] _UITemperature;

    [Header("Ground Data")] [SerializeField] private GroundUIData[] _groundData;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _groundButtons.Length; i++)
        {
            var getData = _groundData[i];
            _groundButtons[i].GetComponent<UIButton>().Setup(getData.Name, getData.ColorIcon, getData.Icon,
                getData.NbLeft, getData.GroundState);
            GroundEmpty(i);
        }

        // for (int j = 0; j < _UITemperature.Length; j++)
        // {
        //     _UITemperature[j].GetComponent<UIButton>()
        //         .SetupTemperature(_textsTemperature[j], _colorsTemperature[j], _nbLeftTemperature[j]);
        // }
    }

    public void UpdateFbGround(int whichState) // Use by Ground buttons
    {
        UpdateFB((AllStates)whichState);
    }

    // public void UpdateFbTemperature(int whichTemperature) // Use by Temperature buttons
    // {
    //     //UpdateFB(whichTemperature, true);
    // }

    private void UpdateFB(AllStates state)
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        MapManager.Instance.ResetButtonSelected();
        MapManager.Instance.ResetGroundSelected();
        
        TrashCrystalManager.Instance.UpdateTrashCan(true);

        _fBDnd.GetComponent<FollowMouseDND>()
            .UpdateObject(_groundData[(int)state].Icon, _groundData[(int)state].Name);
        MapManager.Instance.LastObjButtonSelected = _groundButtons[(int)state];

        if (MapManager.Instance.LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() <= 0)
        {
            MapManager.Instance.LastObjButtonSelected = null;
            return;
        }

        _fBDnd.SetActive(true);
        _fBDnd.GetComponent<FollowMouseDND>().CanMove = true;
        MapManager.Instance.LastStateButtonSelected = state;
        MapManager.Instance.ChangeActivatedButton(_groundButtons[(int)state]);
        GroundStockage.ForcedOpen = true;
    }

    public void EndFb() // Use by Ground buttons
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();
        
        TrashCrystalManager.Instance.UpdateTrashCan(false);

        // n_MapManager.Instance.ResetButtonSelected();
        // n_MapManager.Instance.ResetGroundSelected();
    }

    public void AddNewGround(int which)
    {
        _groundButtons[which].SetActive(true);
        _groundButtons[which].GetComponent<UIButton>().UpdateNumberLeft(1);
    }

    public void GroundEmpty(int which)
    {
        _groundButtons[which].SetActive(false);
    }
}