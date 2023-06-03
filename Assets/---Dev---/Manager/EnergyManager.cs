using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance;


    [Header("Setup")] [SerializeField] private Slider _energyBar;
    [SerializeField] private Slider _hitEnergyBar;
    [SerializeField] private Image _energyBarImg;
    [SerializeField] private Image _hitEnergyBarImg;
    [SerializeField] private TextMeshProUGUI _numberToDisplay;
    [SerializeField] private WaveEffect _waveEffect;
    [SerializeField] private Image _vignettage;
    [SerializeField] private GameObject _maskParent;
    [SerializeField] private GameObject _fbNoEnergy;
    [SerializeField] private float _timeToFillEnergy;

    // [Header("Energy Base")]
    // [SerializeField] private int _howBase;

    [Header("Energy Earn")] [SerializeField]
    private int _earnedByCrystal;

    [SerializeField] private int _earnedByRecycling;

    [Header("Energy Cost")] [SerializeField]
    private int _costBySwap;

    [SerializeField] private int _costByLandingGround;

    [Header("Timing")] [SerializeField] private float _timeInitAnim;
    [SerializeField] private float _timeVignettage;

    private int _energyValue;
    private int _currentEnergy;
    private int _tempValue;
    private int _maxEnergy;
    private float _timerSpawnFBCrystal;
    private GameObject _currentFbNoEnergy;
    private bool _isInit;

    private void Awake()
    {
        Instance = this;

        //_baseInf = 1 / (float)_howBase;
    }

    public void InitEnergy(int startEnergy, int maxEnergy)
    {
        _energyValue = startEnergy;
        //_energyBar.value = 0;
        _energyBarImg.fillAmount = 0;
        // _hitEnergyBar.value = 0;
        _hitEnergyBarImg.fillAmount = 0;
        
        _numberToDisplay.text = $"0";
        _currentEnergy = _energyValue;
        _maxEnergy = maxEnergy;


        _numberToDisplay.color = _energyValue == 0 ? Color.red : Color.white;

        if (_energyValue == 0)
        {
            _waveEffect.StartGrowOnAlways();
        }
    }

    private void AnimEnergyBar()
    {
        float energyDisplay = (float)_currentEnergy / (float)_maxEnergy;
        // _energyBar.DOValue(energyDisplay, _timeToFillEnergy).SetEase(Ease.Linear);
        _energyBarImg.DOFillAmount(energyDisplay, _timeToFillEnergy).SetEase(Ease.Linear);
        // _hitEnergyBar.DOValue(energyDisplay, _timeToFillEnergy).SetEase(Ease.Linear);
        _hitEnergyBarImg.DOFillAmount(energyDisplay, _timeToFillEnergy).SetEase(Ease.Linear);
    }

    public void LaunchAnimEnergy()
    {
        StartCoroutine(AnimInitEnergy(_currentEnergy));
    }

    IEnumerator AnimInitEnergy(int energy)
    {
        _isInit = true;
        // yield return new WaitForSeconds(_timeToFillEnergy);

        if (energy > 0)
        {
            AnimEnergyBar();

            for (int i = 1; i <= energy; i++)
            {
                yield return new WaitForSeconds(_timeToFillEnergy / energy);
                _numberToDisplay.text = $"{i}";
            }

            BounceEnergy();
        }

        _numberToDisplay.text = $"{_currentEnergy}";
        _isInit = false;
    }

    public void ReduceEnergyBySwap()
    {
        StartCoroutine(WaitToUpdate(-_costBySwap));
        AudioManager.Instance.PlaySFX("EnergyLost");
    }

    public void ReduceEnergyByLandingGround()
    {
        StartCoroutine(WaitToUpdate(-_costByLandingGround));
        AudioManager.Instance.PlaySFX("EnergyLost");
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
        if (_isInit)
        {
            StopCoroutine(AnimInitEnergy(_currentEnergy));
            _isInit = false;
        }
        
        _tempValue += value;
        if (_tempValue == 0)
            _tempValue = 1;

        if (value > 0)
        {
            //ItemCollectedManager.Instance.SpawnFBEnergyCollected(_tempValue);
        }

        yield return new WaitForSeconds(.01f);

        UpdateEnergy(_tempValue);
        _tempValue = 0;
    }

    public void UpdateEnergy(int value)
    {
        if (value == 0) return;

        _energyValue += value;

        // _energyBar.DOKill();
        _energyBarImg.DOKill();
        // _hitEnergyBar.DOKill();
        _hitEnergyBarImg.DOKill();

        if (value < 0)
        {
            if (_energyValue <= 0)
            {
                _waveEffect.StartGrowOnAlways();
                _energyValue = 0;
                
                if (MapManager.Instance.IsTutoRecycling && !RecyclingManager.Instance.HasInitTutoRecycling)
                {
                    ScreensManager.Instance.UpdateTutoArrow(true);
                }
            }
            else
            {
                _waveEffect.StartGrowOneTime();
            }

            float energyDisplay = (float)_energyValue / (float)_maxEnergy;

            // _energyBar.value = energyDisplay;
            _energyBarImg.fillAmount = energyDisplay;
            // _hitEnergyBar.DOValue(energyDisplay, .4f).SetDelay(.4f);
            _hitEnergyBarImg.DOFillAmount(energyDisplay, .4f).SetDelay(.4f);
        }
        else
        {
            BounceEnergy();

            _vignettage.DOFade(1, _timeVignettage).OnComplete(() => { _vignettage.DOFade(0, _timeVignettage); });

            StopWaveEffect();

            float energyDisplay = (float)_energyValue / (float)_maxEnergy;
            // _energyBar.DOValue(energyDisplay, .4f);
            _energyBarImg.DOFillAmount(energyDisplay, .4f);
            // _hitEnergyBar.DOValue(energyDisplay, .4f);
            _hitEnergyBarImg.DOFillAmount(energyDisplay, .4f);
        }

        // // Bad system to avoid 499 or 501 but 500 
        // float round = Mathf.Round(_energyValue * _howBase);
        // int number = int.Parse(round + "");
        _numberToDisplay.text = $"{_energyValue}";
        _currentEnergy = _energyValue;

        _numberToDisplay.color = _energyValue == 0 ? Color.red : Color.white;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F))
        //     ReduceEnergyBySwap();
        // if (Input.GetKeyDown(KeyCode.R))
        //     EarnEnergyByCrystal();
    }

    private void BounceEnergy()
    {
        _maskParent.transform.DOScale(1, 0);
        _maskParent.transform.DOPunchScale(Vector3.one * .2f, 1f, 4);
    }

    public void SpawnNoEnergyText()
    {
        if (_currentFbNoEnergy != null || MapManager.Instance.IsOnUI) return;

        GameObject go = Instantiate(_fbNoEnergy, transform);
        go.GetComponent<TextWarning>().Init(LanguageManager.Instance.GetNoEnergyText());
        _currentFbNoEnergy = go;
    }

    public void StopWaveEffect()
    {
        _waveEffect.StopGrownOn();
        _numberToDisplay.color = Color.white;
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

    public void ResetEnergy()
    {
        _numberToDisplay.text = $"0";
        _currentEnergy = 0;
        _numberToDisplay.color = Color.white;
        // _energyBar.DOValue(0, 0);
        _energyBarImg.DOFillAmount(0, 0);
        // _hitEnergyBar.DOValue(0, 0);
        _hitEnergyBarImg.DOFillAmount(0, 0);
    }
}