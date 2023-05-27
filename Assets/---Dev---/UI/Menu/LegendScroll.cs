using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendScroll : MonoBehaviour
{
    [Header("Legend Setup")]
    [SerializeField] private Image _imgLegend;
    [SerializeField] private Sprite[] _sprLegend;
    [Tooltip("Only if needed to depop")] [SerializeField] private GameObject _skipPopUpButton;
    [Header("Page")]
    [SerializeField] private GameObject _gridPage;
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private Sprite[] _sprPage;

    private List<GameObject> _stockPagePrefab = new List<GameObject>();
    private int _count;

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
        UpdateLegend();
    }
    
    private void UpdateLegend()
    {
        for (int i = 0; i < _sprLegend.Length; i++)
        {
            GameObject go = Instantiate(_pagePrefab, _gridPage.transform);
            _stockPagePrefab.Add(go);
        }
        
        _count = 0;
        _imgLegend.sprite = _sprLegend[_count];
        _stockPagePrefab[0].GetComponent<Image>().sprite = _sprPage[_count];
        
        CheckIfEndOfLegend();
    }

    public void MoveToLeft()
    {
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];
        
        _count--;

        if (_count < 0)
            _count = _sprLegend.Length - 1;
        
        _imgLegend.sprite = _sprLegend[_count];
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }
    
    public void MoveToRight()
    {
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[1];

        _count++;

        if (_count >= _sprLegend.Length)
            _count = 0;
        
        _imgLegend.sprite = _sprLegend[_count];
        _stockPagePrefab[_count].GetComponent<Image>().sprite = _sprPage[0];

        CheckIfEndOfLegend();
    }

    private void CheckIfEndOfLegend()
    {
        if(_skipPopUpButton == null) return;

        _skipPopUpButton.SetActive(_count == _sprLegend.Length - 1);
    }
}
