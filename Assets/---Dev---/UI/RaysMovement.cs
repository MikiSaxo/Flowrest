using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RaysMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), 1/speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }
}
