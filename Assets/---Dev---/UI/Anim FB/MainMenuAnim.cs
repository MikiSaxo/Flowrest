using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnim : MonoBehaviour
{
    [SerializeField] private CameraPan _mainCam;
    [SerializeField] private Image _p3dLogoImg;
    [SerializeField] private Image _filterBlueImg;
    [SerializeField] private GameObject _BGTitle;
    [SerializeField] private GameObject _title;
    [SerializeField] private TMP_Text _continueText;
    [SerializeField] private SpawnAnimButtons _mainScreen;


    [Header("Logo")] 
    [SerializeField] private float _timeStartFade;
    [SerializeField] private float _timeFadeInLogo;
    [SerializeField] private float _timeBetweenFadeLogo;
    [SerializeField] private float _timeFadeOutLogo;
    
    [Header("Camera")] [SerializeField] private float _timeRotationCam;
    
    [Header("Title")] [SerializeField] private float _timeSpawnBGTitle;
    [SerializeField] private float _timeSpawnBetween;
    [SerializeField] private float _timeSpawnTitle;
    [SerializeField] private float _timeBetweenBounce;
    [SerializeField] private float _timeSpawnFilter;

    [Header("Continue Text")] [SerializeField] private float _timeFadeInText;
    [SerializeField] private float _timeFadeOutText;

    private bool _hasClick;
    private bool _isIntroEnd;
    private void Start()
    {
        LaunchFadeLogoP3D();
        // LaunchTitleSpawn();
        CursorManager.Instance.UpdateVisibleCursor(false);
    }

    private void LaunchFadeLogoP3D()
    {
        _p3dLogoImg.DOFade(0, 0);
        StartCoroutine(FadeInOutLogo());
    }

    IEnumerator FadeInOutLogo()
    {
        yield return new WaitForSeconds(_timeStartFade);
        _p3dLogoImg.DOFade(1, _timeFadeInLogo).SetEase(Ease.InSine);
        yield return new WaitForSeconds(_timeFadeInLogo);
        yield return new WaitForSeconds(_timeBetweenFadeLogo);
        _p3dLogoImg.DOFade(0, _timeFadeOutLogo).OnComplete(RotationCam);
    }

    private void RotationCam()
    {
        _mainCam.RotationAnimation(_timeRotationCam);
        gameObject.transform.DOScale(1, _timeRotationCam).OnComplete(LaunchTitleSpawn);
    }

    private void LaunchTitleSpawn()
    {
        StartCoroutine(TitleSpawn());
    }

    IEnumerator TitleSpawn()
    {
        _filterBlueImg.DOFade(.26f, _timeSpawnFilter);
        _BGTitle.transform.DOScale(1, _timeSpawnBGTitle).SetEase(Ease.InSine).OnComplete(LaunchBounceExagone);
        yield return new WaitForSeconds(_timeSpawnBetween);
        _title.transform.DOScale(1, _timeSpawnTitle).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(_timeSpawnTitle);
        // _title.GetComponent<PointerMotion>().Bounce();
        StartCoroutine(FadeInOutContinueText());

        _isIntroEnd = true;
        CursorManager.Instance.UpdateVisibleCursor(true);
    }

    private void LaunchBounceExagone()
    {
        StartCoroutine(BounceExagone());
    }

    IEnumerator BounceExagone()
    {
        _BGTitle.GetComponent<PointerMotion>().Bounce();
        yield return new WaitForSeconds(_timeBetweenBounce);
        // _BGTitle.transform.DORotate(new Vector3(0, 0, _BGTitle.gameObject.transform.rotation.eulerAngles.z + 180), _timeSpawnBGTitle * 2, RotateMode.FastBeyond360).SetEase(Ease.OutSine);

        StartCoroutine(BounceExagone());
    }

    IEnumerator FadeInOutContinueText()
    {
        _continueText.DOFade(1, _timeFadeInText);
        yield return new WaitForSeconds(_timeFadeInText);
        _continueText.DOFade(0, _timeFadeOutLogo);
        yield return new WaitForSeconds(_timeFadeOutText);
        
        StartCoroutine(FadeInOutContinueText());
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && !_hasClick && _isIntroEnd)
            OnClick();
    }

    private void OnClick()
    {
        _hasClick = true;
        StopCoroutine(BounceExagone());
        _continueText.DOKill();
        StopCoroutine(FadeInOutContinueText());
        _continueText.gameObject.SetActive(false);
        
        _BGTitle.transform.DOKill();
        _title.transform.DOKill();
        
        _continueText.DOFade(0, _timeFadeOutLogo);
        _BGTitle.GetComponent<Image>().DOFade(0, _timeFadeOutLogo);
        _title.GetComponent<Image>().DOFade(0, _timeFadeOutLogo);

        _mainScreen.LaunchSpawnAnimDelay();
    }
}