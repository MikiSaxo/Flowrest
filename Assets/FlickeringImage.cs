using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlickeringImage : MonoBehaviour
{
    [SerializeField] private float _timeFlickering;
    [SerializeField] private float _timeWaitFadeOut;
    [SerializeField] private float _timeWaitFadeIn;
    private Image _img;
    void Start()
    {
        _img = GetComponent<Image>();
        FadeOut();
    }

    private void FadeIn()
    {
        _img.DOFade(1, _timeFlickering).OnComplete(WaitFadeIn);
    }
    
    private void WaitFadeIn()
    {
        _img.DOFade(0, _timeWaitFadeIn).OnComplete(FadeOut);
    }

    private void FadeOut()
    {
        _img.DOFade(0, _timeFlickering).OnComplete(WaitFadeOut);
    }

    private void WaitFadeOut()
    {
        _img.DOFade(0, _timeWaitFadeOut).OnComplete(FadeIn);
    }
}
