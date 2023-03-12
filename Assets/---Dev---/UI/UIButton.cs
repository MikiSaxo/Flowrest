using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButton : MonoBehaviour
{
    [Header("Setup")] public AllStates _stateButton;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Image _iconButton;
    [SerializeField] private GameObject _selectedIcon;
    [SerializeField] private TextMeshProUGUI _textNumber;
    
    // [Header("Setup for Hot n Cold")]
    // [SerializeField] private bool _isTemperature;
    // [SerializeField] private int _temperature;

    private int _numberGroundLeft;

    private void Start()
    {
        ActivateSelectedIcon(false);
    }

    public void Setup(string text, Color color, Sprite icon, int nbLeft, AllStates stat)
    {
        // SetupTemperature(text, color, nbLeft);
        _iconButton.sprite = icon;
        _iconButton.color = color;
        _textButton.text = text;
        UpdateNumberLeft(nbLeft);
        _stateButton = stat;
    }

    // public void SetupTemperature(string text, Color color, int nbLeft)
    // {
    //     _colorButton.color = color;
    //     _textButton.text = text;
    //     SetupNumberLeftTemperature(nbLeft);
    //     // ChangeNumberLeft(nbLeft);
    // }

    public void ActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }

    public AllStates GetStateButton()
    {
        return _stateButton;
    }

    public int GetNumberLeft()
    {
        return _numberGroundLeft;
    }

    public void UpdateNumberLeft(int numberToAdd)
    {
        _numberGroundLeft += numberToAdd;
    
        //_textNumber.text = _isTemperature ? $"{_numberGroundLeft/9}" : $"{_numberGroundLeft}";
        _textNumber.text = $"{_numberGroundLeft}";

        if (_numberGroundLeft <= 0)
            SetupUIGround.Instance.GroundEmpty((int)_stateButton);
    }

    public void ResetToEmpty()
    {
        ActivateSelectedIcon(false);
        _numberGroundLeft = 0;
        SetupUIGround.Instance.GroundEmpty((int)_stateButton);
    }

    // public void SetupNumberLeftTemperature(int nb)
    // {
    //     _numberGroundLeft += nb;
    //     _numberGroundLeft *= 9;
    //     _textNumber.text = $"{_numberGroundLeft/9}";
    // }

    // public bool GetIsTemperature()
    // {
    //     return _isTemperature;
    // }
    //
    // public int GetHisTemperature()
    // {
    //     return _temperature;
    // }
}