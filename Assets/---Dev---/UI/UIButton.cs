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

    private int _numberGroundLeft;

    private void Start()
    {
        ActivateSelectedIcon(false);
    }

    public void Setup(string text, Color color, Sprite icon, int nbLeft, AllStates stat)
    {
        _iconButton.sprite = icon;
        _iconButton.color = color;
        _textButton.text = text;
        UpdateNumberLeft(nbLeft);
        _stateButton = stat;
    }

    public void ActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }

    public void ChangeActivatedButton()
    {
        MapManager.Instance.ChangeActivatedButton(gameObject);
    }

    public void UpdateFbGround(int whichState)
    {
        SetupUIGround.Instance.UpdateFbGround(whichState);
    }

    public void EndFb()
    {
        SetupUIGround.Instance.EndFb();
    }
    
    public void UpdateNumberLeft(int numberToAdd)
    {
        _numberGroundLeft += numberToAdd;
    
        _textNumber.text = $"{_numberGroundLeft}";

        if (_numberGroundLeft <= 0)
            SetupUIGround.Instance.GroundEmpty((int)_stateButton);
    }

    public AllStates GetStateButton()
    {
        return _stateButton;
    }

    public int GetNumberLeft()
    {
        return _numberGroundLeft;
    }

    public void ResetToEmpty()
    {
        ActivateSelectedIcon(false);
        _numberGroundLeft = 0;
        SetupUIGround.Instance.GroundEmpty((int)_stateButton);
    }
}