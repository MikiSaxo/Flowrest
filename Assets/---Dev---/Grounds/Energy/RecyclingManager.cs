using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class RecyclingManager : MonoBehaviour
{
    public static RecyclingManager Instance;

    [SerializeField] private GameObject _recycling;
    [SerializeField] private TextMeshProUGUI _recyclingNbText;

    private bool _hasInfinitRecycling;
    private int _maxRecycling;
    // private int _currentLeftRecycling;
    private bool _isSelected;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateRecycling(bool activateOrNot)
    {
        _recycling.SetActive(activateOrNot);
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(activateOrNot);

        // if (!activateOrNot)
        //     return;

        //gameObject.GetComponentInChildren<Button>().interactable = true;
    }

    public void InitNbRecycling(bool hasInfinit)
    {
        _hasInfinitRecycling = hasInfinit;
        // _maxRecycling = number;
        // _currentLeftRecycling = _maxRecycling;
        _recyclingNbText.gameObject.SetActive(!_hasInfinitRecycling);
        UpdateDisplayRecyclingNbLeft();
    }

    public void UpdateNbRecyclingLeft()
    {
        if (_hasInfinitRecycling) return;

        // _currentLeftRecycling--;

        if (MapManager.Instance.NbOfRecycling <= 0)
        {
            gameObject.GetComponent<PointerMotion>().UpdateCanEnter(false);
            // gameObject.GetComponentInChildren<Button>().interactable = false;
        }

        UpdateDisplayRecyclingNbLeft();
    }

    private void UpdateDisplayRecyclingNbLeft()
    {
        if (!_hasInfinitRecycling)
            _recyclingNbText.text = $"{MapManager.Instance.NbOfRecycling} restant(s)";
        // _recyclingNbText.text = $"{_currentLeftRecycling}/{_maxRecycling}";
    }

    public void ActivateButton()
    {
        _isSelected = true;
        OnEnter();
    }

    public void OnEnter()
    {
        GetComponentInChildren<Image>().color = Color.yellow;
    }

    public void OnExit()
    {
        if (_isSelected) return;
        
        GetComponentInChildren<Image>().color = Color.white;
    }

    public void DeactivateButton()
    {
        _isSelected = false;
        OnExit();
    }
}