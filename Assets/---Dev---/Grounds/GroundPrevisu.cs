using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPrevisu : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private Sprite[] _iconTile;

    private void Start()
    {
        DeactivateIcon();
    }

    public void ActivateIcon(int indexTile)
    {
        _sprRnd.sprite = _iconTile[indexTile];
        _sprRnd.enabled = true;
    }

    public void DeactivateIcon()
    {
        _sprRnd.enabled = false;
    }

    public Sprite GetIconTile(int index)
    {
        return _iconTile[index];
    }
}
