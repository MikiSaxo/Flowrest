using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class RecyclingManager : MonoBehaviour
{
    public static RecyclingManager Instance;

    [SerializeField] private GameObject _recycling;
    [SerializeField] private TextMeshProUGUI _recyclingNbText;
    [SerializeField] private GameObject _recyclingNoLeft;

    private bool _hasInfinitRecycling;
    private int _maxRecycling;
    private int _currentLeftRecycling;
    
    private void Awake()
    {
        Instance = this;
    }

    public void UpdateRecycling(bool activateOrNot)
    {
        _recycling.SetActive(activateOrNot);
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(activateOrNot);
        UpdateDisplayNoRecyclingLeft(false);
    }

    public void UpdateDisplayNoRecyclingLeft(bool state)
    {
        _recyclingNoLeft.SetActive(state);
    }

    public void InitNbRecycling(int number, bool hasInfinit)
    {
        _hasInfinitRecycling = hasInfinit;
        _maxRecycling = number;
        _currentLeftRecycling = _maxRecycling;
        _recyclingNbText.gameObject.SetActive(!_hasInfinitRecycling);
        UpdateDisplayRecyclingNbLeft();
    }
    
    public void UpdateNbRecyclingLeft()
    {
        if (_hasInfinitRecycling) return;

        _currentLeftRecycling--;
        UpdateDisplayRecyclingNbLeft();
    }

    private void UpdateDisplayRecyclingNbLeft()
    {
        if(!_hasInfinitRecycling)
            _recyclingNbText.text = $"{_currentLeftRecycling}/{_maxRecycling}";
    }
}
