using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigManager : MonoBehaviour
{
    public static BigManager Instance;

    public int CurrentLevel { get; set; }
    public int LevelUnlocked { get; set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
    }
}
