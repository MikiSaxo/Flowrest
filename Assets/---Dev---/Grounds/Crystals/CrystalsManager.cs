using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class CrystalsManager : MonoBehaviour
{
    public static CrystalsManager Instance;

    [Header("Setup")] [SerializeField] private Slider _energyBar;
    [SerializeField] private Slider _hitEnergyBar;

    [Header("Energy Earn")] [SerializeField]
    private float _earnedByGround;

    [SerializeField] private float _earnedByRecycling;

    [Header("Energy Lose")] [SerializeField]
    private float _lostBySwap;

    [SerializeField] private float _lostByLandingGround;

    private float _energyValue;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _energyValue = 1;
    }

    public void ReduceEnergyBySwap()
    {
        ReduceEnergy(_lostBySwap);
    }

    public void ReduceEnergyByLandingGround()
    {
        ReduceEnergy(_lostByLandingGround);
    }

    private void ReduceEnergy(float value)
    {
        value *= .01f;
        _energyValue -= value;
        
        if (_energyValue <= 0)
            _energyValue = 0;
        // print("reduce _energyValue :" + _energyValue);

        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        _energyBar.value = _energyValue;
        _hitEnergyBar.DOValue(_energyValue, .4f).SetDelay(.4f);
    }

    public void EarnEnergyByGround()
    {
        EarnEnergy(_earnedByGround);
    }

    public void EarnEnergyByRecycling()
    {
        EarnEnergy(_earnedByRecycling);
    }

    private void EarnEnergy(float value)
    {
        value *= .01f;
        _energyValue += value;
        
        if (_energyValue >= 1)
            _energyValue = 1;
        // print("earn _energyValue :" + _energyValue);

        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        _hitEnergyBar.DOValue(_energyValue, .4f);
        _energyBar.DOValue(_energyValue, .4f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            EarnEnergyByRecycling();
    }
}