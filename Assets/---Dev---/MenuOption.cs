using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{
    [SerializeField] private GameObject _firstButtonSelected;
    
    private CanvasGroup _canvasGroup;
    private GameObject _saveLastButtonSelected;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _saveLastButtonSelected = _firstButtonSelected;
    }

    public void ActivateMenuOption()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void KeepSelectedButton()
    {
        ChangeSelectedButton(_saveLastButtonSelected);
    }

    public void ChangeSelectedButton(GameObject buttonObject)
    {
        var button = buttonObject.GetComponent<Button>();
        button.Select();
        _saveLastButtonSelected = buttonObject;
    }
}