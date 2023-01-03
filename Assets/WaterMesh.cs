using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMesh : MonoBehaviour
{
    [SerializeField] private GameObject _waterSelectedCubes;

    public void IsEnabled(bool which)
    {
        _waterSelectedCubes.SetActive(which);
    }
}
