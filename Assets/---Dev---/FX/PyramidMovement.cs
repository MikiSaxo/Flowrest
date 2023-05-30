using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PyramidMovement : MonoBehaviour
{
    // [SerializeField] private float speed = 0.5f;

    public void Init(Transform endPos, float timeToDispawn)
    {
        gameObject.transform.DOScale(0, 0);
        // gameObject.transform.DORotate(new Vector3(0,-30,0), 0);
        // transform.DORotate(new Vector3(0, 360, 0), 1/speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        gameObject.transform.DOMove(endPos.position, timeToDispawn).SetEase(Ease.InSine);
        gameObject.transform.DOScale(1, timeToDispawn).SetEase(Ease.InSine);
        Destroy(gameObject, timeToDispawn);
    }
}
