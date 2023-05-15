using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CloudMovement : MonoBehaviour
{
    public void InitMovement(Transform endPos, float timeToEnd)
    {
        gameObject.transform.DOMoveX(endPos.position.x, timeToEnd).OnComplete(DestroyCloud).SetEase(Ease.Linear);
    }

    private void DestroyCloud()
    {
        Destroy(gameObject);
    }
}
