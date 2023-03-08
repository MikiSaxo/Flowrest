using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FollowMouseDND : MonoBehaviour
{
    public static FollowMouseDND Instance;

    public bool CanMove;
    [SerializeField] private Image _iconButton;
    [SerializeField] private TextMeshProUGUI _text;


    private void Awake()
    {
        Instance = this;
    }

    private void Move()
    {
        var mousePos = Input.mousePosition;
        gameObject.transform.position = mousePos;
    }

    private void Update()
    {
        if (CanMove)
            Move();
    }

    public void UpdateObject(Sprite sprite, string text)
    {
        _iconButton.sprite = sprite;
        _text.text = text;
    }

    public void AnimDeactivateObject()
    {
        SetupUIGround.Instance.GroundStockage.ForcedOpen = false;
        _iconButton.transform.DOScale(Vector2.zero, .15f).SetEase(Ease.Linear).OnComplete(DeactivateObject);
        _text.transform.DOScale(Vector2.zero, .15f).SetEase(Ease.Linear);
    }

    private void DeactivateObject()
    {
        _iconButton.transform.DOScale(Vector2.one, 0);
        _text.transform.DOScale(Vector2.one, 0);
        gameObject.SetActive(false);
    }
}