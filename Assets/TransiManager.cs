using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransiManager : MonoBehaviour
{
    [SerializeField] private float _timeBetweenColumnGrowOn;
    [SerializeField] private float _timeBetweenColumnShrink;
    [SerializeField] private TransiColumn[] _columns;
    
    void Start()
    {
        StartCoroutine(LaunchShrink());
    }

    IEnumerator LaunchGrownOn()
    {
        foreach (var column in _columns)
        {
            foreach (var hexa in column.Column)
            {
                hexa.GetComponent<TransiHexagone>().GrowOn();
            }

            yield return new WaitForSeconds(_timeBetweenColumnGrowOn);
        }
    }

    IEnumerator LaunchShrink()
    {
        foreach (var column in _columns)
        {
            foreach (var hexa in column.Column)
            {
                hexa.GetComponent<TransiHexagone>().Shrink();
            }

            yield return new WaitForSeconds(_timeBetweenColumnShrink);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            StartCoroutine(LaunchGrownOn());

    }
}
