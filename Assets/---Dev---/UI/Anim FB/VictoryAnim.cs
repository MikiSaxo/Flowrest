using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryAnim : MonoBehaviour
{
    [SerializeField] private Image _bgFilterVictory;
    [SerializeField] private Image _bgVictory;
    [SerializeField] private Image _textVictory;
    [SerializeField] private Image[] _exagone;
    [SerializeField] private CanvasGroup _canvasMain;
    [SerializeField] private Sprite _victoryFrench;
    [SerializeField] private Sprite _victoryEnglish;

    [SerializeField] private float _timeSpawnBG;
    [SerializeField] private float _timeBetweenBgNText;
    [SerializeField] private float _timeSpawnText;
    [SerializeField] private float _timeBetweenExagone;
    [SerializeField] private float _timeSpawnExagone;

    public void LaunchAnimVictory()
    {
        StartCoroutine(AnimVictory());
    }

    IEnumerator AnimVictory()
    {
        _textVictory.sprite = LanguageManager.Instance.Tongue == Language.Francais ? _victoryFrench : _victoryEnglish;
                
        UpdateMainCanvasAlpha(0);
        
        _bgFilterVictory.gameObject.SetActive(true);
        _bgFilterVictory.DOFade(0.4f, _timeSpawnBG * .5f);
        
        _bgVictory.transform.DOScaleY(1, _timeSpawnBG).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(_timeBetweenBgNText);
        _textVictory.transform.DOScale(1, _timeSpawnText);
        yield return new WaitForSeconds(_timeSpawnText);
        _textVictory.gameObject.GetComponent<PointerMotion>().Bounce();

        foreach (var exa in _exagone)
        {
            yield return new WaitForSeconds(_timeBetweenExagone);
            exa.transform.DOScale(1, _timeSpawnExagone).SetEase(Ease.OutBounce);
            exa.transform.DORotate(new Vector3(0, 0, exa.gameObject.transform.rotation.eulerAngles.z + 180),
                _timeSpawnExagone * 2, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
        }
        
        gameObject.GetComponent<SpawnAnimButtons>().LaunchSpawnAnim();

        yield return new WaitForSeconds(_timeBetweenExagone * _exagone.Length);

        StartCoroutine(BounceExagone());
    }


    IEnumerator BounceExagone()
    {
        foreach (var exa in _exagone)
        {
            yield return new WaitForSeconds(_timeBetweenExagone);
            exa.gameObject.GetComponent<PointerMotion>().Bounce();
            exa.transform.DORotate(new Vector3(0, 0, exa.gameObject.transform.rotation.eulerAngles.z + 180),
                _timeSpawnExagone * 2, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
        }

        yield return new WaitForSeconds(_timeBetweenExagone * _exagone.Length);

        StartCoroutine(BounceExagone());
    }

    public void UpdateMainCanvasAlpha(int alpha)
    {
        _canvasMain.DOFade(alpha, .5f);
    }

    public void ResetAnim()
    {
        StopCoroutine(BounceExagone());
        _bgFilterVictory.transform.DOKill();
        _bgVictory.transform.DOKill();
        _textVictory.transform.DOKill();
        
        _bgFilterVictory.DOFade(0,0);
        _bgVictory.transform.DOScaleY(0, 0);
        _textVictory.transform.DOScale(0, 0);
        foreach (var exa in _exagone)
        {
            exa.transform.DOKill();
            exa.transform.DOScale(0, 0);
        }
        
        _bgFilterVictory.gameObject.SetActive(false);
        
        gameObject.GetComponent<SpawnAnimButtons>().ResetScaleButtons();
    }
}