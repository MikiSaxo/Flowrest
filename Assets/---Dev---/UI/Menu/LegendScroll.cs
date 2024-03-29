using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    [SerializeField] private Sprite[] _sprPage;

    [Header("Arrows")] [SerializeField] private GameObject _leftArrowButton;
    [SerializeField] private GameObject _rightArrowButton;

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
        ResetPopUp();

        if (popUpInfos.Length > 1)
        {
            UpdateLegend();
        }
        else
        {
            _gridPage.SetActive(false);
            if (_leftArrowButton != null && _rightArrowButton != null)
            {
                _leftArrowButton.SetActive(false);
                _rightArrowButton.SetActive(false);
            }

            _count = 0;

            CheckIfEndOfLegend();
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

        if (_leftArrowButton != null && _rightArrowButton != null)
        {
            _leftArrowButton.SetActive(true);
            _rightArrowButton.SetActive(true);
        }

        // Fill first page
        _stockPagePrefab[0].GetComponent<Image>().sprite = _sprPage[_count];
        // Deactivate Left Arrow 
        UpdateStateLeftArrow(false);

        CheckIfEndOfLegend();
    }

    public void MoveToLeft()
    {
        // Change the page indicator to empty 
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];
        UpdateStateRightArrow(true);
        _count--;

        if (!_isVideoLegend)
        {
            if (_count <= 0)
            {
                UpdateStateLeftArrow(false);
                _count = 0;
            }
            else
                UpdateStateLeftArrow(true);

            _imgLegend.sprite = _sprLegend[_count];
        }
        else
        {
            if (_count <= 0)
            {
                UpdateStateLeftArrow(false);
                _count = 0;
            }
            else
                UpdateStateLeftArrow(true);

            UpdatePopUpText();
        }

        // Fill the new page indicator
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }

    public void MoveToRight()
    {
        // Change the page indicator to empty 
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];
        UpdateStateLeftArrow(true);
        _count++;

        if (!_isVideoLegend)
        {
            if (_count >= _sprLegend.Length - 1)
            {
                UpdateStateRightArrow(false);
                _count = _sprLegend.Length - 1;
            }
            else
                UpdateStateLeftArrow(true);

            _imgLegend.sprite = _sprLegend[_count];
        }
        else
        {
            if (_count >= _popUpInfos.Length - 1)
            {
                UpdateStateRightArrow(false);
                _count = _popUpInfos.Length - 1;
            }
            else
                UpdateStateLeftArrow(true);

            UpdatePopUpText();
        }

        // Fill the new page indicator
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }

    public void GoToPageOne()
    {
        // Change the page indicator to empty 
        if (_stockPagePrefab.Count <= 1) return;

        foreach (var page in _stockPagePrefab)
        {
            page.GetComponent<Image>().sprite = _sprPage[1];
        }

        _stockPagePrefab[0].GetComponent<Image>().sprite = _sprPage[0];

        _count = 0;

        UpdateStateLeftArrow(false);
        UpdateStateRightArrow(true);
        UpdatePopUpText();
    }

    private void CheckIfEndOfLegend()
    {
        if (_skipPopUpButton == null) return;

        if (!_isVideoLegend)
            _skipPopUpButton.SetActive(_count == _sprLegend.Length - 1);
        else
            _skipPopUpButton.SetActive(_count == _popUpInfos.Length - 1);
    }

    private void UpdateStateLeftArrow(bool state)
    {
        if (_leftArrowButton == null) return;

        _leftArrowButton.GetComponent<Button>().interactable = state;
        _leftArrowButton.GetComponent<PointerMotion>().UpdateCanEnter(state);
    }

    private void UpdateStateRightArrow(bool state)
    {
        if (_rightArrowButton == null) return;

        _rightArrowButton.GetComponent<Button>().interactable = state;
        _rightArrowButton.GetComponent<PointerMotion>().UpdateCanEnter(state);
    }

    private void ResetPopUp()
    {
        foreach (var page in _stockPagePrefab)
        {
            Destroy(page);
        }

        _stockPagePrefab.Clear();

        UpdateStateRightArrow(true);
    }

    private void UpdatePopUpText()
    {
        if (LanguageManager.Instance.Tongue == Language.Francais)
            GetComponent<PopUpManager>().UpdatePopUp(_popUpInfos[_count].Title, _popUpInfos[_count].ImgPopUp,
                _popUpInfos[_count].Description);
        else
            GetComponent<PopUpManager>().UpdatePopUp(_popUpInfos[_count].TitleEnglish, _popUpInfos[_count].ImgPopUp,
                _popUpInfos[_count].DescriptionEnglish);
    }
}