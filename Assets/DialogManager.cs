using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;


    [Space(5)] [Header("Character Dialogs")] [SerializeField]
    private TMP_Text _characterName;

    [SerializeField] private Image _characterImg;

    [Header("Dialogs")] [SerializeField] private GameObject _dialogParent;
    [SerializeField] private GameObject _dialogContent;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private GameObject _dialogFBEnd;
    [SerializeField] private GameObject _dialogBG;
    [SerializeField] private float _dialogSpeed = .01f;

    [Header("Dialogs Answer")] [SerializeField]
    private GameObject _dialogChoiceParent;

    [SerializeField] private GameObject _dialogChoicePrefab;

    [Header("Dialogs Anim")] [SerializeField]
    private GameObject _dialogGlobal;

    [SerializeField] private float _punchPower;
    [SerializeField] private float _punchDuration;
    [SerializeField] private int _punchVibrato;

    public bool IsDialogTime { get; set; }

    private List<string> _dialogsList = new List<string>();
    private List<DialogPrefab> _dialogsPrefabList = new List<DialogPrefab>();
    private int _countDialog;
    private bool _isTheEnd;
    private Sprite[] _charaSprites;


    private void Awake()
    {
        Instance = this;
    }

    public void InitCharaName(string charaName)
    {
        _characterName.text = charaName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }
    }

    public void UpdateDialogBG(bool state)
    {
        _dialogBG.SetActive(state);
    }

    public void UpdateDialogFBEnd(bool state)
    {
        _dialogFBEnd.SetActive(state);
    }

    public void UpdateDialogGlobal(bool state)
    {
        _dialogGlobal.SetActive(state);
    }

    public void SpawnNewDialogs(DialogData _dialogData, bool isTheEnd, bool hasPopUp)
    {
        Sprite[] chara = Array.Empty<Sprite>();
        string[] dialogsText = Array.Empty<string>();

        if (_dialogData != null)
        {
            if (LanguageManager.Instance.Tongue == Language.Francais)
            {
                if (_dialogData.CoreDialogFrench != null && _dialogData.CoreDialogFrench.Length > 0)
                {
                    chara = new Sprite[_dialogData.CoreDialogFrench.Length];
                    dialogsText = new string[_dialogData.CoreDialogFrench.Length];

                    for (int i = 0; i < _dialogData.CoreDialogFrench.Length; i++)
                    {
                        chara[i] = _dialogData.CoreDialogFrench[i].CharacterSprites;
                        dialogsText[i] = _dialogData.CoreDialogFrench[i].CoreDialog;
                    }
                }
            }
            else
            {
                if (_dialogData.CoreDialogEnglish != null && _dialogData.CoreDialogEnglish.Length > 0)
                {
                    chara = new Sprite[_dialogData.CoreDialogEnglish.Length];
                    dialogsText = new string[_dialogData.CoreDialogEnglish.Length];

                    for (int i = 0; i < _dialogData.CoreDialogEnglish.Length; i++)
                    {
                        chara[i] = _dialogData.CoreDialogEnglish[i].CharacterSprites;
                        dialogsText[i] = _dialogData.CoreDialogEnglish[i].CoreDialog;
                    }
                }
            }
        }

        RemoveLastDialog();

        ScreensManager.Instance._hasPopUp = hasPopUp;

        // Set IsDialogTime and Reset count old dialog
        IsDialogTime = true;
        _countDialog = 0;
        ScreensManager.Instance._isMemoOpened = false;

        // Open Dialog Menu
        _dialogGlobal.SetActive(true);
        _dialogGlobal.transform.DOPunchScale(Vector3.one * _punchPower, _punchDuration, _punchVibrato);

        // Block mouse
        MouseHitRaycast.Instance.IsBlockMouse(true);

        // Clear two list of old dialogs
        if (_dialogsList.Count != 0)
            _dialogsList.Clear();

        if (_dialogsPrefabList.Count != 0)
            _dialogsPrefabList.Clear();

        // set if it's victory dialog
        _isTheEnd = isTheEnd;

        if (dialogsText.Length == 0 && !isTheEnd)
        {
            dialogsText = new[] { " " };
            IsDialogTime = false;
            ScreensManager.Instance._isMemoOpened = true;
            CheckIfEnd();
            return;
        }

        // Add new string dialog
        foreach (var dialog in dialogsText)
        {
            _dialogsList.Add(dialog);
        }

        if (!isTheEnd)
        {
            UpdateButtonGoLevelSupp(false);
        }

        if (_dialogsList.Count == 0)
        {
            IsDialogTime = false;
            ScreensManager.Instance._isMemoOpened = true;
            CheckIfEnd();
            return;
        }

        ScreensManager.Instance.LaunchOpenOrder();

        if (chara != null)
        {
            _charaSprites = chara;
        }

        SpawnAllDialog();

        AudioManager.Instance.PlaySFX("DialogPop");
    }

    private void UpdateButtonGoLevelSupp(bool state)
    {
        ScreensManager.Instance.UpdateButtonGoLevelSupp(state);
        _dialogGlobal.SetActive(!state);
        UpdateDialogBG(!state);
    }

    private void SpawnAllDialog()
    {
        if (_dialogsPrefabList.Count != _dialogsList.Count)
        {
            SpawnDialog();
        }
        else
            print("all dialogs have spawned");
    }

    private void SpawnDialog()
    {
        if (_dialogsPrefabList.Count > 0)
            Destroy(_dialogsPrefabList[^1].gameObject);

        UpdateDialogFBEnd(false);

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

        // Change Chara
        if (_charaSprites.Length > 0)
        {
            if (_countDialog < _charaSprites.Length && _charaSprites[_countDialog] != null)
            {
                _characterImg.sprite = _charaSprites[_countDialog];
            }
        }

        // Init to the dialog prefab with the speed spawn
        if (MapManager.Instance.IsAndroid)
            goDialog.Init(newDialog, _dialogSpeed * 2);
        else
            goDialog.Init(newDialog, _dialogSpeed);

        // Get Size of dialog prefab
        //float textSize = goDialog.GetDialogSizeY();
        // Increase Dialog size content
        //_dialogContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, textSize + SPACING_BETWEEN_TWO_DIALOG);

        //GoToBottomScrollBar();

        _countDialog++;
    }

    private void EndDialog()
    {
        MouseHitRaycast.Instance.IsBlockMouse(false);

        _dialogGlobal.SetActive(false);
        UpdateDialogBG(false);

        // _orderMenu.GetComponent<MenuOrderMemoManager>().OnActivateOrder();

        MapManager.Instance.ActivateArrowIfForceSwap();

        if (MapManager.Instance.HasInventory)
            SetupUIGround.Instance.UpdateOpacityInventory(1);
    }

    public void OnClick()
    {
        if (CheckIfDialogEnded())
            return;

        AudioManager.Instance.PlaySFX("NextDialog");

        if (_dialogsPrefabList[^1].IsFinish)
        {
            SpawnAllDialog();
        }
        else
        {
            _dialogsPrefabList[^1].EndAnimationText();
            UpdateDialogFBEnd(true);
        }
    }

    public bool CheckIfDialogEnded()
    {
        if (_dialogsPrefabList.Count == _dialogsList.Count && _dialogsPrefabList[^1].IsFinish)
        {
            IsDialogTime = false;
            //GoToBottomScrollBar();

            CheckIfEnd();

            return true;
        }

        return false;
    }

    public void CheckIfEnd()
    {
        if (_isTheEnd)
            UpdateButtonGoLevelSupp(true);
        else
        {
            ScreensManager.Instance.CheckIfMemoOpen();

            if (!ScreensManager.Instance._hasPopUp)
            {
                EndDialog();
                if (!MapManager.Instance.IsTuto)
                    StartCoroutine(ResetAfterSkipDialog());
            }
            else
            {
                ScreensManager.Instance.UpdatePopUpState(true);
            }
        }
    }

    public void SkipDialog()
    {
        _dialogsPrefabList[^1].EndAnimationText();
        IsDialogTime = false;

        CheckIfEnd();
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

    public bool GetIsDialogTime()
    {
        return IsDialogTime;
    }

    public void ResetCountDialog()
    {
        _countDialog = 0;
    }

    IEnumerator ResetAfterSkipDialog()
    {
        yield return new WaitForSeconds(.001f);
        MapManager.Instance.ForceResetBig();
    }
}