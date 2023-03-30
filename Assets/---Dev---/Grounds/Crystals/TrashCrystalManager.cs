using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrashCrystalManager : MonoBehaviour
{
    public static TrashCrystalManager Instance;

    [SerializeField] private GameObject _trashCan;
    [SerializeField] private GameObject _fBTrashCan;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTrashCan(bool activateOrNot)
    {
        //_fBTrashCan.SetActive(false);
        _trashCan.SetActive(activateOrNot);
    }
}
