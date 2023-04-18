using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.ProBuilder.MeshOperations;
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
    [SerializeField] private GameObject _nextLevel;

    [Header("Dialogs")] [SerializeField] private GameObject _dialogParent;
    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private GameObject _dialogContent;
    [SerializeField] private Scrollbar _dialogScrollBar;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private GameObject _dialogFBEnd;
    [SerializeField] private float _dialogSpeed = .01f;

    [Header("Titles")] [SerializeField] private TextMeshProUGUI _titlesText;
    [SerializeField] private string[] _titlesString;

    [Header("Tuto")] [SerializeField] private FB_Arrow _tutoArrow;

    // private TMP_Text _dialogText;
    private List<string> _dialogsList = new List<string>();
    private List<DialogPrefab> _dialogsPrefabList = new List<DialogPrefab>();
    private bool _isDialogTime;
    private bool _isTheEnd;
    private bool _isFirstScreen;

    private bool _isPaused;

    // private int _countScreen;
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

        FollowMouse.Instance.IsBlockMouse(true);

        SpawnNewDialogs(MapManager.Instance.GetDialogAtVictory(), true, false);
    }

    private void StartDialog()
    {
    }

    private void EndDialog()
    {
        // if (!MapManager.Instance.IsTuto)
        // _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();

        // RemoveLastDialog();

        FollowMouse.Instance.IsBlockMouse(false);

        _dialogParent.SetActive(false);

        _menuQuest.GetComponent<OpenCloseMenu>().OpenMenuQuest();
    }

    public void SpawnNewDialogs(string[] dialogs, bool isTheEnd, bool isMiddleDialog)
    {
        RemoveLastDialog();

        // Set isDialoging and Reset count old dialog
        _isDialogTime = true;
        _countDialog = 0;

        // Open Dialog Menu
        // _dialogParent.GetComponent<OpenCloseMenu>().OpenAnim();
        _dialogParent.SetActive(true);

        // Block mouse
        // if (!isMiddleDialog)
        FollowMouse.Instance.IsBlockMouse(true);

        // Clear two list of old dialogs
        if (_dialogsList.Count != 0)
            _dialogsList.Clear();

        if (_dialogsPrefabList.Count != 0)
            _dialogsPrefabList.Clear();

        // Add new string dialog
        foreach (var dialog in dialogs)
        {
            _dialogsList.Add(dialog);
        }

        // set if it's victory dialog
        _isTheEnd = isTheEnd;

        if (!isTheEnd)
            UpdateButtonGoLevelSupp(false);

        if (_dialogsList.Count == 0)
        {
            _isDialogTime = false;
            CheckIfEnd();
            return;
        }

        SpawnAllDialog();
    }

    private void SpawnAllDialog()
    {
        if (_dialogsPrefabList.Count != _dialogsList.Count)
            SpawnDialog();
        else
            print("all dialogs have spawned");
    }

    private void SpawnDialog()
    {
        if (_dialogsPrefabList.Count > 0)
            Destroy(_dialogsPrefabList[^1].gameObject);

        UpdateDialogFB(false);

        // Instantiate new dialog
        GameObject go = Instantiate(_dialogPrefab, _dialogContent.transform);
        go.transform.localPosition = Vector3.zero;

        var goDialog = go.GetComponent<DialogPrefab>();

        // Add it to the dialog prefab list
        _dialogsPrefabList.Add(goDialog);

        // Get the text 
        var newDialog = _dialogsList[_countDialog];
        // Security if no text
        if (newDialog == String.Empty)
            newDialog = " ";

        // Init to the dialog prefab with the speed spawn
        goDialog.Init(newDialog, _dialogSpeed);

        // Get Size of dialog prefab
        //float textSize = goDialog.GetDialogSizeY();
        // Increase Dialog size content
        //_dialogContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, textSize + SPACING_BETWEEN_TWO_DIALOG);

        //GoToBottomScrollBar();

        _countDialog++;
    }

    public void GoToBottomScrollBar()
    {
        _dialogScrollBar.value = 0;
    }

    public void GameOver()
    {
        _bg.SetActive(true);
        _gameOverParent.SetActive(true);
        FollowMouse.Instance.IsBlockMouse(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdatePause(!_isPaused);


        if (!_isDialogTime) return;


        if (Input.GetMouseButtonDown(0))
        {
            if (CheckIfDialogEnded())
                return;

            if (_dialogsPrefabList[^1].IsFinish)
            {
                SpawnAllDialog();
            }
            else
            {
                _dialogsPrefabList[^1].EndAnimationText();
                UpdateDialogFB(true);
            }


            // if (_countScreen < _dialogsList.Count * 2 - 1)
            // {
            //     if (_countScreen % 2 == 0)
            //     {
            //         _stopCorou = true;
            //         _dialogText.text = _dialogsList[_countDialog];
            //         StopCoroutine(UpdateText());
            //     }
            //     else
            //     {
            //         // print("anim");
            //         _countDialog++;
            //         //_dialogText.text = String.Empty;
            //         StartCoroutine(UpdateText());
            //     }
            //
            //     _countScreen++;
            // }
            // else
            // {
            //     if (_isBeginning)
            //         EndBeginningDialog();
            //     else
            //         ChangeToLevelSupp();
            // }
        }
    }

    public void UpdatePause(bool state)
    {
        _isPaused = state;
        _bg.SetActive(state);
        _menuPauseParent.SetActive(state);
        FollowMouse.Instance.IsBlockMouse(state);
        _menuPauseTriggered.GetComponent<OpenCloseMenu>().IsMenuPauseOpen = state;

        if (!state)
            _menuPauseTriggered.GetComponent<OpenCloseMenu>().ForcedOpen = false;
    }

    public void UpdateDialogFB(bool state)
    {
        _dialogFBEnd.SetActive(state);
    }

    public bool CheckIfDialogEnded()
    {
        if (_dialogsPrefabList.Count == _dialogsList.Count && _dialogsPrefabList[^1].IsFinish)
        {
            _isDialogTime = false;
            //GoToBottomScrollBar();

            CheckIfEnd();

            return true;
        }

        return false;
    }

    public void SkipDialog()
    {
        _dialogsPrefabList[^1].EndAnimationText();
        _isDialogTime = false;

        CheckIfEnd();
    }

    public void RestartSceneOrLevel()
    {
        _isDialogTime = false;
        // _countScreen = 0;
        _countDialog = 0;

        _bg.SetActive(false);
        _gameOverParent.SetActive(false);
        FollowMouse.Instance.IsBlockMouse(false);
        _menuPauseParent.SetActive(false);
        // _dialoguesParent.SetActive(false);
        // _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();
        _titlesParent.SetActive(false);
    }

    public void GoLevelSupp()
    {
        _isDialogTime = false;
        // _countScreen = 0;
        _countDialog = 0;

        // _dialoguesParent.SetActive(false);
        // _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();
        _bg.SetActive(false);
        _titlesParent.SetActive(false);
        MapManager.Instance.ResetAllMap(true);
        // StartCoroutine(UnlockMouse());
    }

    public void UpdateButtonGoLevelSupp(bool state)
    {
        _nextLevel.SetActive(state);
        _dialogParent.SetActive(!state);
    }

    public void CheckIfEnd()
    {
        if (_isTheEnd)
            UpdateButtonGoLevelSupp(true);
        else
            EndDialog();
    }

    private const float SPACING_BETWEEN_TWO_DIALOG = 18;

    // IEnumerator UpdateText()
    // {
    //     _isCorouRunning = true; 
    //     
    //     GameObject go = Instantiate(_dialogPrefab, _dialogContent.transform);
    //     var goDialog = go.GetComponent<DialogPrefab>();
    //
    //     var newDialog = _dialogsToDisplay[_countDialog];
    //
    //     if (newDialog == String.Empty)
    //         newDialog = " ";
    //
    //     goDialog.Init(newDialog, _dialogSpeed);
    //     _dialogText = goDialog.DialogText;
    //
    //     float textSize = goDialog.GetDialogSizeY();
    //     _dialogContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, textSize + SPACING_BETWEEN_TWO_DIALOG);
    //
    //     int charIndex = 0;
    //
    //     foreach (char c in newDialog.ToCharArray())
    //     {
    //         if (_stopCorou)
    //         {
    //             _stopCorou = false;
    //             yield break;
    //         }
    //
    //         _dialogScrollBar.value = 0;
    //
    //         charIndex++;
    //
    //         var firstText = newDialog.Substring(0, charIndex);
    //         var secondText = $"<color=#00000000>{newDialog.Substring(charIndex)}";
    //         _dialogText.text = firstText + secondText;
    //
    //         yield return new WaitForSeconds(_dialogSpeed);
    //     }
    //
    //     _countScreen++;
    //     _isCorouRunning = false;
    // }

    public void UpdateTutoArrow(bool state)
    {
        _tutoArrow.UpdateArrow(state);
    }

    public bool GetIsDialogTime()
    {
        return _isDialogTime;
    }

    private void RemoveLastDialog()
    {
        if (_dialogsPrefabList.Count > 0)
        {
            var txt = _dialogsPrefabList[^1];
            _dialogsPrefabList.Remove(txt);

            if (txt.gameObject != null)
                Destroy(txt.gameObject);
        }
    }
}