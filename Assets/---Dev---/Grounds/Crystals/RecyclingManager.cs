using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RecyclingManager : MonoBehaviour
{
    public static RecyclingManager Instance;

    [SerializeField] private GameObject _recycling;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateRecycling(bool activateOrNot)
    {
        _recycling.SetActive(activateOrNot);
        gameObject.GetComponent<PointerMotion>().UpdateCanEnter(activateOrNot);
    }
}
