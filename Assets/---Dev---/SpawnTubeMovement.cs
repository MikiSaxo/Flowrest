using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnTubeMovement : MonoBehaviour
{
    [SerializeField] private float _timeSpawnWave;
    [SerializeField] private GameObject _waveStart;
    [SerializeField] private GameObject _foamStart;
    [SerializeField] private float _timeToSpawnTube;
    [SerializeField] private float _timeDissolveWaterTube;
    [SerializeField] private GameObject _waveEnd;
    [SerializeField] private GameObject _foamEnd;
    [SerializeField] private float _timeDisappearWaterTube;
    [SerializeField] private float _timeDestroy;

    [SerializeField] private Material _mat;
    void Start()
    {
        gameObject.transform.DOScale(1, _timeSpawnWave).OnComplete(SpawnWave);
        _waveEnd.SetActive(false);
        _foamEnd.SetActive(false);

        // Dissolve
        _mat.DOFloat(1, "Vector1_3b0bddb6200046a9b085f11bb209c326", 0);
        // Reverse Dissolve
        _mat.DOFloat(1, "Vector1_6952a5337778416b8f3c3c0541e6afcd", _timeDisappearWaterTube);
        
        // _waveStart.transform.DOScale(0, 0);
        // _foamStart.transform.DOScale(0, 0);
        //
        // _waveStart.transform.DOScale(new Vector3(269.2195f,269.2195f,66.91433f), _timeToSpawnTube);
        // _foamStart.transform.DOScale(1, _timeToSpawnTube);
    }

    private void SpawnWave()
    {
        _waveStart.SetActive(true);
        gameObject.transform.DOScale(1, _timeToSpawnTube).OnComplete(SpawnWaterTube);
    }

    private void SpawnWaterTube()
    {
        // Dissolve
        _mat.DOFloat(0, "Vector1_3b0bddb6200046a9b085f11bb209c326", _timeDissolveWaterTube);

        gameObject.transform.DOScale(1, _timeDissolveWaterTube).OnComplete(SpawnWaveFoamEnd);
        
        // _waveStart.transform.DOScale(0, _timeDissolveWaterTube);
        // _foamStart.transform.DOScale(0, _timeDissolveWaterTube);
    }

    private void SpawnWaveFoamEnd()
    {
        // Reverse Dissolve
        _mat.DOFloat(0, "Vector1_6952a5337778416b8f3c3c0541e6afcd", _timeDisappearWaterTube);

        _waveEnd.SetActive(true);
        _foamEnd.SetActive(true);
        
        _waveStart.SetActive(false);
        
        gameObject.transform.DOScale(1, _timeDisappearWaterTube).OnComplete(TimeWaitDestroy);
    }

    private void TimeWaitDestroy()
    {
        // _waveEnd.transform.DOScale(0, _timeDestroy);
        _waveEnd.SetActive(false);

        // _foamEnd.transform.DOScale(0, _timeDestroy);
        gameObject.transform.DOScale(1, _timeDestroy).OnComplete(DestroyFX);
    }

    private void DestroyFX()
    {
        Destroy(gameObject);
    }
}
