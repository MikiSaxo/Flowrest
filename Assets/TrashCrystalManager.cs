using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrashCrystalManager : MonoBehaviour
{
    public static TrashCrystalManager Instance;

    [SerializeField] private GameObject _trashCan;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTrashCan(bool activateOrNot)
    {
        _trashCan.SetActive(activateOrNot);
    }
}
