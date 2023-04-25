using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderStockSprite : MonoBehaviour
{
    [SerializeField] private Image _img;
    // [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _checkMark;
    
    [Header("Orders")]
    [SerializeField] private List<OrderSprite> _orderSprites;
    
    // private int _maxNb;
    // private int _currentNb;
    private int _currentOrder;
    private AllStates _currentStateOrder;
    
    public void Init(int whichOrder, AllStates whichState)
    {
        _currentOrder = whichOrder;
        _currentStateOrder = whichState;
        // _maxNb = nbToReach;

        _img.sprite = _orderSprites[_currentOrder].OrderSprites[(int)_currentStateOrder];
        // _text.text = $"{_currentNb} / {_maxNb}";
    }
}
