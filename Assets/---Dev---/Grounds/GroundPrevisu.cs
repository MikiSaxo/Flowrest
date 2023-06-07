using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class GroundPrevisu : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprRnd;
    [SerializeField] private GameObject _previsuLight;
    [SerializeField] private float _timeWaitPreview;
    [SerializeField] private float _timeSpawnPreview;

    private Material _previsuLightMat;
    
    
    private int _indexIcon;

    private void Awake()
    {
        _previsuLightMat = _previsuLight.GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        DeactivateIcon();
    }

    public void ActivateIcon(int index)
    {
        var getColorIcon = SetupUIGround.Instance.GetGroundUIData(index).ColorIcon;
        _sprRnd.color = getColorIcon;
        _sprRnd.enabled = true;
        _previsuLight.SetActive(true);

        if (_previsuLightMat != null)
        {
            _previsuLightMat.SetColor("_BaseColor", getColorIcon);
        }

        _indexIcon = index;

        _sprRnd.DOKill();
        _sprRnd.DOFade(0, 0);
        _sprRnd.DOFade(0, _timeWaitPreview).OnComplete(() =>
        {
            _sprRnd.DOFade(1, _timeSpawnPreview).SetEase(Ease.InSine);
        });
        
        _previsuLightMat.DOKill();
        _previsuLightMat.DOFade(0, 0);
        _previsuLightMat.DOFade(0, _timeWaitPreview).OnComplete(() =>
        {
            _previsuLightMat.DOFade(1, _timeSpawnPreview).SetEase(Ease.InSine);
        });
    }

    public void DeactivateIcon()
    {
        _sprRnd.DOKill();
        _sprRnd.DOFade(0, 0).OnComplete(HideAllPreview);
    }

    private void HideAllPreview()
    {
        _sprRnd.enabled = false;
        _previsuLight.SetActive(false);
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
    }

    public Sprite GetIconTile(int index)
    {
        return SetupUIGround.Instance.GetGroundUIData(index).Icon;
    }
}