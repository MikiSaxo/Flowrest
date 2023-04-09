using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsGround : MonoBehaviour
{
    [SerializeField] private GameObject _crystals;

    private bool _isCrystalsConsumed;

    public void UpdateCrystals(bool state, bool isInit)
    {
        if (_isCrystalsConsumed) return;
        
        if (state == false)
        {
            _isCrystalsConsumed = true;
            _crystals.SetActive(false);
            
            if (isInit) return;
            
            EnergyManager.Instance.EarnEnergyByCrystal();
        }
        else
            _crystals.SetActive(true);
    }
}
