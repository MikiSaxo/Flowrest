using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FB_CrystalCollected : MonoBehaviour
{
    [Header("Setup")] [SerializeField] private GameObject _objToMove;
    // [SerializeField] private TMP_Text _text;
    [SerializeField] private float _yOffset;
    
    [Header("Durations")] [SerializeField] private float _durationSpawn;
    [SerializeField] private float _durationWait;
    [SerializeField] private float _durationDispawn;
    [SerializeField] private float _durationWaitToDelete;
    
    
    private Transform _tpEndPoint;
    private int _value;

    public void Init(int value, Transform tpEndPoint)//, float durSpawn, float durWait, float durDispawn)
    {
        // _text.text = $"{value}";
        // _text.color = value <= 0 ? Color.red : _earnEnergyColor;
        
        _tpEndPoint = tpEndPoint;
        _value = value;

        ResetAll();
        SpawnAnimCrystal();
    }

    private void ResetAll()
    {
        _objToMove.transform.DOKill();
        //_objToMove.transform.DOScale(0, 0);
    }

    private void SpawnAnimCrystal()
    {
        _objToMove.transform.DOMoveY(transform.position.y + _yOffset, _durationSpawn).OnComplete(WaitToDispawnCrystal);
        _objToMove.transform.DOScale(1, _durationSpawn);
        
        // AudioManager.Instance.PlaySFX("EnergyGain");
        AudioManager.Instance.PlaySFX("EnergyTrail");
    }

    private void WaitToDispawnCrystal()
    {
        _objToMove.transform.DOScale(Vector3.one, _durationWait).OnComplete(DispawnAnimCrystal);
    }


    private void DispawnAnimCrystal()
    {
        _objToMove.transform.DOJump(_tpEndPoint.position, 20, 1,  _durationDispawn);
        _objToMove.transform.DOScale(0, _durationDispawn).OnComplete(WaitToDelete);
    }

    private void WaitToDelete()
    {
        EnergyManager.Instance.EarnEnergyByCrystal();
        _objToMove.transform.DOScale(0, _durationWaitToDelete).OnComplete(DeleteObj);
    }

    private void DeleteObj()
    {
        KillTween();
        ItemCollectedManager.Instance.DeleteFB(gameObject);
    }

    public void KillTween()
    {
        _objToMove.transform.DOKill();
    }
}