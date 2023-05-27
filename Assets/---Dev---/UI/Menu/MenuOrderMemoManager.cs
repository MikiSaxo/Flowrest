using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOrderMemoManager : MonoBehaviour
{
    [Header("BG")] [SerializeField] private GameObject _bg;
    [Header("Order")] [SerializeField] private Image _orderButtons;
    [SerializeField] private GameObject _orderContent;
    [SerializeField] private Color _orderNormal;
    [SerializeField] private Color _orderHovered;
    [SerializeField] private Color _orderSelected;
    [SerializeField] private Color _orderHoveredSelected;
    private bool _orderIsSelected;
    
    [Header("Memo")] [SerializeField] private Image _memoButtons; 
    [SerializeField] private GameObject _memoContent;
    [SerializeField] private Color _memoNormal;
    [SerializeField] private Color _memoHovered;
    [SerializeField] private Color _memoSelected;
    [SerializeField] private Color _memoHoveredSelected;
    private bool _memoIsSelected;

    private void Start()
    {
        _orderContent.SetActive(false);
        _memoContent.SetActive(false);
        _bg.SetActive(false);
    }

    public void OnEnterOrder()
    {
        if (_orderIsSelected)
        {
            _orderButtons.color = _orderHoveredSelected;
        }
        else
        {
            _orderButtons.color = _orderHovered;
        }
    }
    
    public void OnLeaveOrder()
    {
        if (_orderIsSelected)
        {
            _orderButtons.color = _orderSelected;
        }
        else
        {
            _orderButtons.color = _orderNormal;
        }
    }

    public void OnClickOrder()
    {
        if (_orderIsSelected)
        {
            _orderContent.SetActive(false);
            _bg.SetActive(false);
            _orderIsSelected = false;
            _orderButtons.color = _orderHovered;
        }
        else
        {
            _orderContent.SetActive(true);
            _bg.SetActive(true);
            _memoContent.SetActive(false);
            _orderIsSelected = true;
            _orderButtons.color = _orderHoveredSelected;
        }
        
        _memoButtons.color = _memoNormal;
        _memoIsSelected = false;
    }

    public void OnActivateOrder()
    {
        _orderIsSelected = false;
        OnClickOrder();
    }
    
    public void OnEnterMemo()
    {
        if (_memoIsSelected)
        {
            _memoButtons.color = _memoHoveredSelected;
        }
        else
        {
            _memoButtons.color = _memoHovered;
        }
    }
    
    public void OnLeaveMemo()
    {
        if (_memoIsSelected)
        {
            _memoButtons.color = _memoSelected;
        }
        else
        {
            _memoButtons.color = _memoNormal;
        }
    }
    
    public void OnClickMemo()
    {
        if (_memoIsSelected)
        {
            _memoContent.SetActive(false);
            _bg.SetActive(false);
            _memoIsSelected = false;
            _memoButtons.color = _memoHovered;
        }
        else
        {
            _memoContent.SetActive(true);
            _bg.SetActive(true);
            _orderContent.SetActive(false);
            _memoIsSelected = true;
            _memoButtons.color = _memoHoveredSelected;
        }

        _orderButtons.color = _orderNormal;
        _orderIsSelected = false;
    }
}
