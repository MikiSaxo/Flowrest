using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] private Image _cursorImg;
    
    [Header("Normal")] [SerializeField] private Sprite _idle;
    [SerializeField] private Sprite _click;
    
    [Header("Foot")] [SerializeField] private Sprite _idleFoot;
    [SerializeField] private Sprite _clickFoot;
    
    [Header("Gold")] [SerializeField] private Sprite _idleGold;
    [SerializeField] private Sprite _clickGold;

    private CursorChanges _currentCursor;
    private List<Sprite> _idleSprite = new List<Sprite>();
    private List<Sprite> _clickSprite = new List<Sprite>();

    private bool _isAndroid;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.visible = false;

        if (MapManager.Instance != null && MapManager.Instance.IsAndroid)
        {
            _cursorImg.enabled = false;
            _isAndroid = true;
            return;
        }
        else if (MainMenuManager.Instance != null && MainMenuManager.Instance.IsAndroid)
        {
            _cursorImg.enabled = false;
            _isAndroid = true;
            return;
        }
        
        _idleSprite.Add(_idle);
        _idleSprite.Add(_idleFoot);
        _idleSprite.Add(_idleGold);
        
        _clickSprite.Add(_click);
        _clickSprite.Add(_clickFoot);
        _clickSprite.Add(_clickGold);
        
        UpdateCursor(CursorChanges.Normal);
    }

    public void UpdateCursor(CursorChanges cursorChanges)
    {
        if (_isAndroid) return;
        
        _currentCursor = cursorChanges;

        _cursorImg.sprite = _idleSprite[(int)_currentCursor];
    }

    private void Update()
    {
        if (_isAndroid) return;

        if(Input.GetMouseButton(0))
            _cursorImg.sprite = _clickSprite[(int)_currentCursor];

        if (Input.GetMouseButtonUp(0))
        {
            _cursorImg.sprite = _idleSprite[(int)_currentCursor];
            Cursor.visible = false;
        }
    }

    public void UpdateVisibleCursor(bool state)
    {
        if (_isAndroid) return;

        _cursorImg.enabled = state;
    }
}

public enum CursorChanges
{
    Normal = 0,
    Foot = 1,
    Gold = 2
}
