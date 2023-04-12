using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendScroll : MonoBehaviour
{
    [SerializeField] private Image _imgLegend;
    [SerializeField] private Sprite[] _sprLegend;

    private int _count;

    private void Start()
    {
        _count = 0;
        _imgLegend.sprite = _sprLegend[_count];
    }

    public void MoveToLeft()
    {
        _count--;

        if (_count < 0)
            _count = _sprLegend.Length - 1;
        
        _imgLegend.sprite = _sprLegend[_count];
    }
    
    public void MoveToRight()
    {
        _count++;

        if (_count >= _sprLegend.Length)
            _count = 0;
        
        _imgLegend.sprite = _sprLegend[_count];
    }
}
