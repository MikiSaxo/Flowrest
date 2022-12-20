using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

enum States
{
    Plain = 0,
    Desert = 1,
    Water = 2
}

public class nGroundUIButton : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private States _stateButton;
    [SerializeField] private TextMeshProUGUI _textButton;
    [SerializeField] private Image _colorButton;
    [SerializeField] private GameObject _selectedIcon;
    [Header("Possibilities")]
    [SerializeField] private string[] _texts;
    [SerializeField] private Color[] _colors;

    private void Start()
    {
        _colorButton.color = _colors[(int)_stateButton];
        _textButton.text = _texts[(int)_stateButton];
        NeedActivateSelectedIcon(false);
    }

    public void NeedActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }

    public int GetStateButton()
    {
        return (int)_stateButton;
    }
}