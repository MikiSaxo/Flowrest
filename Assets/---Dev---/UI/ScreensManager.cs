using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class ScreensManager : MonoBehaviour
{
    public static ScreensManager Instance;
        
    [Header("Parents")] [SerializeField] private GameObject _bg;
    [SerializeField] private GameObject _dialoguesParent;
    [SerializeField] private GameObject _titlesParent;
    [SerializeField] private GameObject _gameOverParent;
    [SerializeField] private GameObject _menuPauseParent;
    [SerializeField] private GameObject _menuPauseTriggered;

    [Header("Texts")] [SerializeField] private TextMeshProUGUI _dialoguesText;
    [SerializeField] private TextMeshProUGUI _titlesText;
    [SerializeField] private string[] _titlesString;
    
    private List<string> _dialogsToDisplay = new List<string>();
    private bool _isDialogTime;
    private bool _isBeginning;
    private bool _isFirstScreen;
    private int _countScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //FirstScreenOfLevel();
    }

    public void InitDialogs(string[] dialogs, bool isBeginning)
    {
        // print("Init Dialog length = " + dialogs.Length);
        if(_dialogsToDisplay.Count != 0)
            _dialogsToDisplay.Clear();
        
        foreach (var dialog in dialogs)
        {
            _dialogsToDisplay.Add(dialog);
        }

        if(isBeginning)
            BeginningDialog();
    }
    
    public void VictoryScreen()
    {
        _titlesParent.SetActive(true);
        _titlesText.text = _titlesString[0];
        
        _isDialogTime = true;
        
        _dialoguesParent.SetActive(true);
        _dialoguesText.text = _dialogsToDisplay[_countScreen];
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void BeginningDialog()
    {
        _isDialogTime = true;
        _isBeginning = true;
        
        _dialoguesParent.SetActive(true);
        _dialoguesText.text = _dialogsToDisplay[_countScreen];
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void EndBeginningDialog()
    {
        _countScreen = 0;
        _isDialogTime = false;
        _isBeginning = false;
        
        _dialoguesParent.SetActive(false);
        FollowMouse.Instance.IsBlockMouse(false);
        
        // if(MapManager.Instance.GetDialogAtVictory().Length == 0) return;
        
        InitDialogs(MapManager.Instance.GetDialogAtVictory(), false);
        
    }

    public void AlmanachScreen()
    {
        _dialoguesParent.SetActive(false);
        _titlesText.text = _titlesString[1];
    }

    public void GameOver()
    {
        _bg.SetActive(true);
        _gameOverParent.SetActive(true);
        FollowMouse.Instance.IsBlockMouse(true);
    }

    public void UpdatePause(bool state)
    {
        if (state)
        {
            _bg.SetActive(true);
            _menuPauseParent.SetActive(true);
            FollowMouse.Instance.IsBlockMouse(true);
            _menuPauseTriggered.GetComponent<OpenCloseMenu>().IsMenuPauseOpen = true;
        }
        else
        {
            _bg.SetActive(false);
            _menuPauseParent.SetActive(false);
            FollowMouse.Instance.IsBlockMouse(false);
            _menuPauseTriggered.GetComponent<OpenCloseMenu>().IsMenuPauseOpen = false;
            _menuPauseTriggered.GetComponent<OpenCloseMenu>().ForcedOpen = false;
        }
    }

    public void RestartSceneOrLevel()
    {
        _bg.SetActive(false);
        _gameOverParent.SetActive(false);
        FollowMouse.Instance.IsBlockMouse(false);
        _menuPauseParent.SetActive(false);
        _dialoguesParent.SetActive(false);
        _titlesParent.SetActive(false);
    }

    public void ChangeToLevelSupp()
    {
        _isDialogTime = false;
        _countScreen = 0;

        _dialoguesParent.SetActive(false);
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        MapManager.Instance.ResetAllMap();
        // StartCoroutine(UnlockMouse());
    }

    IEnumerator UnlockMouse()
    {
        yield return new WaitForSeconds(.1f);
        FollowMouse.Instance.IsBlockMouse(false);
    }

    private void Update()
    {
        if (!_isDialogTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            _countScreen++;
            
            if (_countScreen < _dialogsToDisplay.Count)
            {
                _dialoguesText.text = _dialogsToDisplay[_countScreen];
            }
            else
            {
                if (_isBeginning)
                    EndBeginningDialog();
                else
                    ChangeToLevelSupp();
            }
        }
    }
}