using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DefeatAnim : MonoBehaviour
{
    [SerializeField] private Image _bgFilterDefeat;
    [SerializeField] private Image _bgDefeat;
    [SerializeField] private Image _textDefeat;
    [SerializeField] private CanvasGroup _canvasMain;
    [SerializeField] private Sprite _defeatFrench;
    [SerializeField] private Sprite _defeatEnglish;
    
    [SerializeField] private float _timeSpawnBG;
    [SerializeField] private float _timeBetweenBgNText;
    [SerializeField] private float _timeSpawnText;

    public void LaunchAnimDefeat()
    {
        StartCoroutine(AnimDefeat());
    }

    IEnumerator AnimDefeat()
    {
        _textDefeat.sprite = LanguageManager.Instance.Tongue == Language.Francais ? _defeatFrench : _defeatEnglish;
                
        UpdateMainCanvasAlpha(0);
        
        _bgFilterDefeat.gameObject.SetActive(true);
        _bgFilterDefeat.DOFade(0.4f, _timeSpawnBG * .5f);
        
        _bgDefeat.transform.DOScaleY(1, _timeSpawnBG).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(_timeBetweenBgNText);
        _textDefeat.transform.DOScale(1, _timeSpawnText);
        yield return new WaitForSeconds(_timeSpawnText);
        // _textDefeat.gameObject.GetComponent<PointerMotion>().Bounce();

        gameObject.GetComponent<SpawnAnimButtons>().LaunchSpawnAnim();
    }

    public void UpdateMainCanvasAlpha(int alpha)
    {
        _canvasMain.DOFade(alpha, .5f);
    }

    public void ResetAnim()
    {
        _bgFilterDefeat.transform.DOKill();
        _bgDefeat.transform.DOKill();
        _textDefeat.transform.DOKill();
        
        _bgFilterDefeat.DOFade(0,0);
        _bgDefeat.transform.DOScaleY(0, 0);
        _textDefeat.transform.DOScale(0, 0);
        
        _bgFilterDefeat.gameObject.SetActive(false);
        
        gameObject.GetComponent<SpawnAnimButtons>().ResetScaleButtons();
    }
}
