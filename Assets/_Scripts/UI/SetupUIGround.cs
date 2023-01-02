using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SetupUIGround : MonoBehaviour
{
    public static SetupUIGround Instance;

    [Header("Setup")] 
    [SerializeField] private GameObject[] _uiButtons; 
    [SerializeField] private GameObject _fBDnd; 
    
    [Header("Banners")]
    [SerializeField] private string[] _texts;
    [SerializeField] private Color[] _colors;
    [SerializeField] private int[] _nbLeft;
    [SerializeField] private States[] _groundState;

    [Header("Anims")]
    [SerializeField] private Vector2 _bounceValues;
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < _uiButtons.Length; i++)
        {
            _uiButtons[i].GetComponent<nGroundUIButton>().Setup(_texts[i], _colors[i], _nbLeft[i], _groundState[i]);
        }
    }

    public void UpdateFb(int whichState) // Use by Ground buttons
    {
        if (n_MapManager.Instance.IsGroundFirstSelected) return;
        
        n_MapManager.Instance.ResetButtonSelected();
        n_MapManager.Instance.ResetGroundSelected();
        _fBDnd.SetActive(true);
        _fBDnd.GetComponent<FollowMouseDND>().CanMove = true;
        _fBDnd.GetComponent<FollowMouseDND>().UpdateObject(_colors[whichState], _texts[whichState]);
        n_MapManager.Instance.LastNbButtonSelected = whichState;
        n_MapManager.Instance.LastButtonSelected = _uiButtons[whichState];
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
