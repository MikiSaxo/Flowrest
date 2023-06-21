using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class RecyclingManager : MonoBehaviour
{
    public static RecyclingManager Instance;


    public bool HasInitTutoRecycling { get; set; }

    [SerializeField] private GameObject _recyclingParent;
    [SerializeField] private FB_Arrow _arrowTuto;
    [SerializeField] private TextMeshProUGUI _recyclingNbText;

    [Header("Anim Rotation")] [SerializeField]
    private GameObject _recyclingImg;

    [SerializeField] private Transform _minDistancePoint;
    [SerializeField] private float _minDistanceToClose = 2f;
    [SerializeField] private float _timeClose = .5f;
    [SerializeField] private float _timeOpen = 1f;

    private bool _hasInfinitRecycling;

    private int _maxRecycling;

    // private int _currentLeftRecycling;
    private bool _isSelected;
    private bool _isClosed;
    private const float _minRecyclingRotate = 0;
    private const float _maxRecyclingRotate = 60;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(false);
        _recyclingNbText.DOFade(0, 0);
    }

    public void UpdateRecycling(bool activateOrNot)
    {
        // _recyclingParent.SetActive(activateOrNot);
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(activateOrNot);

        _recyclingNbText.DOFade(activateOrNot ? 1 : 0, 0.25f);
    }

    public void InitNbRecycling(bool hasInfinit)
    {
        _hasInfinitRecycling = hasInfinit;
        HasInitTutoRecycling = false;
        // _maxRecycling = number;
        // _currentLeftRecycling = _maxRecycling;
        _recyclingNbText.gameObject.SetActive(!_hasInfinitRecycling);
        UpdateDisplayRecyclingNbLeft();
    }

    public void UpdateNbRecyclingLeft()
    {
        if (_hasInfinitRecycling) return;

        // _currentLeftRecycling--;

        if (MapManager.Instance.NbOfRecycling <= 0)
        {
            gameObject.GetComponent<PointerMotion>().UpdateCanEnter(false);
            // gameObject.GetComponentInChildren<Button>().interactable = false;
        }

        //GetComponent<PointerMotion>().Bounce();

        UpdateDisplayRecyclingNbLeft();
    }

    public void UpdateDisplayRecyclingNbLeft()
    {
        if (!_hasInfinitRecycling)
            _recyclingNbText.text = $"{MapManager.Instance.NbOfRecycling} {LanguageManager.Instance.GetRecycleText()}";

        UpdateVisualState();
        // _recyclingNbText.text = $"{_currentLeftRecycling}/{_maxRecycling}";
    }

    private void Update()
    {
        if (MapManager.Instance.WantToRecycle)
            return;

        if (MapManager.Instance.LastObjButtonSelected == null
            || MapManager.Instance.IsLoading
            || MapManager.Instance.IsPosing
            || MapManager.Instance.IsSwapping
            || MapManager.Instance.NbOfRecycling <= 0)
        {
            CloseRecycling();
            return;
        }

        var distance = _minDistancePoint.position.x - Input.mousePosition.x;
        var invertDistance = (1 / distance) * 2000;

        var angle = Mathf.Clamp(invertDistance, _minRecyclingRotate, _maxRecyclingRotate);

        if (invertDistance < 0)
            angle = _maxRecyclingRotate;
        if (invertDistance < _minDistanceToClose && invertDistance > 0f)
            angle = _minRecyclingRotate;

        _recyclingImg.transform.DORotate(new Vector3(0, 0, -angle), 0);
    }

    public void ActivateButton()
    {
        if (DialogManager.Instance.GetIsDialogTime()) return;

        _isSelected = true;
        OnEnter();
    }

    public void OnEnter()
    {
        if (MapManager.Instance.NbOfRecycling > 0)
            GetComponentInChildren<Image>().color = Color.yellow;
    }

    public void OnExit()
    {
        if (_isSelected) return;

        UpdateVisualState();
    }

    public void DeselectRecycle()
    {
        CloseRecycling();
        MapManager.Instance.ResetWantToRecycle();

        _isSelected = false;
        OnExit();
    }

    private void CloseRecycling()
    {
        if (_isClosed) return;

        _isClosed = true;
        _recyclingImg.transform.DOKill();
        _recyclingImg.transform.DORotate(Vector3.zero, _timeClose).SetEase(Ease.OutBounce).OnComplete(ResetClosing);
    }

    private void ResetClosing()
    {
        _isClosed = false;
    }

    public void OpenRecycling()
    {
        _isClosed = false;
        _recyclingImg.transform.DOKill();
        _recyclingImg.transform.DORotate(new Vector3(0, 0, -_maxRecyclingRotate), _timeOpen).SetEase(Ease.OutBounce);
    }

    private void UpdateVisualState()
    {
        if (MapManager.Instance.NbOfRecycling > 0)
        {
            GetComponentInChildren<Image>().color = Color.white;
            GetComponent<PointerMotion>().UpdateCanEnter(true);
        }
        else
        {
            Color newAlpha = Color.white;
            newAlpha.a = .5f;
            _recyclingImg.GetComponent<Image>().color = newAlpha;
            GetComponent<PointerMotion>().UpdateCanEnter(false);
        }
    }

    public void UpdateArrowTuto(bool state)
    {
        _arrowTuto.UpdateArrow(state);
    }
}