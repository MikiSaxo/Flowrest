using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class RecyclingManager : MonoBehaviour
{
    public static RecyclingManager Instance;

    [SerializeField] private GameObject _recycling;
    [SerializeField] private TextMeshProUGUI _recyclingNbText;

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
        gameObject.GetComponentInChildren<Button>().interactable = activateOrNot;
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

        if (_currentLeftRecycling <= 0)
        {
            gameObject.GetComponent<PointerMotion>().UpdateCanEnter(false);
            gameObject.GetComponentInChildren<Button>().interactable = false;
        }
        
        UpdateDisplayRecyclingNbLeft();
    }

    private void UpdateDisplayRecyclingNbLeft()
    {
        if(!_hasInfinitRecycling)
            _recyclingNbText.text = $"{_currentLeftRecycling}/{_maxRecycling}";
    }
}
