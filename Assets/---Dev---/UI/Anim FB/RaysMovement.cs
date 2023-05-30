using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RaysMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private Vector3 _axis = new Vector3(0,0,360);
    void Start()
    {
        transform.DORotate(_axis, 1/_speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InSine);
    }
}
