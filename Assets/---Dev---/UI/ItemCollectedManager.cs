using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class ItemCollectedManager : MonoBehaviour
{
    public static ItemCollectedManager Instance;

    [Header("Setup")] [SerializeField] private Transform[] _tpPointsGround;
    [SerializeField] private Transform[] _tpPointsCrystals;
    [SerializeField] private GameObject _prefabGroundCollected;
    [SerializeField] private GameObject _prefabCrystalCollected;
    [SerializeField] private GameObject _feedbacksParent;

    private List<GameObject> _stockFB = new List<GameObject>();
    // [Header("Durations, 0:Ground, 1:Crystal")] 
    // [SerializeField]
    // private float[] _durationSpawn;
    //
    // [SerializeField] private float[] _durationWait;
    // [SerializeField] private float[] _durationDispawn;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // StartAnim();
    }

    public void SpawnFBGroundCollected(Sprite icon, string text, AllStates state)
    {
        GameObject go = Instantiate(_prefabGroundCollected, _feedbacksParent.transform);
        go.GetComponent<FB_GroundCollected>().Init(icon, text, _tpPointsGround, state);

        _stockFB.Add(go);
    }

    public void SpawnFBEnergyCollected(int value)
    {
        if (value == 0) return;
        
        GameObject go = Instantiate(_prefabCrystalCollected, _feedbacksParent.transform);
        go.transform.position = Input.mousePosition;
        go.GetComponent<FB_CrystalCollected>().Init(value, _tpPointsCrystals[2]);
        
        _stockFB.Add(go);
    }

    private void DeleteFB(GameObject fb)
    {
        _stockFB.Remove(fb);
    }

    public void DeleteAllFB()
    {
        foreach (var fb in _stockFB)
        {
            if(fb.GetComponent<FB_GroundCollected>() != null)
                fb.GetComponent<FB_GroundCollected>().KillTween();
            if(fb.GetComponent<FB_CrystalCollected>() != null)
                fb.GetComponent<FB_CrystalCollected>().KillTween();
            
            DeleteFB(fb);
        }
        _stockFB.Clear();
    }
}