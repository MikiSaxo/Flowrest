using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable All

public class ScreensManager : MonoBehaviour
{
    public static ScreensManager Instance;

    [Header("Big Screens")] [SerializeField]
    private GameObject _bg;

    [SerializeField] private GameObject _victoryParent;
    [SerializeField] private GameObject _victoryConfettis;
    [SerializeField] private GameObject _defeatParent;
    [SerializeField] private GameObject _nextLevel;

    [Header("Pause")] [SerializeField] private GameObject _menuPauseParent;
    [SerializeField] private GameObject _menuOption;

    [Header("Order")] [SerializeField] private GameObject _orderMenu;
    [SerializeField] private GameObject _orderGrid;
    [SerializeField] private GameObject _orderPrefab;
    [SerializeField] private GameObject _orderTextGrid;
    [SerializeField] private GameObject _orderTextPrefab;
    [Space(15)] [SerializeField] private List<OrderText> _orderText;

    [Header("Tuto")] [SerializeField] private FB_Arrow _tutoArrow;

    [Header("Memo")] [SerializeField] private OpenCloseMenu _memoMenu;
    [SerializeField] private WaveEffect _memoWaveEffect;
    
    [Header("Credit - Outro")] 
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _outro;

    private bool _isTheEnd;
    private bool _isFirstScreen;
    public bool HasPopUp { get; set; }
    public bool IsMemoOpened { get; set; }

    private bool _isPaused;
    private bool _hasUnlockedMemo;
    private bool _hasMemoWaveEffect;

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

    public void InitOrderDescription(string text)
    {
        text ??= String.Empty;
        // img ??= null;

        GameObject txt = Instantiate(_orderTextPrefab, _orderTextGrid.transform);
        var desc = txt.GetComponent<DialogPrefab>();
        desc.InitDescOrder($"{text}\n ", true);
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

        if (LanguageManager.Instance.Tongue == Language.Francais)
        {
            var orderDesc = _orderText[whichOrder].OrderDescription[(int)whichState];
            if (orderDesc == null)
                orderDesc = " ";
            order.InitOrder($"{orderDesc}", nbToReach);
        }
        else
        {
            var orderDesc = _orderText[whichOrder].OrderDescriptionEnglish[(int)whichState];
            if (orderDesc == null)
                orderDesc = " ";
            order.InitOrder($"{orderDesc}", nbToReach);
        }

        // Image
        GameObject go = Instantiate(_orderPrefab, _orderGrid.transform);
        go.GetComponent<OrderStockSprite>().Init(whichOrder, whichState);
        _stockOrderImg.Add(go);

        StartCoroutine(UpdateGridText());
    }

    IEnumerator UpdateGridText()
    {
        yield return new WaitForSeconds(.2f);
        // Update the grid by force to actualise the grid because it's buggy??
        _orderTextGrid.GetComponent<VerticalLayoutGroup>().spacing = 1;
    }

