using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Setup")] [SerializeField] private int _totalNumbersOfLevels;
    [SerializeField] private int _numberOfLevelsPerPage;
    [Header("Prefabs")] [SerializeField] private GameObject _gridLevelSelectionPrefab;
    [SerializeField] private GameObject _levelSelectionPrefab;

    [Header("Page")] [SerializeField] private GameObject _gridPage;
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private Sprite[] _sprPage;

    [Header("Arrow Buttons")] [SerializeField]
    private GameObject _leftArrowButton;

    [SerializeField] private GameObject _rightArrowButton;


    private GameObject _currentGridToCreateLevel;
    private List<GameObject> _gridList = new List<GameObject>();

    private List<GameObject> _stockPagePrefab = new List<GameObject>();
    private int _count;

    private void Start()
    {
        for (int i = 0; i < _totalNumbersOfLevels; i++)
        {
            if (i == 0 || i % _numberOfLevelsPerPage == 0)
            {
                GameObject grid = Instantiate(_gridLevelSelectionPrefab, transform);
                _currentGridToCreateLevel = grid;
                _gridList.Add(grid);
            }

            GameObject go = Instantiate(_levelSelectionPrefab, _currentGridToCreateLevel.transform);
            go.GetComponent<LevelButton>().Init(i + 1, i > BigManager.Instance.LevelUnlocked);
        }

        UpdateLegend();
    }

    private void UpdateLegend()
    {
        for (int i = 0; i < _gridList.Count; i++)
        {
            GameObject go = Instantiate(_pagePrefab, _gridPage.transform);
            _stockPagePrefab.Add(go);
        }

        _count = 0;

        if (_gridList.Count <= 1) return;

        for (int i = 1; i < _gridList.Count; i++)
        {
            _gridList[i].SetActive(false);
        }

        // Display First Page
        _stockPagePrefab[0].GetComponent<Image>().sprite = _sprPage[_count];
        // Deactivate Left Arrow
        UpdateStateLeftArrow(false);
    }

    public void MoveToLeft()
    {
        // Reset Right Arrow
        UpdateStateRightArrow(true);

        // Change old page to empty
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];
        // Deactivate current displayed grid
        _gridList[_count].SetActive(false);

        // Remove to count
        _count--;

        // Check if next move it's good for this arrow
        if (_count <= 0)
            UpdateStateLeftArrow(false);
        else
            UpdateStateLeftArrow(true);

        // Activate new grid
        _gridList[_count].SetActive(true);
        // Full fill new page
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];
    }

    public void MoveToRight()
    {
        // Reset Left Arrow
        UpdateStateLeftArrow(true);

        // Change old page to empty
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];
        // Deactivate current displayed grid
        _gridList[_count].SetActive(false);

        // Add to count
        _count++;
            
        // Check if next move it's good for this arrow
        if (_count >= _gridList.Count - 1)
            UpdateStateRightArrow(false);
        else
            UpdateStateRightArrow(true);

        // Activate new grid
        _gridList[_count].SetActive(true);
        // Full fill new page
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];
    }

    private void UpdateStateLeftArrow(bool state)
    {
        _leftArrowButton.GetComponent<Button>().interactable = state;
        _leftArrowButton.GetComponent<PointerMotion>().UpdateCanEnter(state);
    }
    
    private void UpdateStateRightArrow(bool state)
    {
        _rightArrowButton.GetComponent<Button>().interactable = state;
        _rightArrowButton.GetComponent<PointerMotion>().UpdateCanEnter(state);
    }
}