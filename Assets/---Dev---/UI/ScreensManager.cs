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
    [SerializeField] private GameObject _menuQuest;
    [SerializeField] private TMP_Text _descriptionQuest;
    [SerializeField] private GameObject _menuPauseTriggered;

    [Header("Texts")] [SerializeField] private TextMeshProUGUI _dialoguesText;
    [SerializeField] private TextMeshProUGUI _titlesText;
    [SerializeField] private string[] _titlesString;
    [SerializeField] private float _dialogSpeed = .01f;

    private List<string> _dialogsToDisplay = new List<string>();
    private bool _isDialogTime;
    private bool _isBeginning;
    private bool _isFirstScreen;
    private int _countScreen;
    private int _countDialog;
    private bool _stopCorou;

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
        if (_dialogsToDisplay.Count != 0)
            _dialogsToDisplay.Clear();

        foreach (var dialog in dialogs)
        {
            _dialogsToDisplay.Add(dialog);
        }

        if (isBeginning)
            BeginningDialog();
    }

    public void InitQuestDescription(string text)
    {
        _descriptionQuest.text = text;
    }

    public void VictoryScreen()
    {
        _titlesParent.SetActive(true);
        _titlesText.text = _titlesString[0];

        _isDialogTime = true;

        _dialoguesParent.SetActive(true);
        // _dialoguesText.text = _dialogsToDisplay[_countScreen];
        _dialoguesText.text = String.Empty;
        StartCoroutine(UpdateText());
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void BeginningDialog()
    {
        _isDialogTime = true;
        _isBeginning = true;

        _dialoguesParent.SetActive(true);
        // _dialoguesText.text = _dialogsToDisplay[_countScreen];
        _dialoguesText.text = String.Empty;
        StartCoroutine(UpdateText());
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void EndBeginningDialog()
    {
        _countScreen = 0;
        _countDialog = 0;
        
        _isDialogTime = false;
        _isBeginning = false;

        _dialoguesParent.SetActive(false);
        FollowMouse.Instance.IsBlockMouse(false);

        // if(MapManager.Instance.GetDialogAtVictory().Length == 0) return;

        InitDialogs(MapManager.Instance.GetDialogAtVictory(), false);
        _menuQuest.GetComponent<OpenCloseMenu>().OpenMenuQuest();
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
        _isDialogTime = false;
        _countScreen = 0;
        _countDialog = 0;
        
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
        _countDialog = 0;

        _dialoguesParent.SetActive(false);
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        MapManager.Instance.ResetAllMap(true);
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
            if (_countScreen < _dialogsToDisplay.Count * 2 -1)
            {
                if (_countScreen % 2 == 0)
                {
                    _stopCorou = true;
                    _dialoguesText.text = _dialogsToDisplay[_countDialog];
                    StopCoroutine(UpdateText());
                }
                else
                {
                    // print("anim");
                    _countDialog++;
                    _dialoguesText.text = String.Empty;
                    StartCoroutine(UpdateText());
                }
                _countScreen++;
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

    IEnumerator UpdateText()
    {
        foreach (char c in _dialogsToDisplay[_countDialog].ToCharArray())
        {
            if (_stopCorou)
            {
                // print("stop");
                _stopCorou = false;
                yield break;
                print("drop it");
            }
            _dialoguesText.text += c;
            yield return new WaitForSeconds(_dialogSpeed);
        }
        _countScreen++;
    }

    public bool GetIsDialogTime()
    {
        return _isDialogTime;
    }
}