using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Fx_BoredTile : MonoBehaviour
{
    [Header("Sphere")] [SerializeField] private MeshRenderer _sphere;
    [SerializeField] private float _timeDissolveOn;
    [SerializeField] private float _timeDissolveOff;
    [SerializeField] private float _timeBeforeDispawnBoudins;

    [Header("Boudins")] [SerializeField] private GameObject _boudinTop;
    [SerializeField] private GameObject _boudinDown;
    [SerializeField] private float _timeBoudinOn;
    [SerializeField] private float _timeBoudinOff;
    [SerializeField] private float _timeBetweenTwo;

    private float _startYPos;
    private bool _isBored;

    private void Start()
    {
        _boudinTop.transform.DOScale(0, 0);
        _boudinDown.transform.DOScale(0, 0);
        _sphere.material.SetFloat("_DissolveStep", 1.1f);

        _startYPos = _boudinTop.transform.position.y;
    }

    public void UpdateBored(bool state)
    {
        _isBored = state;
        if (state)
        {
            LaunchSpawnAnim();
        }
        else
        {
            if(!MapManager.Instance.IsVictory)
                LaunchDispawnAnim();
            else
                LaunchDispawnAnimQuick();
        }
    }

    private void LaunchSpawnAnim()
    {
        StopCoroutine(DispawnAnim());
        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        if (!_isBored) yield break;

        _sphere.material.DOKill();
        _boudinDown.transform.DOKill();
        _boudinTop.transform.DOKill();

        _sphere.material.DOFloat(.62f, "_DissolveStep", _timeDissolveOn).SetEase(Ease.InOutSine);
        _boudinDown.transform.DOScale(1, _timeBoudinOn).SetEase(Ease.InBack);
        yield return new WaitForSeconds(_timeBetweenTwo);
        
        if (!_isBored) yield break;

        _boudinTop.transform.DOScale(1, _timeBoudinOn).SetEase(Ease.InBack);
    }

    private void LaunchDispawnAnim()
    {
        StartCoroutine(DispawnAnim());
    }

    IEnumerator DispawnAnim()
    {
        if (_isBored) yield break;

        _sphere.material.DOKill();
        _boudinTop.transform.DOKill();
        _boudinDown.transform.DOKill();

        _sphere.material.DOFloat(1.1f, "_DissolveStep", _timeDissolveOff);

        yield return new WaitForSeconds(_timeBeforeDispawnBoudins);

        if (_isBored) yield break;
        _boudinTop.transform.DOScale(0, _timeBoudinOff).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(_timeBetweenTwo);

        if (_isBored) yield break;
        _boudinDown.transform.DOScale(0, _timeBoudinOff).SetEase(Ease.OutBack);
    }
    
    private void LaunchDispawnAnimQuick()
    {
        StartCoroutine(DispawnAnimQuick());
    }

    IEnumerator DispawnAnimQuick()
    {
        if (_isBored) yield break;

        _sphere.material.DOKill();
        _boudinTop.transform.DOKill();
        _boudinDown.transform.DOKill();

        _sphere.material.DOFloat(1.1f, "_DissolveStep", _timeDissolveOff/5);

        yield return new WaitForSeconds(_timeBeforeDispawnBoudins/5);

        if (_isBored) yield break;
        _boudinTop.transform.DOScale(0, _timeBoudinOff/5).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(_timeBetweenTwo/5);

        if (_isBored) yield break;
        _boudinDown.transform.DOScale(0, _timeBoudinOff/5).SetEase(Ease.OutBack);
    }
    
    public void LaunchDispawnInstant()
    {
        _sphere.material.DOKill();
        _boudinTop.transform.DOKill();
        _boudinDown.transform.DOKill();

        _sphere.material.DOFloat(1.1f, "_DissolveStep", 0);
        _boudinTop.transform.DOScale(0, 0);
        _boudinDown.transform.DOScale(0, 0);

        _sphere.material.DOComplete();
        _boudinTop.transform.DOComplete();
        _boudinDown.transform.DOComplete();
    }

    public void UpdateCanPoseTile(bool state)
    {
        _sphere.material.DOKill();
        _boudinTop.transform.DOKill();

        if (state)
        {
            if (!_isBored) return;
            
            _sphere.material.DOFloat(.78f, "_DissolveStep", _timeDissolveOff).SetEase(Ease.OutSine);
            _boudinTop.transform.DOMoveY(_startYPos - .5f, _timeDissolveOff);
        }
        else
        {
            if (!_isBored) return;
            
            _sphere.material.DOFloat(.62f, "_DissolveStep", _timeDissolveOff);
            _boudinTop.transform.DOMoveY(_startYPos, _timeDissolveOff);
        }
    }
}