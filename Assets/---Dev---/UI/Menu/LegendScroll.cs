using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendScroll : MonoBehaviour
{
    [Header("Legend Setup")] [SerializeField]
    private Image _imgLegend;

    [SerializeField] private Sprite[] _sprLegend;

    [Tooltip("Only if needed to depop")] [SerializeField]
    private GameObject _skipPopUpButton;

    [Header("Page")] [SerializeField] private GameObject _gridPage;
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private GameObject _arrows;
    [SerializeField] private Sprite[] _sprPage;

    private List<GameObject> _stockPagePrefab = new List<GameObject>();
    private int _count;
    private PopUpInfos[] _popUpInfos;
    private bool _isVideoLegend;

    private void Awake()
    {
        if (_sprLegend.Length > 0)
        {
            UpdateLegend();
        }
    }

    public void InitLegend(Sprite[] allSprites)
    {
        _sprLegend = allSprites;
        _isVideoLegend = false;
        UpdateLegend();
    }

    public void InitVideoLegend(PopUpInfos[] popUpInfos)
    {
        _popUpInfos = popUpInfos;
        _isVideoLegend = true;

        if (popUpInfos.Length > 1)
        {
            UpdateLegend();
        }
        else
        {
            _gridPage.SetActive(false);
            if(_arrows != null)
                _arrows.SetActive(false);
        }
    }

    private void UpdateLegend()
    {
        _count = 0;
        
        if (!_isVideoLegend)
        {
            for (int i = 0; i < _sprLegend.Length; i++)
            {
                GameObject go = Instantiate(_pagePrefab, _gridPage.transform);
                _stockPagePrefab.Add(go);
            }

            _imgLegend.sprite = _sprLegend[_count];
        }
        else
        {
            for (int i = 0; i < _popUpInfos.Length; i++)
            {
                GameObject go = Instantiate(_pagePrefab, _gridPage.transform);
                _stockPagePrefab.Add(go);
            }
        }
        
        _gridPage.SetActive(true);
        if(_arrows != null)
            _arrows.SetActive(true);

        // Fill first page
        _stockPagePrefab[0].GetComponent<Image>().sprite = _sprPage[_count];

        CheckIfEndOfLegend();
    }

    public void MoveToLeft()
    {
        // Change the page indicator to empty 
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];

        _count--;

        if (!_isVideoLegend)
        {
            if (_count < 0)
            {
                _count = 0;
            }
                // _count = _sprLegend.Length - 1;

            _imgLegend.sprite = _sprLegend[_count];
        }
        else
        {
            if (_count < 0)
            {
                _count = 0;
            }
            //_count = _popUpInfos.Length - 1;

            GetComponent<PopUpManager>().UpdatePopUp(_popUpInfos[_count].Title, _popUpInfos[_count].VideoName,
                _popUpInfos[_count].Description);
        }

        // Fill the new page indicator
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }

    public void MoveToRight()
    {
        // Change the page indicator to empty 
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];

        _count++;

        if (!_isVideoLegend)
        {
            if (_count >= _sprLegend.Length)
                _count = _sprLegend.Length - 1;
                // _count = 0;

            _imgLegend.sprite = _sprLegend[_count];
        }
        else
        {
            if (_count >= _popUpInfos.Length)
                _count = _sprLegend.Length - 1;
                // _count = 0;

            GetComponent<PopUpManager>().UpdatePopUp(_popUpInfos[_count].Title, _popUpInfos[_count].VideoName,
                _popUpInfos[_count].Description);
        }

        // Fill the new page indicator
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }

    private void CheckIfEndOfLegend()
    {
        if (_skipPopUpButton == null) return;

        if (!_isVideoLegend)
            _skipPopUpButton.SetActive(_count == _sprLegend.Length - 1);
        else
            _skipPopUpButton.SetActive(_count == _popUpInfos.Length - 1);
    }
}