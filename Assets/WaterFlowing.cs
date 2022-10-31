using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFlowing : MonoBehaviour
{
    [SerializeField] private GameObject _mesh = null;
    [SerializeField] private GameObject _indicator = null;
    public bool IsWater = false;
    private void Start()
    {
    }

    public void DesactivateWater()
    {
        gameObject.GetComponent<GroundManager>().ChangeMat(_mesh, 0);
        gameObject.GetComponent<GroundManager>().ChangeMat(_indicator, 0);
        IsWater = false;
    }

    public void ActivateWater()
    {
        gameObject.GetComponent<GroundManager>().ChangeMat(_mesh, 5);
        gameObject.GetComponent<GroundManager>().ChangeMat(_indicator, 5);
        IsWater = true;
    }
}
