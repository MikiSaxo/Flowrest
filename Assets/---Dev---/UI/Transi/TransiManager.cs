using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransiManager : MonoBehaviour
{
    public static TransiManager Instance;
    
    [Header("Grow")]
    [SerializeField] private float _timeBetweenColumnGrowOn;
    [SerializeField] private float _timeGrowOn;
    [Header("Shrink")]
    [SerializeField] private float _timeBetweenColumnShrink;
    [SerializeField] private float _timeShrink;
    [SerializeField] private TransiColumn[] _columns;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // StartCoroutine(Shrink());
    }

    public void LaunchGrownOn()
    {
        StartCoroutine(GrownOn());
    }

    IEnumerator GrownOn()
    {
        foreach (var column in _columns)
        {
            foreach (var hexa in column.Column)
            {
                hexa.GetComponent<TransiHexagone>().GrowOn(_timeGrowOn);
            }

            yield return new WaitForSeconds(_timeBetweenColumnGrowOn);
        }
    }
    
    public void LaunchShrink()
    {
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        foreach (var column in _columns)
        {
            foreach (var hexa in column.Column)
            {
                hexa.GetComponent<TransiHexagone>().Shrink(_timeShrink);
            }

            yield return new WaitForSeconds(_timeBetweenColumnShrink);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            StartCoroutine(GrownOn());

    }

    public float GetTimeForGrowOn()
    {
        return _columns.Length * _timeBetweenColumnGrowOn + _timeGrowOn;
    }
}
