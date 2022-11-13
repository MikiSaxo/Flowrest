using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEditorManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _waterDirectionsNSWE;

    public void ChangeWaterDir(bool[] waterData)
    {
        print(_waterDirectionsNSWE.Length);
        for (int i = 0; i < _waterDirectionsNSWE.Length; i++)
        {
            print(waterData[i]);
            _waterDirectionsNSWE[i].SetActive(waterData[i]);
        }
    }
}