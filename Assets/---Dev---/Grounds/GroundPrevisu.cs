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

    public void ActivateIcon(int indexTile)
    {
        _sprRnd.sprite = _iconTile[indexTile];
        _sprRnd.enabled = true;
        _indexIcon = indexTile;
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
        return _iconTile[index];
    }
}
