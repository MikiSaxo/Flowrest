using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GroundCollected : MonoBehaviour
{
    public static GroundCollected Instance;

    [Header("Setup")]
    [SerializeField] private Transform[] _tpPoints;
    [SerializeField] private GameObject _rays;
    [SerializeField] private Image _icon;
    
    [Header("Durations")]
    [SerializeField] private float _durationSpawn;
    [SerializeField] private float _durationWait;
    [SerializeField] private float _durationDispawnRays;
    [SerializeField] private float _durationDispawn;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
            // StartAnim();
    }

    public void StartAnim(Sprite icon)
    {
        _icon.sprite = icon;
        ResetAll();
        Spawn();
    }

    private void ResetAll()
    {
        transform.DOKill();
        transform.DOMove(_tpPoints[0].position, 0);
        transform.DOScale(0, 0);
        _rays.transform.DOScale(1, 0);
    }

    private void Spawn()
    {
        transform.DOMove(_tpPoints[1].position,_durationSpawn).OnComplete(WaitToDispawn);
        transform.DOScale(1, _durationSpawn);
    }

    private void WaitToDispawn()
    {
        transform.DOMove(_tpPoints[1].position, _durationWait).OnComplete(DispawnRays);
    }

    private void DispawnRays()
    {
        _rays.transform.DOScale(0, _durationDispawnRays).OnComplete(Dispawn);
    }

    private void Dispawn()
    {
        transform.DOMove(_tpPoints[2].position, _durationDispawn);
        transform.DOScale(0, _durationDispawn);
    }
}
