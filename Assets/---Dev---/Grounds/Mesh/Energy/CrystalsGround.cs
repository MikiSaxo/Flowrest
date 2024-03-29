using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsGround : MonoBehaviour
{
    [SerializeField] private GameObject _crystal;

    private bool _isCrystalsConsumed;

    public void InitCrystal()
    {
        _isCrystalsConsumed = false;
        _crystal.SetActive(true);
    }

    public void UpdateCrystals(bool state, bool isInit)
    {
        if (_isCrystalsConsumed) return;

        if (state == false)
        {
            _isCrystalsConsumed = true;
            _crystal.SetActive(false);

            if (isInit) return;

        }
        else
        {
            _crystal.SetActive(true);
        }
    }

    public bool GetIfHasCrystal()
    {
        // print("I have cristal " + _isCrystalsConsumed);
        return !_isCrystalsConsumed;
    }

    public void ChangeCrystal(GameObject newCrystal)
    {
        if (newCrystal == null) return;

        if (_crystal == null) return;
        
        _crystal.SetActive(false);
        _crystal = newCrystal;
        
        if(!_isCrystalsConsumed)
            _crystal.SetActive(true);
    }
}