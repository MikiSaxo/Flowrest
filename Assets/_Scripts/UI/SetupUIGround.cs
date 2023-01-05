using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")] [SerializeField] private GameObject[] _UIGround;
    [SerializeField] private GameObject[] _UITemperature;
    [SerializeField] private GameObject _fBDnd;

    [Header("Ground")] [SerializeField] private string[] _texts;
    [SerializeField] private Color[] _colors;
    [SerializeField] private int[] _nbLeft;
    [SerializeField] private States[] _groundState;

    [Header("Temperature")] [SerializeField]
    private string[] _textsTemperature;

    [SerializeField] private Color[] _colorsTemperature;
    [SerializeField] private int[] _nbLeftTemperature;

    [Header("Anims")] [SerializeField] private Vector2 _bounceValues;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _UIGround.Length; i++)
        {
            _UIGround[i].GetComponent<nUIButton>().Setup(_texts[i], _colors[i], _nbLeft[i], _groundState[i]);
        }

        for (int j = 0; j < _UITemperature.Length; j++)
        {
            _UITemperature[j].GetComponent<nUIButton>()
                .SetupTemperature(_textsTemperature[j], _colorsTemperature[j], _nbLeftTemperature[j]);
        }
    }

    public void UpdateFbGround(int whichState) // Use by Ground buttons
    {
        UpdateFB(whichState, false);
    }

    public void UpdateFbTemperature(int whichTemperature) // Use by Temperature buttons
    {
        UpdateFB(whichTemperature, true);
    }

    private void UpdateFB(int which, bool isTemp)
    {
        if (n_MapManager.Instance.IsGroundFirstSelected) return;

        n_MapManager.Instance.ResetButtonSelected();
        n_MapManager.Instance.ResetGroundSelected();

        if (isTemp)
        {
            _fBDnd.GetComponent<FollowMouseDND>().UpdateObject(_colorsTemperature[which], _textsTemperature[which]);
            n_MapManager.Instance.LastButtonSelected = _UITemperature[which];
        }
        else
        {
            _fBDnd.GetComponent<FollowMouseDND>().UpdateObject(_colors[which], _texts[which]);
            n_MapManager.Instance.LastButtonSelected = _UIGround[which];
        }

        if (n_MapManager.Instance.LastButtonSelected.GetComponent<nUIButton>().GetNumberLeft() <= 0)
        {
            n_MapManager.Instance.LastButtonSelected = null;
            return;
        }
        
        _fBDnd.SetActive(true);
        _fBDnd.GetComponent<FollowMouseDND>().CanMove = true;
        n_MapManager.Instance.LastNbButtonSelected = which;
    }

    public void EndFb() // Use by Ground buttons
    {
        if (n_MapManager.Instance.IsGroundFirstSelected) return;

        _fBDnd.GetComponent<FollowMouseDND>().AnimDeactivateObject();

        // n_MapManager.Instance.ResetButtonSelected();
        // n_MapManager.Instance.ResetGroundSelected();
    }

    public void BounceButtonAnim(GameObject obj)
    {
        AnimDotween.Instance.BounceAnim(obj, _bounceValues.x, _bounceValues.y);
    }

    public void MouseEnterButton(GameObject obj)
    {
        obj.transform.DOScale(Vector3.one * 1.1f, _bounceValues.x);
    }

    public void MouseLeaveButton(GameObject obj)
    {
        obj.transform.DOScale(Vector3.one, _bounceValues.y);
    }
}