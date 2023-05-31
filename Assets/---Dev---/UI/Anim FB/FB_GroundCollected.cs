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
    private AllStates _state;

    public void Init(Sprite icon, Color color, string text, Transform[] tpPoints, AllStates state)//, float durSpawn, float durWait, float durDispawn)
    {
        _icon.sprite = icon;
        _icon.color = color;
        _text.text = text;
        _tpPoints = tpPoints;
        _state = state;

        // _durationSpawn = durSpawn;
        // _durationWait = durWait;
        // _durationDispawn = durDispawn;

        ResetAll();
        SpawnAnimGround();
    }

    private void ResetAll()
    {
        _objToMove.transform.DOKill();
        _objToMove.transform.DOMove(_tpPoints[0].position, 0);
        _objToMove.transform.DOScale(0, 0);

        _rays.transform.DOScale(1, 0);
    }

    private void SpawnAnimGround()
    {
        _objToMove.transform.DOMove(_tpPoints[1].position, _durationSpawn).OnComplete(WaitToDispawnGround);
        _objToMove.transform.DOScale(1, _durationSpawn);
    }

    private void WaitToDispawnGround()
    {
        _objToMove.transform.DOMove(_tpPoints[1].position, _durationWait).OnComplete(DispawnRays);
    }

    private void DispawnRays()
    {
        _rays.transform.DOScale(0, _durationDispawnRays).OnComplete(DispawnAnimGround);
    }

    private void DispawnAnimGround()
    {
        SetupUIGround.Instance.ChangeSizeBGBeforeNewGround((int)_state, false);
        
        _objToMove.transform.DOMove(_tpPoints[2].position, _durationDispawn);
        _objToMove.transform.DOScale(0, _durationDispawn).OnComplete(DeleteObj);

        //SetupUIGround.Instance.GroundStockage.OnMouseEntered();
    }

    private void DeleteObj()
    {

        KillTween();
        SetupUIGround.Instance.AddNewGround((int)_state, false);
        ItemCollectedManager.Instance.DeleteFB(gameObject);
        
        MapManager.Instance.QuestsManager.CheckQuest();
        MapManager.Instance.CheckIfGameOver();
    }

    public void KillTween()
    {
        _objToMove.transform.DOKill();
    }
}