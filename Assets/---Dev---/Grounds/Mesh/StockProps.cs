using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockProps : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _propsMesh;
    [SerializeField] private Animator[] _propsAnim;
    private Material[] _propMat;

    private void Awake()
    {
        _propMat = new Material[_propsMesh.Length];

        for (int i = 0; i < _propsMesh.Length; i++)
        {
            _propMat[i] = _propsMesh[i].material;
        }
    }


    public Material[] GetProps()
    {
        return _propMat;
    }
}
