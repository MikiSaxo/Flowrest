using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum States
{
    Plain = 0,
    Desert = 1,
    Water = 2
}

public class nUIButton : MonoBehaviour
{
    [Header("Setup")] public AllStates _stateButton;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Image _colorButton;
    [SerializeField] private GameObject _selectedIcon;
    [SerializeField] private TextMeshProUGUI _textNumber;
    [Header("Setup for Hot n Cold")]
    [SerializeField] private bool _isTemperature;
    [SerializeField] private int _temperature;

    private int _numberGroundLeft;

    private void Start()
    {
        NeedActivateSelectedIcon(false);
    }

    public void Setup(string text, Color color, int nbLeft, AllStates stat)
    {
        // SetupTemperature(text, color, nbLeft);
        _colorButton.color = color;
        _textButton.text = text;
        ChangeNumberLeft(nbLeft);
        _stateButton = stat;
    }

    public void SetupTemperature(string text, Color color, int nbLeft)
    {
        _colorButton.color = color;
        _textButton.text = text;
        SetupNumberLeftTemperature(nbLeft);
        // ChangeNumberLeft(nbLeft);
    }

    public void NeedActivateSelectedIcon(bool which)
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

    public void ChangeNumberLeft(int decrease)
    {
        _numberGroundLeft += decrease;

        _textNumber.text = _isTemperature ? $"{_numberGroundLeft/9}" : $"{_numberGroundLeft}";
    }

    public void SetupNumberLeftTemperature(int nb)
    {
        _numberGroundLeft += nb;
        _numberGroundLeft *= 9;
        _textNumber.text = $"{_numberGroundLeft/9}";
    }

    public bool GetIsTemperature()
    {
        return _isTemperature;
    }

    public int GetHisTemperature()
    {
        return _temperature;
    }
}