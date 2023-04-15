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

    public void Setup(Color color, Sprite icon, AllStates state)
    {
        _iconButton.sprite = icon;
        _iconButton.color = color;
        UpdateNumberLeft(1);
        _stateButton = state;
    }

    public void ActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }

    public void ChangeActivatedButton()
    {
        MapManager.Instance.ChangeActivatedButton(gameObject);
    }

    public void UpdateFbGround()
    {
        SetupUIGround.Instance.UpdateFbGround((int)_stateButton, gameObject);
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
            SetupUIGround.Instance.GroundEmpty(gameObject);
    }

    public AllStates GetStateButton()
    {
        return _stateButton;
    }

    public int GetNumberLeft()
    {
        return _numberGroundLeft;
    }

    public float GetWidthIcon()
    {
        return _iconButton.sprite.textureRect.width;
    }

    public void ResetToEmpty()
    {
        ActivateSelectedIcon(false);
        _numberGroundLeft = 0;
        // SetupUIGround.Instance.GroundEmpty((int)_stateButton);
    }
}