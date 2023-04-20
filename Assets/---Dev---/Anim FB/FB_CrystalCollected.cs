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
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _yOffset;
    
    [Header("Durations")] [SerializeField] private float _durationSpawn;
    [SerializeField] private float _durationWait;
    [SerializeField] private float _durationDispawn;
    
    private Transform _tpEndPoint;
    private int _value;

    public void Init(int value, Transform tpEndPoint)//, float durSpawn, float durWait, float durDispawn)
    {
        _text.text = $"{value}";
        _text.color = value <= 0 ? Color.red : Color.green;
        
        _tpEndPoint = tpEndPoint;
        _value = value;

        ResetAll();
        SpawnAnim();
    }

    private void ResetAll()
    {
        _objToMove.transform.DOKill();
        //_objToMove.transform.DOScale(0, 0);
    }

    private void SpawnAnim()
    {
        _objToMove.transform.DOMoveY(transform.position.y + _yOffset, _durationSpawn).OnComplete(WaitToDispawn);
        _objToMove.transform.DOScale(1, _durationSpawn);
    }

    private void WaitToDispawn()
    {
        _objToMove.transform.DOScale(Vector3.one, _durationWait).OnComplete(DispawnAnim);
    }


    private void DispawnAnim()
    {
        _objToMove.transform.DOMove(_tpEndPoint.position, _durationDispawn);
        _objToMove.transform.DOScale(0, _durationDispawn).OnComplete(DeleteObj);
    }

    private void DeleteObj()
    {
        KillTween();
        // EnergyManager.Instance.UpdateEnergy(_value);
        ItemCollectedManager.Instance.DeleteFB(gameObject);
    }

    public void KillTween()
    {
        _objToMove.transform.DOKill();
    }
}