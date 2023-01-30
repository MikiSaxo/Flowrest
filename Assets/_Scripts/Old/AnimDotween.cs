using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;

public class AnimDotween : MonoBehaviour
{
    public static AnimDotween Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BounceAnim(GameObject obj, float start, float end)
    {
        StartCoroutine(Bounce(obj, start, end));
    }

    IEnumerator Bounce(GameObject obj, float start, float end)
    {
        obj.transform.DOScale(Vector3.one * 1.2f, start);
        yield return new WaitForSeconds(start);
        obj.transform.DOScale(Vector3.one, end);
    }
}