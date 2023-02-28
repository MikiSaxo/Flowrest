using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")] [SerializeField] private GameObject _fBDnd;
    [SerializeField] private GameObject[] _groundButtons;
    // [SerializeField] private GameObject[] _UITemperature;

    [Header("Ground")] [SerializeField] private GroundUIData[] _groundDatas;
    // [SerializeField] private string[] _texts;
    // [SerializeField] private Color[] _colors;
    // [SerializeField] private int[] _nbLeft;
    // [SerializeField] private AllStates[] _groundState;

    // [Header("Temperature")] [SerializeField]
    // private string[] _textsTemperature;
    //
    // [SerializeField] private Color[] _colorsTemperature;
    // [SerializeField] private int[] _nbLeftTemperature;

    // [Header("Anims")] [SerializeField] private Vector2 _bounceValues;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _groundButtons.Length; i++)
        {
            var getData = _groundDatas[i];
            _groundButtons[i].GetComponent<UIButton>().Setup(getData.Name, getData.ColorIcon, 0, getData.GroundState);
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

        // if (isTemp)
        // {
        //     _fBDnd.GetComponent<FollowMouseDND>().UpdateObject(_colorsTemperature[(int)state], _textsTemperature[(int)state]);
        //     MapManager.Instance.LastObjButtonSelected = _UITemperature[(int)state];
        // }
        // else
        // {
        _fBDnd.GetComponent<FollowMouseDND>()
            .UpdateObject(_groundDatas[(int)state].ColorIcon, _groundDatas[(int)state].Name);
        MapManager.Instance.LastObjButtonSelected = _groundButtons[(int)state];
        // }

        if (MapManager.Instance.LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() <= 0)
        {
            MapManager.Instance.LastObjButtonSelected = null;
            return;
        }

        _fBDnd.SetActive(true);
        _fBDnd.GetComponent<FollowMouseDND>().CanMove = true;
        MapManager.Instance.LastStateButtonSelected = state;
    }

    public void EndFb() // Use by Ground buttons
    {
        if (MapManager.Instance.IsGroundFirstSelected) return;

        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();

        // n_MapManager.Instance.ResetButtonSelected();
        // n_MapManager.Instance.ResetGroundSelected();
    }

    // public void BounceButtonAnim(GameObject obj)
    // {
    //     //AnimDotween.Instance.BounceAnim(obj, _bounceValues.x, _bounceValues.y);
    // }
    //
    // public void MouseEnterButton(GameObject obj)
    // {
    //     obj.transform.DOScale(Vector3.one * 1.1f, _bounceValues.x);
    // }

    // public void MouseLeaveButton(GameObject obj)
    // {
    //     obj.transform.DOScale(Vector3.one, _bounceValues.y);
    // }

    public void AddNewGround(int which)
    {
        _groundButtons[which].GetComponent<UIButton>().UpdateNumberLeft(1);
    }
}