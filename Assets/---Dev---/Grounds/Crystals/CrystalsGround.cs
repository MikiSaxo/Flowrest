using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalsGround : MonoBehaviour
{
    [SerializeField] private GameObject _crystals;

    private bool _isCrystalsConsumed;

    public void UpdateCrystals(bool state)
    {
        if (_isCrystalsConsumed) return;
        
        if (state == false)
        {
            _isCrystalsConsumed = true;
            _crystals.SetActive(false);
            CrystalsManager.Instance.EarnEnergyByGround();
        }
        else
            _crystals.SetActive(true);
    }
}
