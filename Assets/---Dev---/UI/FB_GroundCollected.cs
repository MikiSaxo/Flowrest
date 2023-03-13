using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FB_GroundCollected : MonoBehaviour
{
    [Header("Setup")] [SerializeField] private GameObject _objToMove;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;

    [Header("Rays")] [SerializeField] private GameObject _rays;
    [SerializeField] private float _durationDispawnRays;

    [Header("Durations")] [SerializeField] private float _durationSpawn;
    [SerializeField] private float _durationWait;
    [SerializeField] private float _durationDispawn;

    private Transform[] _tpPoints;

    public void Init(Sprite icon, string text, Transform[] tpPoints)//, float durSpawn, float durWait, float durDispawn)
    {
        _icon.sprite = icon;
        _text.text = text;

        // _durationSpawn = durSpawn;
        // _durationWait = durWait;
        // _durationDispawn = durDispawn;

        _tpPoints = tpPoints;

        ResetAll();
        SpawnAnim();
    }

    private void ResetAll()
    {
        _objToMove.transform.DOKill();
        _objToMove.transform.DOMove(_tpPoints[0].position, 0);
        _objToMove.transform.DOScale(0, 0);

        _rays.transform.DOScale(1, 0);
    }

    private void SpawnAnim()
    {
        _objToMove.transform.DOMove(_tpPoints[1].position, _durationSpawn).OnComplete(WaitToDispawn);
        _objToMove.transform.DOScale(1, _durationSpawn);
    }

    private void WaitToDispawn()
    {
        _objToMove.transform.DOMove(_tpPoints[1].position, _durationWait).OnComplete(DispawnRays);
    }

    private void DispawnRays()
    {
        _rays.transform.DOScale(0, _durationDispawnRays).OnComplete(DispawnAnim);
    }

    private void DispawnAnim()
    {
        _objToMove.transform.DOMove(_tpPoints[2].position, _durationDispawn);
        _objToMove.transform.DOScale(0, _durationDispawn).OnComplete(DeleteObj);

        SetupUIGround.Instance.GroundStockage.OnMouseEntered();
    }

    private void DeleteObj()
    {
        Destroy(gameObject);
    }
}