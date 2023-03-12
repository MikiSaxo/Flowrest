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
    [SerializeField] private string[] _dialoguesString;
    [SerializeField] private string[] _titlesString;

    private bool _isVictory;
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

    public void FirstScreenOfLevel()
    {
        FollowMouse.Instance.IsBlockMouse(true);
        _bg.SetActive(true);
        _dialoguesParent.SetActive(true);
        _dialoguesText.text = _dialoguesString[0];
        _isFirstScreen = true;
    }

    public void LeaveFirstScreen()
    {
        FollowMouse.Instance.IsBlockMouse(false);
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        _dialoguesParent.SetActive(false);
        _isFirstScreen = false;
    }
    public void VictoryScreen()
    {
        _bg.SetActive(true);
        _titlesParent.SetActive(true);
        _titlesText.text = _titlesString[0];
        _isVictory = true;
        FollowMouse.Instance.IsBlockMouse(true);
    }

    public void VictoryDialogues()
    {
        _dialoguesParent.SetActive(true);
        _dialoguesText.text = _dialoguesString[MapManager.Instance.GetActualLevel()+1];
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
        _countScreen = 0;
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        MapManager.Instance.ResetAllMap();
        _isVictory = false;
        StartCoroutine(UnlockMouse());
    }

    IEnumerator UnlockMouse()
    {
        yield return new WaitForSeconds(.1f);
        FollowMouse.Instance.IsBlockMouse(false);
    }

    private void Update()
    {
        if(_isFirstScreen && Input.GetMouseButtonDown(0))
            LeaveFirstScreen();
        
        if (!_isVictory) return;

        if (Input.GetMouseButtonDown(0))
        {
            _countScreen++;

            switch (_countScreen)
            {
                case 1:
                    VictoryDialogues();
                    break;
                case 2:
                    AlmanachScreen();
                    break;
                case 3:
                    ChangeToLevelSupp();
                    break;
            }
        }
    }
}