using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Setup")] [SerializeField] private GameObject[] _buttons;
    [SerializeField] private bool _keepButtonOn;
    [SerializeField] private bool _hasFirstButtonSelected;
    [SerializeField] private bool _isLanguage;

    [Header("Sprites Button")] [SerializeField]
    private Sprite _buttonIdle;

    [SerializeField] private Sprite _buttonHighlight;
    [SerializeField] private Sprite _buttonSelected;
    [SerializeField] private Sprite _buttonSelectedHighlight;

    private GameObject _lastButtonSelected;
    private bool[] _buttonsIsSelected;
    private bool[] _buttonsIsEntered;

    private void Start()
    {
        _buttonsIsSelected = new bool[_buttons.Length];
        _buttonsIsEntered = new bool[_buttons.Length];

        if (_hasFirstButtonSelected)
        {
            if (_isLanguage)
            {
                if(LanguageManager.Instance.Tongue == Language.English)
                    UpdateButton(1, true);
                else
                    UpdateButton(0, true);
            }
            else
                UpdateButton(0, true);
        }
    }

    public void OnEnter(GameObject button)
    {
        if (_keepButtonOn)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] == button)
                {
                    if (_buttonsIsSelected[i])
                        button.GetComponent<Image>().sprite = _buttonSelectedHighlight;
                    else
                        button.GetComponent<Image>().sprite = _buttonHighlight;

                    _buttonsIsEntered[i] = true;

                    return;
                }
            }
        }
        else
        {
            if (_lastButtonSelected != null && _lastButtonSelected == button)
                button.GetComponent<Image>().sprite = _buttonSelectedHighlight;
            else
                button.GetComponent<Image>().sprite = _buttonHighlight;
        }
    }

    public void OnLeave(GameObject button)
    {
        if (_keepButtonOn)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] == button)
                {
                    if (_buttonsIsSelected[i])
                        button.GetComponent<Image>().sprite = _buttonSelected;
                    else
                        button.GetComponent<Image>().sprite = _buttonIdle;

                    _buttonsIsEntered[i] = false;

                    return;
                }
            }
        }
        else
        {
            if (_lastButtonSelected != null && _lastButtonSelected == button)
                button.GetComponent<Image>().sprite = _buttonSelected;
            else
                button.GetComponent<Image>().sprite = _buttonIdle;
        }
    }

    public void OnClick(GameObject button)
    {
        if (_keepButtonOn)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] == button)
                {
                    _buttonsIsSelected[i] = !_buttonsIsSelected[i];

                    if (_buttonsIsEntered[i])
                    {
                        if (_buttonsIsSelected[i])
                            button.GetComponent<Image>().sprite = _buttonSelectedHighlight;
                        else
                            button.GetComponent<Image>().sprite = _buttonHighlight;
                    }
                    else
                    {
                        if (_buttonsIsSelected[i])
                            button.GetComponent<Image>().sprite = _buttonSelected;
                        else
                            button.GetComponent<Image>().sprite = _buttonIdle;
                    }

                    return;
                }
            }
        }
        else
        {
            if (_lastButtonSelected != null)
                _lastButtonSelected.GetComponent<Image>().sprite = _buttonHighlight;
            _lastButtonSelected = button;
            _lastButtonSelected.GetComponent<Image>().sprite = _buttonSelectedHighlight;
        }
    }


    public void UpdateButton(int index, bool state)
    {
        _buttonsIsSelected[index] = state;

        // if (state)
        //     _buttons[index].GetComponent<Image>().sprite = _buttonSelectedHighlight;
        // else
        //     _buttons[index].GetComponent<Image>().sprite = _buttonHighlight;
        if (!_keepButtonOn)
            _lastButtonSelected = _buttons[index];
        
        
        if (_buttonsIsEntered[index])
        {
            if (_buttonsIsSelected[index])
                _buttons[index].GetComponent<Image>().sprite = _buttonSelectedHighlight;
            else
                _buttons[index].GetComponent<Image>().sprite = _buttonHighlight;
        }
        else
        {
            if (_buttonsIsSelected[index])
                _buttons[index].GetComponent<Image>().sprite = _buttonSelected;
            else
                _buttons[index].GetComponent<Image>().sprite = _buttonIdle;
        }
    }
}