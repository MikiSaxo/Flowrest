using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseSpawn : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject _textPauseBG;
    [SerializeField] private GameObject _textPause;
    
    [Header("Timing")]
    [SerializeField] private float _timeSpawnBG;
    [SerializeField] private float _timeBetween;
    [SerializeField] private float _timeSpawnText;
    
    public void LaunchSpawnAnim()
    {
        ResetPause();
        StartCoroutine(SpawnAnim());
    }

    IEnumerator SpawnAnim()
    {
        _textPauseBG.transform.DOScaleY(1, _timeSpawnBG).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(_timeBetween);
        _textPause.transform.DOScale(1, _timeSpawnText);
        yield return new WaitForSeconds(_timeSpawnText);
        _textPause.gameObject.GetComponent<PointerMotion>().Bounce();
    }

    public void ResetPause()
    {
        _textPause.transform.DOKill();
        _textPauseBG.transform.DOKill();
        
        _textPause.transform.DOScaleY(0, 0);
        _textPauseBG.transform.DOScaleY(0, 0);
    }
}
