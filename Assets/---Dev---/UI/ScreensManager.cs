using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ScreensManager : MonoBehaviour
{
    public static ScreensManager Instance;

    [Header("Parents")] [SerializeField] private GameObject _bg;
    [SerializeField] private GameObject _titlesParent;
    [SerializeField] private GameObject _gameOverParent;
    [SerializeField] private GameObject _menuPauseParent;
    [SerializeField] private GameObject _menuQuest;
    [SerializeField] private TMP_Text _descriptionQuest;
    [SerializeField] private Image _imageQuest;
    [SerializeField] private GameObject _menuPauseTriggered;

    [Header("Dialogs")] [SerializeField] private GameObject _dialogParent;
    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private GameObject _dialogContent;
    [SerializeField] private Scrollbar _dialogScrollBar;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private float _dialogSpeed = .01f;

    [Header("Titles")] [SerializeField] private TextMeshProUGUI _titlesText;
    [SerializeField] private string[] _titlesString;

    [Header("Tuto")] [SerializeField] private FB_Arrow _tutoArrow;

    private TMP_Text _dialogText;
    private List<string> _dialogsToDisplay = new List<string>();
    private bool _isDialogTime;
    private bool _isBeginning;
    private bool _isFirstScreen;
    private bool _isPaused;
    private int _countScreen;
    private int _countDialog;
    private bool _stopCorou;
    private bool _isCorouRunning;
    private bool _hasSpawnDialog;
    private string _saveSpawnDialog;

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

    public void InitCharaName(string charaName)
    {
        _characterName.text = charaName;
    }

    public void InitQuestDescription(string text, Sprite img)
    {
        text ??= String.Empty;
        // img ??= null;

        _imageQuest.gameObject.SetActive(img != null);
        _imageQuest.sprite = img;
        _descriptionQuest.text = text;
    }

    public void VictoryScreen()
    {
        MapManager.Instance.IsVictory = true;
        _titlesParent.SetActive(true);
        _titlesText.text = _titlesString[0];

        _isDialogTime = true;

        // _dialoguesParent.SetActive(true);
        _dialogParent.GetComponent<OpenCloseMenu>().OpenAnim();
        // _dialoguesText.text = _dialogsToDisplay[_countScreen];
        // _dialogText.text = String.Empty;
        //
        // _stopCorou = true;
        // _dialogText.text = _dialogsToDisplay[_countDialog];
        // StopCoroutine(UpdateText());
        //
        // if (_dialogsToDisplay.Count != 0)
        //     _dialogsToDisplay.Clear();
        if (_isCorouRunning && _hasSpawnDialog)
        {
            _dialogText.text = _saveSpawnDialog;
            _hasSpawnDialog = false;
        }

        // InitDialogs(MapManager.Instance.GetDialogAtVictory(), false);

        StartCoroutine(UpdateText());
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void BeginningDialog()
    {
        _isDialogTime = true;
        _isBeginning = true;

        // _dialoguesParent.SetActive(true);
        _dialogParent.GetComponent<OpenCloseMenu>().OpenAnim();
        // _dialoguesText.text = _dialogsToDisplay[_countScreen];
        // _dialogText.text = String.Empty;
        StartCoroutine(UpdateText());

        //if(!MapManager.Instance.IsTuto)
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void EndBeginningDialog()
    {
        _countScreen = 0;
        _countDialog = 0;

        _isDialogTime = false;
        _isBeginning = false;

        // _dialoguesParent.SetActive(false);
        if (!MapManager.Instance.IsTuto)
            _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();

        FollowMouse.Instance.IsBlockMouse(false);

        // if(MapManager.Instance.GetDialogAtVictory().Length == 0) return;

        InitDialogs(MapManager.Instance.GetDialogAtVictory(), false);
        _menuQuest.GetComponent<OpenCloseMenu>().OpenMenuQuest();
    }

    public void SpawnDialog(string[] dialogs)
    {
        // _isDialogTime = true;
        // _isBeginning = true;
        _hasSpawnDialog = true;
        
        _dialogParent.GetComponent<OpenCloseMenu>().OpenAnim();

        if (_dialogsToDisplay.Count != 0)
            _dialogsToDisplay.Clear();

        foreach (var dialog in dialogs)
        {
            _dialogsToDisplay.Add(dialog);
        }

        _saveSpawnDialog = _dialogsToDisplay[0];

        StartCoroutine(UpdateText());

        _countScreen--;
        InitDialogs(MapManager.Instance.GetDialogAtVictory(), false);
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
            _isPaused = true;
            _bg.SetActive(true);
            _menuPauseParent.SetActive(true);
            FollowMouse.Instance.IsBlockMouse(true);
            _menuPauseTriggered.GetComponent<OpenCloseMenu>().IsMenuPauseOpen = true;
        }
        else
        {
            _isPaused = false;
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
        // _dialoguesParent.SetActive(false);
        // _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();
        _titlesParent.SetActive(false);
    }

    public void ChangeToLevelSupp()
    {
        _isDialogTime = false;
        _countScreen = 0;
        _countDialog = 0;

        // _dialoguesParent.SetActive(false);
        _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        MapManager.Instance.ResetAllMap(true);
        // StartCoroutine(UnlockMouse());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdatePause(!_isPaused);

        if (!_isDialogTime) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_countScreen < _dialogsToDisplay.Count * 2 - 1)
            {
                if (_countScreen % 2 == 0)
                {
                    _stopCorou = true;
                    _dialogText.text = _dialogsToDisplay[_countDialog];
                    StopCoroutine(UpdateText());
                }
                else
                {
                    // print("anim");
                    _countDialog++;
                    //_dialogText.text = String.Empty;
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

    private const float SPACING_BETWEEN_TWO_DIALOG = 18;

    IEnumerator UpdateText()
    {
        _isCorouRunning = true; 
        
        GameObject go = Instantiate(_dialogPrefab, _dialogContent.transform);
        var goDialog = go.GetComponent<DialogPrefab>();

        var newDialog = _dialogsToDisplay[_countDialog];

        if (newDialog == String.Empty)
            newDialog = " ";

        goDialog.Init(newDialog);
        _dialogText = goDialog.DialogText;

        float textSize = goDialog.GetDialogSizeY();
        _dialogContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, textSize + SPACING_BETWEEN_TWO_DIALOG);

        int charIndex = 0;

        foreach (char c in newDialog.ToCharArray())
        {
            if (_stopCorou)
            {
                _stopCorou = false;
                yield break;
            }

            _dialogScrollBar.value = 0;

            charIndex++;

            var firstText = newDialog.Substring(0, charIndex);
            var secondText = $"<color=#00000000>{newDialog.Substring(charIndex)}";
            _dialogText.text = firstText + secondText;

            yield return new WaitForSeconds(_dialogSpeed);
        }

        _countScreen++;
        _isCorouRunning = false;
    }

    public void UpdateTutoArrow(bool state)
    {
        _tutoArrow.UpdateArrow(state);
    }

    public bool GetIsDialogTime()
    {
        return _isDialogTime;
    }
}