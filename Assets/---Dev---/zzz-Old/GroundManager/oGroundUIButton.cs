using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oGroundUIButton : MonoBehaviour
{
    [SerializeField] private GameObject _selectedIcon;

    private void Start()
    {
        NeedActivateSelectedIcon(false);
    }

    public void NeedActivateSelectedIcon(bool which)
    {
        _selectedIcon.SetActive(which);
    }
}
