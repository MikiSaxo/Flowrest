using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryAnim : MonoBehaviour
{
    [SerializeField] private GameObject _victoryText;
    [SerializeField] private Image _bgVictory;
    [SerializeField] private Image _blockButton;
    [SerializeField] private float _timeSpawnVictoryText = 1;
    [Range(0f,1f)] [SerializeField] private float _alphaBg;

    private void Start()
    {
        ResetVictoryAnim();
    }

    public void VictorySpawnAnim()
    {
        ResetVictoryAnim();
        _victoryText.transform.DOScale(1, _timeSpawnVictoryText).OnComplete(LaunchBounce).SetEase(Ease.InSine);
        _bgVictory.DOFade(_alphaBg, _timeSpawnVictoryText);
        // Here to spawn confettis
    }

    private void LaunchBounce()
    {
        _victoryText.GetComponent<PointerMotion>().Bounce();
        
        gameObject.GetComponent<SpawnAnimButtons>().LaunchSpawnAnimDelay();
        _blockButton.enabled = false;
    }

    public void ResetVictoryAnim()
    {
        _blockButton.enabled = true;
        _bgVictory.DOFade(0, 0);
        _victoryText.transform.DOScale(0, 0);
        gameObject.GetComponent<SpawnAnimButtons>().ResetScaleButtons();
    }
}
