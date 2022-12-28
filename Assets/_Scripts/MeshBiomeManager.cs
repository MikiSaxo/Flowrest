using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBiomeManager : MonoBehaviour
{
    [SerializeField] private GameObject _simple;
    [SerializeField] private GameObject _biome;


    public void TransformTo(bool isBiome)
    {
        if (isBiome)
        {
            _simple.SetActive(false);
            _biome.SetActive(true);
        }
        else
        {
            _simple.SetActive(true);
            _biome.SetActive(false);
        }
    }
}
