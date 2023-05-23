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
    [SerializeField] private FB_Arrow _arrowTuto;
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
        
        GetComponent<PointerMotion>().Bounce();
        
        UpdateDisplayRecyclingNbLeft();
    }

    public void UpdateDisplayRecyclingNbLeft()
    {
        if (!_hasInfinitRecycling)
            _recyclingNbText.text = $"{MapManager.Instance.NbOfRecycling} {LanguageManager.Instance.GetRecycleText()}";

        UpdateVisualState();
        // _recyclingNbText.text = $"{_currentLeftRecycling}/{_maxRecycling}";
    }

    public void ActivateButton()
    {
        if (ScreensManager.Instance.GetIsDialogTime()) return;
        
        _isSelected = true;
        OnEnter();
    }

    public void OnEnter()
    {
        if (MapManager.Instance.NbOfRecycling > 0)
            GetComponentInChildren<Image>().color = Color.yellow;
    }

    public void OnExit()
    {
        if (_isSelected) return;

        UpdateVisualState();
    }

    public void DeselectRecycle()
    {
        _isSelected = false;
        OnExit();
    }

    private void UpdateVisualState()
    {
        if (MapManager.Instance.NbOfRecycling > 0)
        {
            GetComponentInChildren<Image>().color = Color.white;
            GetComponent<PointerMotion>().UpdateCanEnter(true);
        }
        else
        {
            GetComponentInChildren<Image>().color = Color.grey;
            GetComponent<PointerMotion>().UpdateCanEnter(false);
        }
    }

    public void UpdateArrowTuto(bool state)
    {
        _arrowTuto.UpdateArrow(state);
    }
}