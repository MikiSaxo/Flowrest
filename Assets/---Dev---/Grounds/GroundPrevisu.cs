using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundPrevisu : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private Sprite _noSwap;
    [SerializeField] private Sprite[] _iconTile;

    private int _indexIcon;
    
    private void Start()
    {
        DeactivateIcon();
    }

    public void ActivateIcon(int index)
    {
        // _sprRnd.sprite = _iconTile[indexTile];
        _sprRnd.sprite = SetupUIGround.Instance.GetGroundUIData(index).Icon;
        _sprRnd.color = SetupUIGround.Instance.GetGroundUIData(index).ColorIcon;
        _sprRnd.enabled = true;
        _indexIcon = index;
    }

    public void DeactivateIcon()
    {
        _sprRnd.enabled = false;
    }

    public bool IsIconActivated()
    {
        return _sprRnd.enabled;
    }

    public int GetIndexActualIcon()
    {
        return _indexIcon;
    }

    public void UpdateSwap(bool state)
    {
        _sprRnd.enabled = state;
        _sprRnd.sprite = _noSwap;
    }

    public Sprite GetIconTile(int index)
    {
        return SetupUIGround.Instance.GetGroundUIData(index).Icon;
    }
}
