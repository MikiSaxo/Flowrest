using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Fx_BoredTile : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] private MeshRenderer _sphere;
    [SerializeField] private float _timeDissolveOn;
    [SerializeField] private float _timeDissolveOff;
    [SerializeField] private float _timeBeforeDispawnBoudins;
    
    [Header("Boudins")]
    [SerializeField] private GameObject _boudinTop;
    [SerializeField] private GameObject _boudinDown;
    [SerializeField] private float _timeBoudinOn;
    [SerializeField] private float _timeBoudinOff;
    [SerializeField] private float _timeBetweenTwo;

    private float _startYPos;
    private void Start()
    {
        _boudinTop.transform.DOScale(0, 0);
        _boudinDown.transform.DOScale(0, 0);
        _sphere.material.SetFloat("_DissolveStep", 1.1f);

        _startYPos = _boudinTop.transform.position.y;
    }

    public void UpdateBored(bool state)
    {
        if (state)
        {
            LaunchSpawnAnim();
        }
        else
        {
            LaunchDispawnAnim();
        }
    }

    private void LaunchSpawnAnim()
    {
        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        _sphere.material.DOFloat(.62f, "_DissolveStep", _timeDissolveOn).SetEase(Ease.InOutSine);
        _boudinDown.transform.DOScale(1, _timeBoudinOn).SetEase(Ease.InBack);
        yield return new WaitForSeconds(_timeBetweenTwo);
        _boudinTop.transform.DOScale(1, _timeBoudinOn).SetEase(Ease.InBack);
    }

    private void LaunchDispawnAnim()
    {
        StartCoroutine(DispawnAnim());
    }

    IEnumerator DispawnAnim()
    {
        _sphere.material.DOFloat(1.1f, "_DissolveStep", _timeDissolveOff);
        yield return new WaitForSeconds(_timeBeforeDispawnBoudins);
        _boudinTop.transform.DOScale(0, _timeBoudinOff).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(_timeBetweenTwo);
        _boudinDown.transform.DOScale(0, _timeBoudinOff).SetEase(Ease.OutBack);
    }

    public void UpdateCanPoseTile(bool state)
    {
        if (state)
        {
            _sphere.material.DOFloat(.78f, "_DissolveStep", _timeDissolveOff).SetEase(Ease.OutSine);
            _boudinTop.transform.DOMoveY(_startYPos - .5f, _timeDissolveOff);
        }
        else
        {
            _sphere.material.DOFloat(.62f, "_DissolveStep", _timeDissolveOff);
            _boudinTop.transform.DOMoveY(_startYPos, _timeDissolveOff);
        }
    }
}