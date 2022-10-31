using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFlowing : MonoBehaviour
{
    [SerializeField] private GameObject _mesh = null;
    [SerializeField] private Material[] _waterMats;
    [SerializeField] private GameObject _indicator = null;

    public bool IsWater = false;
    public bool IsTreated = false;

    private void Start()
    {
        WaterSourceManager.ResetTreatedWater += ResetTreated;
    }

    public void DesactivateWater()
    {
        ChangeMat(_mesh, 0);
        IsWater = false;
    }

    public void ActivateWater()
    {
        ChangeMat(_mesh, 1);
        IsWater = true;
        IsTreated = true;
    }
    
    private void ChangeMat(GameObject which, int mat)
    {
        which.GetComponent<MeshRenderer>().material = _waterMats[mat];
    }

    private void ResetTreated()
    {
        IsTreated = false;
    }

    private void OnDisable()
    {
        WaterSourceManager.ResetTreatedWater -= ResetTreated;
    }
}