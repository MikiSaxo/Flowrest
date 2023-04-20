using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class GroundPrevisu : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private Sprite _noSwap;
    [SerializeField] private float _timeWaitPreview;
    [SerializeField] private float _timeSpawnPreview;
    [SerializeField] private float _endScaleValue;

    private int _indexIcon;

    private void Start()
    {
        DeactivateIcon();
    }

    public void ActivateIcon(int index)
    {
        // _sprRnd.sprite = _iconTile[indexTile];
        // _sprRnd.sprite = SetupUIGround.Instance.GetGroundUIData(index).Icon;
        _sprRnd.color = SetupUIGround.Instance.GetGroundUIData(index).ColorIcon;
        _sprRnd.enabled = true;
        _indexIcon = index;

        _sprRnd.DOKill();
        // _sprRnd.transform.DOScale(0, 0);
        _sprRnd.DOFade(0, 0);
        _sprRnd.DOFade(0, _timeWaitPreview).OnComplete(() =>
        {
            _sprRnd.DOFade(1, _timeSpawnPreview).SetEase(Ease.InSine);
        });
    }

    public void DeactivateIcon()
    {
        _sprRnd.DOKill();
        _sprRnd.DOFade(0, 0).OnComplete(() => { _sprRnd.enabled = false; });
        // _sprRnd.transform.DOScale(0, .1f).OnComplete(() => { _sprRnd.enabled = false; });
        // _sprRnd.enabled = false;
    }

    public bool IsIconActivated()
    {
        return _sprRnd.enabled;
    }

    public int GetIndexActualIcon()
    {
        return _indexIcon;
    }

    public void UpdateSwap(bool state)
    {
        _sprRnd.enabled = state;
        // _sprRnd.sprite = _noSwap;
    }

    public Sprite GetIconTile(int index)
    {
        return SetupUIGround.Instance.GetGroundUIData(index).Icon;
    }
}