using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Big Screens")] [SerializeField]
    private GameObject _bg;

    [SerializeField] private GameObject _titlesParent;
    [SerializeField] private GameObject _gameOverParent;
    [SerializeField] private GameObject _nextLevel;

    [Header("Pause")] [SerializeField] private Button _backwardsButton;

    [Header("Pause")] [SerializeField] private GameObject _menuPauseParent;
    [SerializeField] private GameObject _menuPauseTriggered;

    [Header("Order")] [SerializeField] private GameObject _orderMenu;
    [SerializeField] private GameObject _orderGrid;
    [SerializeField] private GameObject _orderPrefab;
    [SerializeField] private GameObject _orderTextGrid;
    [SerializeField] private GameObject _orderTextPrefab;
    [Space(15)] [SerializeField] private List<OrderText> _orderText;

    [Space(5)] [Header("Dialogs")] [SerializeField]
    private GameObject _dialogParent;

    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private GameObject _dialogContent;
    [SerializeField] private Scrollbar _dialogScrollBar;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private GameObject _dialogFBEnd;
    [SerializeField] private float _dialogSpeed = .01f;

    // [Header("Titles")] [SerializeField] private TextMeshProUGUI _titlesText;
    // [SerializeField] private string[] _titlesString;

    [Header("Tuto")] [SerializeField] private FB_Arrow _tutoArrow;

    // private TMP_Text _dialogText;
    private List<string> _dialogsList = new List<string>();
    private List<DialogPrefab> _dialogsPrefabList = new List<DialogPrefab>();
    private bool _isDialogTime;
    private bool _isTheEnd;
    private bool _isFirstScreen;
    private bool _hasPopUp;

    private bool _isPaused;

    // private int _countScreen;
    private int _countDialog;
    private bool _stopCorou;
    private bool _isCorouRunning;
    private bool _hasSpawnDialog;
    private string _saveSpawnDialog;
    private List<DialogPrefab> _stockOrderText = new List<DialogPrefab>();
    private Dictionary<AllStates, DialogPrefab> _stockOrderMultipleText = new Dictionary<AllStates, DialogPrefab>();
    private List<GameObject> _stockOrderImg = new List<GameObject>();
    private AllStates _saveLastState;
    private int _saveLastNbToReach;

    private void Awake()
    {
        Instance = this;
        _saveLastState = AllStates.None;
    }

    private void Start()
    {
        //FirstScreenOfLevel();
    }

    public void InitCharaName(string charaName)
    {
        _characterName.text = charaName;
    }

    public void InitOrderDescription(string text)
    {
        text ??= String.Empty;
        // img ??= null;

        GameObject txt = Instantiate(_orderTextPrefab, _orderTextGrid.transform);
        var desc = txt.GetComponent<DialogPrefab>();
        desc.InitDescOrder($"{text}\n ");
        _stockOrderText.Add(desc);

        // _descriptionQuest.text = text;
    }

    public void InitOrderGoal(int whichOrder, AllStates whichState, int nbToReach, bool isMultiple)
    {
        // Text
        GameObject txt = Instantiate(_orderTextPrefab, _orderTextGrid.transform);
        var order = txt.GetComponent<DialogPrefab>();

        _orderTextGrid.GetComponent<VerticalLayoutGroup>().spacing = 0;

        if (_saveLastState == whichState)
        {
            // print("salut");
            if (isMultiple)
            {
                var dialog = _stockOrderMultipleText.Last();
                Destroy(dialog.Value.gameObject);
                _stockOrderMultipleText.Remove(_stockOrderMultipleText.Keys.Last());
            }
            else
            {
                Destroy(_stockOrderText[^1].gameObject);
                _stockOrderText.RemoveAt(_stockOrderText.Count - 1);
            }

            if (_stockOrderImg.Count > 0)
            {
                Destroy(_stockOrderImg[^1]);
                _stockOrderImg.RemoveAt(_stockOrderImg.Count - 1);
            }

            _saveLastNbToReach++;
            nbToReach = _saveLastNbToReach;
        }
        else
            _saveLastNbToReach = 1;

        _saveLastState = whichState;


        if (isMultiple)
            _stockOrderMultipleText.Add(whichState, order);
        else
            _stockOrderText.Add(order);

        order.InitOrder(
            LanguageManager.Instance.Tongue == Language.Francais
                ? $"{_orderText[whichOrder].OrderDescription[(int)whichState]}"
                : $"{_orderText[whichOrder].OrderDescriptionEnglish[(int)whichState]}", nbToReach);

        // Image
        GameObject go = Instantiate(_orderPrefab, _orderGrid.transform);
        go.GetComponent<OrderStockSprite>().Init(whichOrder, whichState);
        _stockOrderImg.Add(go);

        StartCoroutine(UpdateGridText());
    }

    IEnumerator UpdateGridText()
    {
        yield return new WaitForSeconds(.1f);
        // Update the grid by force to actualise the grid because it's buggy??
        _orderTextGrid.GetComponent<VerticalLayoutGroup>().spacing = 1;
    }

    public void InitMaxNbFullFloor(int nb)
    {
        _stockOrderText[1].UpdateMaxNb(nb);
    }

    public void UpdateOrder(int newNb, int whichOrder)
    {
        _stockOrderText[whichOrder].UpdateCurrentNbOrder(newNb);
    }

    public void UpdateMultipleOrder(AllStates whichOrder, int newNb)
    {
        _stockOrderMultipleText[whichOrder].UpdateCurrentNbOrder(newNb);
    }

    public void AddNewMultipleOrder(AllStates whichOrder, int nbToAdd)
    {
        _stockOrderMultipleText[whichOrder].AddNewNbOrder(nbToAdd);
    }

    public void ChangeSizeGridOrder(Vector2 newSize)
    {
        _orderGrid.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }

    public void VictoryScreen()
    {
        MapManager.Instance.IsVictory = true;

        // Reset Wave Energy
        EnergyManager.Instance.StopWaveEffect();

        _titlesParent.SetActive(true);
        // _titlesText.text = _titlesString[0];

        MouseHitRaycast.Instance.IsBlockMouse(true);

        SpawnNewDialogs(MapManager.Instance.GetDialogAtVictory(), true, false);
    }

    private void EndDialog()
    {
        MouseHitRaycast.Instance.IsBlockMouse(false);

        _dialogParent.SetActive(false);

        _orderMenu.GetComponent<OpenCloseMenu>().OpenMenuQuest();
    }

    public void UpdatePopUp(bool state)
    {
        PopUpManager.Instance.UpdatePopUp(state);

        _dialogParent.SetActive(false);
        _hasPopUp = false;

        if (!state)
        {
            CheckIfEnd();
        }
    }

    public void SpawnNewDialogs(string[] dialogs, bool isTheEnd, bool hasPopUp)
    {
        RemoveLastDialog();

        _hasPopUp = hasPopUp;

        // Set isDialoging and Reset count old dialog
        _isDialogTime = true;
        _countDialog = 0;

        // Open Dialog Menu
        _dialogParent.SetActive(true);

        // Block mouse
        MouseHitRaycast.Instance.IsBlockMouse(true);

        // Clear two list of old dialogs
        if (_dialogsList.Count != 0)
            _dialogsList.Clear();

        if (_dialogsPrefabList.Count != 0)
            _dialogsPrefabList.Clear();

        if (dialogs.Length == 0 && !isTheEnd)
            dialogs = new[] { " " };

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

    // public void GoToBottomScrollBar()
    // {
    //     _dialogScrollBar.value = 0;
    // }

    public void GameOver()
    {
        _bg.SetActive(true);
        _gameOverParent.SetActive(true);
        MouseHitRaycast.Instance.IsBlockMouse(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdatePause(!_isPaused);


        if (!_isDialogTime) return;


        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (CheckIfDialogEnded())
        //         return;
        //
        //     if (_dialogsPrefabList[^1].IsFinish)
        //     {
        //         SpawnAllDialog();
        //     }
        //     else
        //     {
        //         _dialogsPrefabList[^1].EndAnimationText();
        //         UpdateDialogFB(true);
        //     }


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
        //}
    }

    public void OnClick()
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
    }

    public void UpdatePause(bool state)
    {
        _isPaused = state;
        _bg.SetActive(state);
        _menuPauseParent.SetActive(state);
        MouseHitRaycast.Instance.IsBlockMouse(state);
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

    IEnumerator ResetAfterSkipDialog()
    {
        yield return new WaitForSeconds(.001f);
        MapManager.Instance.ForceResetBig();
    }

    public void RestartSceneOrLevel()
    {
        _isDialogTime = false;
        // _countScreen = 0;
        _countDialog = 0;

        _bg.SetActive(false);
        _gameOverParent.SetActive(false);
        MouseHitRaycast.Instance.IsBlockMouse(false);

        _menuPauseParent.SetActive(false);
        // _dialoguesParent.SetActive(false);
        // _dialogParent.GetComponent<OpenCloseMenu>().CloseAnim();
        _titlesParent.SetActive(false);
        ResetOrder();
    }

    private void ResetOrder()
    {
        foreach (var order in _stockOrderText)
        {
            Destroy(order.gameObject);
        }

        foreach (var order in _stockOrderImg)
        {
            Destroy(order);
        }

        foreach (var order in _stockOrderMultipleText)
        {
            Destroy(order.Value.gameObject);
        }

        _stockOrderText.Clear();
        _stockOrderImg.Clear();
        _stockOrderMultipleText.Clear();

        _saveLastState = AllStates.None;
    }

    public void ResetMultiplestock()
    {
        foreach (var text in _stockOrderMultipleText)
        {
            text.Value.UpdateCurrentNbOrder(0);
        }
    }

    public void GoLevelSupp()
    {
        MapManager.Instance.ForceResetBig();
        StartCoroutine(WaitToGoLevelSupp());
    }

    IEnumerator WaitToGoLevelSupp()
    {
        TransiManager.Instance.LaunchGrownOn();
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());

        _isDialogTime = false;
        // _countScreen = 0;
        _countDialog = 0;

        ResetOrder();
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
        {
            if (!_hasPopUp)
            {
                EndDialog();
                if (!MapManager.Instance.IsTuto)
                    StartCoroutine(ResetAfterSkipDialog());
            }
            else
            {
                UpdatePopUp(true);
            }
        }
    }

    public void UpdateTutoArrow(bool state)
    {
        _tutoArrow.UpdateArrow(state);
    }

    public void UpdateBackwardsButton(bool state)
    {
        _backwardsButton.interactable = state;
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