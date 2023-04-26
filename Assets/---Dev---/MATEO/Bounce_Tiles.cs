using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Bounce_Tiles : MonoBehaviour
{

    public float StartSize;
    public float EndSize;

    public float AnimationTime;

    public float EaseNumber;
    public float EaseDuration;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(StartSize, 0f);
        RunAnim();
    }

    public void RunAnim()
    {
        transform.DOScale(EndSize, AnimationTime)
            .SetEase(Ease.OutElastic, EaseNumber, EaseDuration);
    }
}