    public void InitMaxNbFullFloor(int nb)
    {
        _stockOrderText[1].UpdateMaxNb(nb);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdatePause(!_isPaused);
        }
    }

    public void UpdateOrder(int newNb, int whichOrder)
    {
        _stockOrderText[whichOrder].UpdateCurrentNbOrder(newNb);
    }

    public void UpdatePopUpState(bool state)
    {
        PopUpManager.Instance.UpdatePopUpState(state);

        DialogManager.Instance.UpdateDialogGlobal(false);
        DialogManager.Instance.UpdateDialogBG(false);

        if (!state)
        {
            if (!MapManager.Instance.IsRestart)
                DialogManager.Instance.EndDialog();

            MapManager.Instance.IsRestart = false;
        }
    }

    public void UpdatePause(bool state)
    {
        _isPaused = state;
        _bg.SetActive(state);
        _menuPauseParent.SetActive(state);
        if (state)
        {
            _menuPauseParent.GetComponent<SpawnAnimButtons>().LaunchSpawnAnim();
            _menuPauseParent.GetComponent<PauseSpawn>().LaunchSpawnAnim();
        }

        if (!state)
            _menuOption.SetActive(false);

        if (!state && MapManager.Instance.IsVictory)
        {
            _victoryParent.SetActive(true);
            _victoryConfettis.SetActive(true);
            _victoryParent.GetComponent<VictoryAnim>().LaunchAnimVictory();
        }

        if (state)
            MapManager.Instance.IsOnUI = true;
        else
            MapManager.Instance.IsOnUI = false;

        if (DialogManager.Instance.IsDialogTime) return;

        StartCoroutine(WaitToUnlockMouse(state));
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
        // AudioManager.Instance.PlaySFX("Victory");

        MapManager.Instance.IsVictory = true;
        MapManager.Instance.ResetTwoLastSwapped();

        // Reset Wave Energy
        EnergyManager.Instance.StopWaveEffect();

        MapManager.Instance.IsOnUI = true;
        MouseHitRaycast.Instance.IsBlockMouse(true);


        var dialogOfEnd = DialogManager.Instance.DialogOfEnd;

        if (dialogOfEnd != null)
        {
            DialogManager.Instance.SpawnNewDialogs(dialogOfEnd, true, false);
            return;
        }

        AudioManager.Instance.PlaySFX("Victory");

        if (CheckIfEndGame())
        {
            print("it's end game");
            LaunchCredits();
            return;
        }

        if (!_isPaused)
        {
            _victoryParent.SetActive(true);
            _victoryConfettis.SetActive(true);
            _victoryParent.GetComponent<VictoryAnim>().LaunchAnimVictory();
        }


        UpdateButtonGoLevelSupp(true);

        // Update Save
        PlayerPrefs.SetString("CurrentDialogData", DialogManager.Instance.NextDialogToLoad.name);
        MapManager.Instance.CurrentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", MapManager.Instance.CurrentLevel);
        PlayerPrefs.Save();
    }

    public bool CheckIfEndGame()
    {
        if (DialogManager.Instance.WhichEnd != 0)
            return true;
        
        if (DialogManager.Instance.NoNextEndDialogChoice && DialogManager.Instance.NoNextEndDialog)
            return true;
        

        return false;
    }

    public void GameOver()
    {
        AudioManager.Instance.PlaySFX("Defeat");

        _bg.SetActive(true);
        _defeatParent.SetActive(true);
        _defeatParent.GetComponent<DefeatAnim>().LaunchAnimDefeat();
        MouseHitRaycast.Instance.IsBlockMouse(true);
    }

    IEnumerator WaitToUnlockMouse(bool state)
    {
        yield return new WaitForSeconds(.1f);
        if (state)
            MapManager.Instance.IsOnUI = true;
        MouseHitRaycast.Instance.IsBlockMouse(state);
    }

    public void RestartSceneOrLevel()
    {
        DialogManager.Instance.IsDialogTime = false;
        // DialogManager.Instance.ResetCountDialog();
        _bg.SetActive(false);
        MouseHitRaycast.Instance.IsBlockMouse(false);

        _menuPauseParent.SetActive(false);

        _victoryParent.GetComponent<VictoryAnim>().UpdateMainCanvasAlpha(1);

        _victoryParent.GetComponent<VictoryAnim>().ResetAnim();
        _victoryParent.SetActive(false);
        _victoryConfettis.SetActive(false);

        _defeatParent.GetComponent<DefeatAnim>().ResetAnim();
        _defeatParent.SetActive(false);
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
        if (MapManager.Instance.IsLoading) return;

        MapManager.Instance.IsLoading = false;
        MapManager.Instance.IsOnUI = false;

        StartCoroutine(WaitToGoLevelSupp());
    }

    IEnumerator WaitToGoLevelSupp()
    {
        MapManager.Instance.CurrentDialogData = DialogManager.Instance.NextDialogToLoad;

        TransiManager.Instance.LaunchGrownOn();
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());

        CloseMemo();

        DialogManager.Instance.IsDialogTime = false;
        DialogManager.Instance.ResetCountDialog();

        ResetOrder();

        _bg.SetActive(false);

        _victoryParent.GetComponent<VictoryAnim>().UpdateMainCanvasAlpha(1);
        _victoryParent.GetComponent<VictoryAnim>().ResetAnim();
        _victoryParent.SetActive(false);
        _victoryConfettis.SetActive(false);

        MapManager.Instance.ForceResetBig();
        MapManager.Instance.ResetAllMap();
    }

    public void LaunchOpenOrder()
    {
        StartCoroutine(WaitToOpenOrder());
    }

    IEnumerator WaitToOpenOrder()
    {
        yield return new WaitForSeconds(1f);
        _orderMenu.GetComponent<OpenCloseMenu>().OpenAnim();
        _orderMenu.gameObject.GetComponent<ButtonManager>().UpdateButton(0, true);
    }

    public void UpdateButtonGoLevelSupp(bool state)
    {
        _nextLevel.SetActive(state);
    }

    public void UpdateTutoArrowInventory(bool state)
    {
        _tutoArrow.UpdateArrow(state);
    }

    public void CloseCommandMenu()
    {
        _orderMenu.GetComponent<OpenCloseMenu>().CloseQuick();
        _orderMenu.GetComponent<ButtonManager>().UpdateButton(0, false);
    }


    public void LaunchCredits()
    {
        StartCoroutine(WaitToLaunchCredit());
    }

    IEnumerator WaitToLaunchCredit()
    {
        TransiManager.Instance.LaunchGrownOn();

        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());

        TransiManager.Instance.LaunchShrink();

        _credits.SetActive(true);
        
        if (DialogManager.Instance.WhichEnd == 1)
            _outro.gameObject.GetComponent<DialogPrefab>().InitWithoutAnim(LanguageManager.Instance.GetEndOneText());
        else if (DialogManager.Instance.WhichEnd == 2)
            _outro.gameObject.GetComponent<DialogPrefab>().InitWithoutAnim(LanguageManager.Instance.GetEndTwoText());
        else
            _outro.gameObject.SetActive(false);
        
        _credits.GetComponent<CreditsMovement>().Init();
    }

    public void CheckIfMemoOpen()
    {
        if (MapManager.Instance.OpenMemo)
            _hasUnlockedMemo = true;

        if (_hasUnlockedMemo && !IsMemoOpened)
        {
            // _memoMenu.OpenAnim();
            // _orderMenu.gameObject.GetComponent<ButtonManager>().UpdateButton(1, true);
            StartCoroutine(WaitToLaunchMemoOpening());
        }
    }

    IEnumerator WaitToLaunchMemoOpening()
    {
        if (IsMemoOpened) yield break;

        IsMemoOpened = true;
        
        yield return new WaitForSeconds(1f);
        
        _memoMenu.OpenAnim();
        _orderMenu.gameObject.GetComponent<ButtonManager>().UpdateButton(1, true);
        
        yield return new WaitForSeconds(.5f);
        
        if (!_hasMemoWaveEffect)
        {
            _memoWaveEffect.StartGrowOneTime();
            _hasMemoWaveEffect = true;
        }
    }

    public void CloseMemo()
    {
        _memoMenu.CloseQuick();
        _orderMenu.gameObject.GetComponent<ButtonManager>().UpdateButton(1, false);
    }
}