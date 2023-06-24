using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;


    [Space(5)] [Header("Character Dialogs")] [SerializeField]
    private TMP_Text _characterName;

    [SerializeField] private GameObject _characterObj;

    [Header("Dialogs")] [SerializeField] private GameObject _dialogGlobal;
    [SerializeField] private GameObject _dialogContent;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private GameObject _dialogFBEnd;
    [SerializeField] private GameObject _dialogBG;
    [SerializeField] private float _dialogSpeed = .01f;

    [Header("Dialogs Answer")] [SerializeField] private GameObject _dialogChoiceParent;
    [SerializeField] private GameObject _dialogChoicePrefab;
    
    [Header("Wave Effect")] 
    [SerializeField] private WaveEffect _waveOrder;
    [SerializeField] private WaveEffect _waveMemo;


    public bool IsDialogTime { get; set; }

    public DialogData NextDialogToLoad { get; private set; }
    public bool NoNextEndDialog { get; private set; }
    public bool NoNextEndDialogChoice { get; private set; }

    public DialogData DialogOfEnd { get; private set; }


    private List<string> _dialogsList = new List<string>();
    private List<DialogPrefab> _dialogsPrefabList = new List<DialogPrefab>();
    private int _countDialog;
    private bool _isDialogOfEnd;
    private Sprite[] _charaSprites;
    private string[] _charaNames;
    private DialogChoice[] _choices;
    private List<GameObject> _stockChoiceButtons = new List<GameObject>();
    private DialogData _currentDialogData;
    private LevelData _levelToLoad;
    private bool _hasMadeChoices;

    private void Awake()
    {
        Instance = this;
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

        if (_choices != null && _choices.Length > 0)
        {
            CheckIfDialogEnded();
        }
    }

    public void UpdateDialogGlobal(bool state)
    {
        _dialogGlobal.SetActive(state);
    }

    public void UpdateCharaName(string charaName)
    {
        if (charaName != String.Empty)
            _characterName.text = charaName;
    }

    private void UpdateLevelToLoad(LevelData level)
    {
        _levelToLoad = level;
    }

    private void UpdateButtonGoLevelSupp(bool state)
    {
        ScreensManager.Instance.UpdateButtonGoLevelSupp(state);
        _dialogGlobal.SetActive(!state);
        UpdateDialogBG(!state);
    }

    private void UpdateCharaSprite()
    {
        if (_charaSprites.Length > 0)
        {
            if (_charaSprites[_countDialog] != null)
            {
                _characterObj.GetComponent<CharaMovement>().UpdateChara(_charaSprites[_countDialog]);
            }
            else
            {
                _characterObj.GetComponent<CharaMovement>().UpdateChara(null);
            }
        }
    }

    public void SpawnNewDialogs(DialogData dialogData, bool isDialogOfEnd, bool hasPopUp)
    {
        // Activate BG
        UpdateDialogBG(true);

        // Set if it's victory dialog
        _isDialogOfEnd = isDialogOfEnd;

        if (!_isDialogOfEnd)
            TransiManager.Instance.LaunchShrink();

        _hasMadeChoices = false;

        // Init dialogs
        string[] charaNames = Array.Empty<string>();
        Sprite[] charaSprites = Array.Empty<Sprite>();
        string[] dialogsText = Array.Empty<string>();

        if (dialogData != null)
        {
            if (LanguageManager.Instance.Tongue == Language.Francais)
            {
                if (dialogData.DialogFrench != null && dialogData.DialogFrench.Length > 0)
                {
                    DialogCore[] dialogFrench = dialogData.DialogFrench;
                    charaNames = new string[dialogFrench.Length];
                    charaSprites = new Sprite[dialogFrench.Length];
                    dialogsText = new string[dialogFrench.Length];

                    for (int i = 0; i < dialogFrench.Length; i++)
                    {
                        charaNames[i] = dialogData.DialogFrench[i].CharacterName;
                        charaSprites[i] = dialogFrench[i].CharacterSprite;
                        dialogsText[i] = dialogFrench[i].Text;
                    }
                }
            }
            else
            {
                if (dialogData.DialogEnglish != null && dialogData.DialogEnglish.Length > 0)
                {
                    charaNames = new string[dialogData.DialogEnglish.Length];
                    charaSprites = new Sprite[dialogData.DialogEnglish.Length];
                    dialogsText = new string[dialogData.DialogEnglish.Length];

                    for (int i = 0; i < dialogData.DialogEnglish.Length; i++)
                    {
                        charaNames[i] = dialogData.DialogEnglish[i].CharacterName;
                        charaSprites[i] = dialogData.DialogEnglish[i].CharacterSprite;
                        dialogsText[i] = dialogData.DialogEnglish[i].Text;
                    }
                }
            }
        }
        
        if (dialogData != null)
        {
            _currentDialogData = dialogData;
            if (_currentDialogData.LevelToLoad != null)
                UpdateLevelToLoad(_currentDialogData.LevelToLoad);
        }

        if (dialogData != null && dialogData.name == "d_Niv1_1")
        {
            print("salut bogos");
            MapManager.Instance.InitFalseFloor();
        }

        // If has choices
        if (dialogData != null)
            _choices = dialogData.Choices;
        else
            _choices = null;

        RemoveLastDialog();

        // ScreensManager.Instance.LaunchOpenOrder();
        ScreensManager.Instance.HasPopUp = hasPopUp;

        // Set IsDialogTime and Reset count old dialog
        IsDialogTime = true;
        _countDialog = 0;
        ScreensManager.Instance.IsMemoOpened = false;

        // Open Dialog Menu
        _dialogGlobal.SetActive(true);
        _dialogGlobal.GetComponent<PointerMotion>().Bounce();

        // Block mouse
        MouseHitRaycast.Instance.IsBlockMouse(true);

        // Clear two list of old dialogs
        if (_dialogsList.Count != 0)
            _dialogsList.Clear();

        if (_dialogsPrefabList.Count != 0)
            _dialogsPrefabList.Clear();


        if (dialogsText.Length == 0 && !_isDialogOfEnd)
        {
            dialogsText = new[] { " " };
            IsDialogTime = false;
            ScreensManager.Instance.IsMemoOpened = true;
            CheckIfEndDialog();

            return;
        }

        // Add new string dialog
        foreach (var dialog in dialogsText)
        {
            _dialogsList.Add(dialog);
        }

        if (!_isDialogOfEnd)
        {
            UpdateButtonGoLevelSupp(false);
        }

        if (_dialogsList.Count == 0)
        {
            IsDialogTime = false;
            ScreensManager.Instance.IsMemoOpened = true;
            CheckIfEndDialog();
            return;
        }

        if (charaSprites != null)
        {
            _charaSprites = charaSprites;
        }

        if (charaNames != null)
        {
            _charaNames = charaNames;
        }

        NoNextEndDialogChoice = true;

        SpawnAllDialog();

        // Update if dialog of end 
        DialogOfEnd = dialogData.DialogOfEnd;

        AudioManager.Instance.PlaySFX("DialogPop");
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
        // Destroy the last Dialog
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
        UpdateCharaSprite();

        UpdateCharaName(_charaNames[_countDialog]);

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
        var charaSpriteName = String.Empty;
        
        if(_charaSprites[_countDialog] != null)
            charaSpriteName = _charaSprites[_countDialog].name;
        
        if (charaSpriteName != String.Empty)
        {
            if (charaSpriteName[0] == 'D' && charaSpriteName[1] == 'G')
                VisualModifier.Instance.UpdateCharacters(Characters.DG);
            else
                VisualModifier.Instance.UpdateCharacters(Characters.Profess);
        }
        else
            VisualModifier.Instance.UpdateCharacters(Characters.None);


        _countDialog++;
        
        if(_currentDialogData.name == "d_Niv1_5" && _countDialog == 4)
            _waveOrder.StartGrowOneTime();
        if(_currentDialogData.name == "d_Niv1_5" && _countDialog == 5)
            _waveMemo.StartGrowOneTime();
    }

    public void EndDialog()
    {
        _dialogGlobal.SetActive(false);
        UpdateDialogBG(false);

        CheckUpgrades();

        if (!_isDialogOfEnd)
        {
            MouseHitRaycast.Instance.IsBlockMouse(false);
            
            if (MapManager.Instance.IsFalseLevel)
            {
                MapManager.Instance.IsFalseLevel = false;
                MapManager.Instance.ResetFalseMap();
            }
            MapManager.Instance.LaunchCheckFileMap(_levelToLoad);

            if (_currentDialogData.NextLevelDialog != null)
            {
                NoNextEndDialog = false;
                NextDialogToLoad = _currentDialogData.NextLevelDialog;

                PlayerPrefs.SetString("CurrentDialogData", NextDialogToLoad.name);
            }
            else
            {
                NoNextEndDialog = true;
            }
        }
        else
        {
            ScreensManager.Instance.VictoryScreen();
        }
    }

    private void CheckUpgrades()
    {
        if (_currentDialogData.VisualUpgrades._Upgrades == Upgrades.Nothing) return;

        VisualModifier.Instance.UpdateUpgrades(_currentDialogData.VisualUpgrades._Upgrades);
    }
    private void CheckUpgradesChoice(Upgrades upgrade)
    {
        if (upgrade == Upgrades.Nothing) return;

        VisualModifier.Instance.UpdateUpgrades(upgrade);
    }

    public void OnClick()
    {
        if (CheckIfDialogEnded())
            return;

        AudioManager.Instance.PlaySFX("NextDialog");

        if (_dialogsPrefabList[^1].IsFinishDialoging)
        {
            SpawnAllDialog();
        }
        else
        {
            _dialogsPrefabList[^1].EndAnimationText();
            UpdateDialogFBEnd(true);
        }
    }

    private bool CheckIfDialogEnded()
    {
        if (_dialogsPrefabList.Count == _dialogsList.Count && _dialogsPrefabList[^1].IsFinishDialoging)
        {
            IsDialogTime = false;
            CheckIfEndDialog();

            return true;
        }

        return false;
    }

    public void CheckIfEndDialog()
    {
        if (_choices != null && _choices.Length > 0 &&
            (_currentDialogData.DialogEnglish.Length > 0 || _currentDialogData.DialogFrench.Length > 0) &&
            !_hasMadeChoices)
        {
            SpawnChoices();
            return;
        }

        if (_currentDialogData.AddDialog != null &&
            (_currentDialogData.DialogEnglish.Length > 0 || _currentDialogData.DialogFrench.Length > 0))
        {
            SpawnNewDialogs(_currentDialogData.AddDialog, false, false);
            return;
        }

        ScreensManager.Instance.CheckIfMemoOpen();


        if (_levelToLoad.PopUpInfos != null && _levelToLoad.PopUpInfos.Length > 0 && !_isDialogOfEnd)
        {
            ScreensManager.Instance.HasPopUp = true;
            PopUpManager.Instance.InitPopUp(_levelToLoad.PopUpInfos);
        }


        if (!ScreensManager.Instance.HasPopUp)
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

    private void SpawnChoices()
    {
        if (_stockChoiceButtons.Count > 0) return;

        if (_choices != null && _choices.Length > 0 &&
            (_currentDialogData.DialogEnglish.Length > 0 || _currentDialogData.DialogFrench.Length > 0))
        {
            _dialogChoiceParent.SetActive(true);
            _dialogChoiceParent.GetComponent<SpawnAnimButtons>().ClearButtonList();

            for (int i = 0; i < _choices.Length; i++)
            {
                GameObject go = Instantiate(_dialogChoicePrefab, _dialogChoiceParent.transform);
                go.GetComponent<DialogChoiceButton>().InitChoiceIndex(i);
                _stockChoiceButtons.Add(go);
                _dialogChoiceParent.GetComponent<SpawnAnimButtons>().AddToButtonList(go);

                if (LanguageManager.Instance.Tongue == Language.Francais)
                    go.GetComponent<DialogPrefab>().Init(_choices[i].Choice, 0);
                else
                    go.GetComponent<DialogPrefab>().Init(_choices[i].ChoiceEnglish, 0);
            }

            _dialogChoiceParent.GetComponent<SpawnAnimButtons>().LaunchSpawnAnim();
        }
    }

    public void MakeAChoice(int index)
    {
        _hasMadeChoices = true;

        CheckUpgradesChoice(_choices[index].VisualUpgrades._Upgrades);

        if (_choices[index].LevelToLoad != null)
        {
            // ScreensManager.Instance.NewLevelData = _choices[index].NextLevelNoDialog;
            // UpdateLevelToLoad(_choices[index].NextLevelNoDialog);
            print("update le niveau to load");
            _levelToLoad = _choices[index].LevelToLoad;
            // MapManager.Instance.UpdateLevelToLoad(NextLevelToLoad);
        }

        if (_choices[index].NextLevelDialog != null)
        {
            print("ça update le choice");
            NoNextEndDialogChoice = false;
            NextDialogToLoad = _choices[index].NextLevelDialog;
            // if (LevelProgressionManager.Instance != null)
            //     LevelProgressionManager.Instance.CurrentDialogData = NextDialogToLoad;
            PlayerPrefs.SetString("CurrentDialogData", NextDialogToLoad.name);
        }
        else
        {
            NoNextEndDialogChoice = true;
        }

        SpawnNewDialogs(_choices[index].AddDialog, false, false);

        _dialogChoiceParent.SetActive(false);
        if (_stockChoiceButtons.Count > 0)
        {
            foreach (var choiceBut in _stockChoiceButtons)
            {
                Destroy(choiceBut);
            }

            _stockChoiceButtons.Clear();
        }
    }

    public void SkipDialog()
    {
        _dialogsPrefabList[^1].EndAnimationText();
        IsDialogTime = false;

        CheckIfEndDialog();
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

    public DialogData GetCurrentDialogData()
    {
        return _currentDialogData;
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