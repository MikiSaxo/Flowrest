using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryButton : MonoBehaviour
{
    [Header("Setup")] public AllStates _stateButton;
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
        MapManager.Instance.CheckIfWantToRecycle(gameObject);
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
    
        _textNumber.text = $"x{_numberGroundLeft}";

        if (_numberGroundLeft <= 0)
            SetupUIGround.Instance.GroundEmpty(gameObject);
    }

    public void IsOnUI(bool state)
    {
        MapManager.Instance.IsOnUI = state;
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