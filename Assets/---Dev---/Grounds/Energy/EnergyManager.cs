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

    [Header("Setup")] [SerializeField] private Slider _energyBar;
    [SerializeField] private Slider _hitEnergyBar;
    [SerializeField] private TextMeshProUGUI _numberToDisplay;
    [SerializeField] private WaveEffect _waveEffect;
    [SerializeField] private Image _vignettage;
    [SerializeField] private GameObject _maskParent;

    // [Header("Energy Base")]
    // [SerializeField] private int _howBase;

    [Header("Energy Earn")] [SerializeField]
    private int _earnedByCrystal;

    [SerializeField] private int _earnedByRecycling;

    [Header("Energy Cost")] [SerializeField]
    private int _costBySwap;

    [SerializeField] private int _costByLandingGround;

    [Header("Timing")] [SerializeField] private float _timeInitAnim;
    [Header("Timing")] [SerializeField] private float _timeVignettage;

    private int _energyValue;
    private int _currentEnergy;
    private int _tempValue;
    private float _timerSpawnFBCrystal;
    private bool _isInit;
    private float _lerpTiming;

        private void Awake()
    {
        Instance = this;

        //_baseInf = 1 / (float)_howBase;
    }

    public void InitEnergy(int startEnergy)
    {
        _energyValue = startEnergy;
        StartCoroutine(AnimInitEnergy(startEnergy));
        _energyBar.value = 0;
        _hitEnergyBar.value = 0;
        _numberToDisplay.text = $"{0}";
        _currentEnergy = _energyValue;
        _lerpTiming = 0;

        if (startEnergy == 0)
            _waveEffect.StartGrowOnAlways();
    }

    IEnumerator AnimInitEnergy(int energy)
    {
        for (int i = 0; i <= energy; i++)
        {
            yield return new WaitForSeconds(_timeInitAnim);
            _isInit = true;
            _numberToDisplay.text = $"{i}";
        }

        _isInit = false;
        BounceEnergy();
    }

    public void ReduceEnergyBySwap()
    {
        StartCoroutine(WaitToUpdate(-_costBySwap));
    }

    public void ReduceEnergyByLandingGround()
    {
        StartCoroutine(WaitToUpdate(-_costByLandingGround));
    }

    public void EarnEnergyByCrystal()
    {
        StartCoroutine(WaitToUpdate(_earnedByCrystal));
    }

    public void EarnEnergyByRecycling()
    {
        StartCoroutine(WaitToUpdate(_earnedByRecycling));
    }

    IEnumerator WaitToUpdate(int value)
    {
        _tempValue += value;
        if (_tempValue == 0)
            _tempValue = 1;
        yield return new WaitForSeconds(.01f);
        ItemCollectedManager.Instance.SpawnFBEnergyCollected(_tempValue);
        UpdateEnergy(_tempValue);
        _tempValue = 0;
    }

    public void UpdateEnergy(int value)
    {
        // value *= _baseInf;
        _energyValue += value;

        _energyBar.DOKill();
        _hitEnergyBar.DOKill();

        if (value < 0)
        {
            if (_energyValue <= 0)
            {
                _waveEffect.StartGrowOnAlways();
                _energyValue = 0;
            }
            else
            {
                _waveEffect.StartGrowOneTime();
            }

            _energyBar.value = _energyValue;
            _hitEnergyBar.DOValue(_energyValue, .4f).SetDelay(.4f);
        }
        else
        {
            // var energyValue = 1;
            //
            // if (_energyValue < 1)
            //     energyValue = 0;
            //
            BounceEnergy();

            _vignettage.DOFade(1, _timeVignettage).OnComplete(() => { _vignettage.DOFade(0, _timeVignettage);});

            StopWaveEffect();
            _hitEnergyBar.DOValue(1, .4f);
            _energyBar.DOValue(1, .4f);
        }

        // // Bad system to avoid 499 or 501 but 500 
        // float round = Mathf.Round(_energyValue * _howBase);
        // int number = int.Parse(round + "");
        _numberToDisplay.text = $"{_energyValue}";
        _currentEnergy = _energyValue;

        // MapManager.Instance.CheckIfGameOver();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ReduceEnergyBySwap();
        if (Input.GetKeyDown(KeyCode.R))
            EarnEnergyByCrystal();

        if (_isInit)
        {
            _lerpTiming += Time.deltaTime / (_currentEnergy * _timeInitAnim);
            
            float newValue = Mathf.Lerp(0, 1, _lerpTiming);
            
            _energyBar.value = newValue;
            _hitEnergyBar.value = newValue;
        }
    }

    private void BounceEnergy()
    {
        _maskParent.transform.DOPunchScale(Vector3.one * .2f, 1f, 4);
    }

    public void StopWaveEffect()
    {
        print("stop it");
        _waveEffect.StopGrownOn();
    }

    public bool IsEnergyInferiorToCostSwap()
    {
        return _currentEnergy < _costBySwap;
    }

    public bool IsEnergyInferiorToCostLandingGround()
    {
        return _currentEnergy < _costByLandingGround;
    }

    public int GetCurrentEnergy()
    {
        return _currentEnergy;
    }
}