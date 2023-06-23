using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FxSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private GameObject[] _fxPrefab;
    [SerializeField] private float _timeToSpawn;
    [SerializeField] private float _timeToEnd;
    private float _cooldown;
    private int _lastRandom;

    private void SpawnFx()
    {
        int randomStartPos = Random.Range(0, _startPos.Length);
        if (_lastRandom == randomStartPos)
            randomStartPos++;
        if (randomStartPos >= _startPos.Length)
            randomStartPos = 0;
        
        _lastRandom = randomStartPos;

        
        int randomFx = Random.Range(0, _fxPrefab.Length);

        GameObject go = Instantiate(_fxPrefab[randomFx], _startPos[randomStartPos].transform);

        if (go.GetComponent<CloudMovement>() != null)
        {
            go.GetComponent<CloudMovement>().InitMovement(_endPos, _timeToEnd);
        }
        
        if (go.GetComponent<PyramidMovement>() != null)
        {
            go.GetComponent<PyramidMovement>().Init(_endPos, _timeToEnd);
        }
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (_cooldown <= 0)
        {
            SpawnFx();
            _cooldown = _timeToSpawn;
        }
    }
}
