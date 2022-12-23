using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum States
{
    Plain = 0,
    Desert = 1,
    Water = 2
}

public class nGroundUIButton : MonoBehaviour
{
    [Header("Setup")]
    public States _stateButton;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Image _colorButton;
    [SerializeField] private GameObject _selectedIcon;
    [SerializeField] private TextMeshProUGUI _textNumber;

    private int _numberGroundLeft;

    private void Start()
    {
        NeedActivateSelectedIcon(false);
    }

    public void Setup(string text, Color color, int nbLeft, States stat)
    {
        _colorButton.color = color;
        _textButton.text = text;
        ChangeNumberLeft(nbLeft);
        _stateButton = stat;
    }

    public void NeedActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }

    public int GetStateButton()
    {
        return (int)_stateButton;
    }

    public int GetNumberLeft()
    {
        return _numberGroundLeft;
    }

    public void ChangeNumberLeft(int decrease)
    {
        _numberGroundLeft += decrease;
        _textNumber.text = $"{_numberGroundLeft}";
    }
}