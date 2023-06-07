using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXWaveClic : MonoBehaviour
{
    public static FXWaveClic Instance;
    
    
    [SerializeField] private GameObject _fxWaterWaveClick;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnFXWaterWave(Ray ray)
    {
        if (MapManager.Instance.IsOnUI) return;
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject go = Instantiate(_fxWaterWaveClick, hit.point, Quaternion.identity);
            
            var position = go.transform.position;
            position = new Vector3(position.x, 2, position.z-2);
            go.transform.position = position;
            go.transform.SetParent(gameObject.transform);
        }
    }
}
