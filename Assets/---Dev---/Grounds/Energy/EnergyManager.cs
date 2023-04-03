using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;

    [Header("Setup")][SerializeField] private Slider _energyBar;
    [SerializeField] private Slider _hitEnergyBar;
    [SerializeField] private TextMeshProUGUI _numberToDisplay;

    [Header("Energy Base")]
    [Tooltip("If max energy = 1000, put 1000 - If 100, put 100")] [SerializeField] private int _howBase;
    
    [Header("Energy Earn")]
    [SerializeField] private float _earnedByGround;
    [SerializeField] private float _earnedByRecycling;

    [Header("Energy Cost")]
    [SerializeField] private float _costBySwap;
    [SerializeField] private float _costByLandingGround;

    private float _energyValue;
    private float _baseInf;
    private int _currentEnergy;
    private float _tempValue;
    private float _timerSpawnFBCrystal;

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
        _currentEnergy = startEnergy;
    }

    public void ReduceEnergyBySwap()
    {
        StartCoroutine(WaitToUpdate(-_costBySwap));
    }

    public void ReduceEnergyByLandingGround()
    {
        StartCoroutine(WaitToUpdate(-_costByLandingGround));
    }

    public void EarnEnergyByGround()
    {
        StartCoroutine(WaitToUpdate(_earnedByGround));
    }

    public void EarnEnergyByRecycling()
    {
        StartCoroutine(WaitToUpdate(_earnedByRecycling));
    }
    IEnumerator WaitToUpdate(float value)
    {
        _tempValue += value;
        yield return new WaitForSeconds(.01f);
        ItemCollectedManager.Instance.SpawnFBEnergyCollected((int)_tempValue);
        UpdateEnergy(_tempValue);
        _tempValue = 0;
    }
    private void UpdateEnergy(float value)
    {
        value *= _baseInf;
        _energyValue += value;
        
        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        if (value < 0)
        {
            if (_energyValue <= 0)
                _energyValue = 0;
            
            _energyBar.value = _energyValue;
            _hitEnergyBar.DOValue(_energyValue, .4f).SetDelay(.4f);
        }
        else
        {
            if (_energyValue >= 1)
                _energyValue = 1;
            
            _hitEnergyBar.DOValue(_energyValue, .4f);
            _energyBar.DOValue(_energyValue, .4f);
        }
        
        // Bad system to avoid 499 or 501 but 500 
        float round = Mathf.Round(_energyValue * _howBase);
        int number = int.Parse(round + "");
        _numberToDisplay.text = $"{number}";
        _currentEnergy = number;

        // MapManager.Instance.CheckIfGameOver();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ReduceEnergyBySwap();
        if (Input.GetKeyDown(KeyCode.R)) 
            EarnEnergyByGround();
    }

    public bool IsEnergyInferiorToCostSwap()
    {
        return _currentEnergy < _costBySwap;
    }

    public bool IsEnergyInferiorToCostLandingGround()
    {
        return _currentEnergy < _costByLandingGround;
    }
}