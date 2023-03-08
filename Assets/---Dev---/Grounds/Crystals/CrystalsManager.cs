using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;

public class CrystalsManager : MonoBehaviour
{
    public static CrystalsManager Instance;

    [Header("Setup")][SerializeField] private Slider _energyBar;
    [SerializeField] private Slider _hitEnergyBar;
    [SerializeField] private TextMeshProUGUI _numberToDisplay;

    [Header("Energy Base")]
    [Tooltip("If max energy = 1000, put 1000 - If 100, put 100")] [SerializeField] private int _howBase;
    
    [Header("Energy Earn")]
    [SerializeField] private float _earnedByGround;

    [SerializeField] private float _earnedByRecycling;

    [Header("Energy Lose")]
    [SerializeField] private float _lostBySwap;

    [SerializeField] private float _lostByLandingGround;

    private float _energyValue;
    private float _baseInf;

    private void Awake()
    {
        Instance = this;
        
        _baseInf = 1 / (float)_howBase;
    }

    public void InitEnergy(int startEnergy)
    {
        _energyValue = startEnergy * _baseInf;
        _energyBar.value = _energyValue;
        _hitEnergyBar.value = _energyValue;
        _numberToDisplay.text = $"{startEnergy}";
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
        value *= _baseInf;
        _energyValue -= value;

        if (_energyValue <= 0)
            _energyValue = 0;
        // print("reduce _energyValue :" + _energyValue);

        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        _energyBar.value = _energyValue;
        _hitEnergyBar.DOValue(_energyValue, .4f).SetDelay(.4f);
        float number = (_energyValue * _howBase);
        _numberToDisplay.text = $"{(int)number}";
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
        value *= _baseInf;
        _energyValue += value;

        if (_energyValue >= 1)
            _energyValue = 1;
        // print("earn _energyValue :" + _energyValue);

        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        _hitEnergyBar.DOValue(_energyValue, .4f);
        _energyBar.DOValue(_energyValue, .4f);
        float number = _energyValue * _howBase;
        _numberToDisplay.text = $"{(int)number}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            EarnEnergyByRecycling();
    }
}