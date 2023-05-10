using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEditor;


public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public event Action UpdateGround;

    // public event Action CheckBiome;
    public event Action ResetSelection;

    public AllStates LastStateButtonSelected { get; set; }
    public GameObject LastObjButtonSelected { get; set; }
    public bool IsGroundFirstSelected { get; set; }
    public bool IsVictory { get; set; }
    public QuestManager QuestsManager { get; private set; }
    public int NbOfRecycling { get; set; }
    public bool IsSwapping { get; private set; }
    public bool IsPosing { get; set; }
    public bool IsLoading { get; set; }
    public bool IsOnUI { get; set; }
    public bool IsTuto { get; set; }
    public bool IsPlayerForcePoseBlocAfterSwap { get; private set; }


    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;
    [SerializeField] private float _distance;
    [SerializeField] private float _timeToSwap;
    [SerializeField] private float _timeToSpawnMap;

    // [Header("Level")] [SerializeField] private string _levelName;
    // [SerializeField] private string[] _lvlDataName;

    [Header("Data")] [SerializeField] private LevelData[] _levelData;

    private bool _hasInventory;
    private bool _hasRecycling;
    private bool _hasInfinitRecycling;
    private bool _hasPrevisu;
    private bool _blockLastGroundsSwapped;
    private bool _isPlayerForceSwap;
    private bool _hasFirstSwap;
    private List<Vector2Int> _stockPlayerForceSwap = new List<Vector2Int>();
    private bool _isDragNDrop;
    private bool _wantToRecycle;
    private int _currentLevel;
    private string[] _mapInfo;
    private string[] _previewMessageTuto;
    private bool _isFullFloorOrder;

    // private AllStates[,] _currentStateMap;
    // private List<AllStates[,]> _stockStateMap = new List<AllStates[,]>();
    // private List<bool[,]> _stockCrystals = new List<bool[,]>();
    // private List<int> _stockEnergy = new List<int>();
    // private List<GroundStateManager[]> _stockLastGroundSwaped = new List<GroundStateManager[]>();
    // private List<List<int>> _stockTileButtonTest = new List<List<int>>();
    // private List<int> _stockNbRecycle = new List<int>();

    private Vector2Int _mapSize;
    private Vector2Int _lastGroundCoordsSelected;
    private Vector2Int _coordsForcePoseBloc;
    private GameObject[,] _mapGrid;
    private GameObject _lastGroundSelected;
    private Image _recycleImg;
    private AllStates _secondLastGroundSelected;
    private int _countNbOfTile;

    private MapConstructData _mapConstructData;

    private GroundStateManager[] _lastGroundSwapped = new GroundStateManager[2];

    private Dictionary<char, AllStates> dico = new Dictionary<char, AllStates>();

    #region AllStateConst

    private const char NONE = 'N';
    private const char PLAIN = 'P';
    private const char DESERT = 'D';
    private const char WATER = 'W';
    private const char TROPICAL = 'T';
    private const char SAVANNA = 'S';
    private const char GEYSER = 'H';
    private const char SNOW = 'G';
    private const char POLAR_DESERT = 'O';
    private const char TUNDRA = 'U';
    private const char SWAMP = 'A';
    private const char MOUNTAIN = 'M';

    #endregion

    private const float QUARTER_OFFSET = .85f;
    private const float HALF_OFFSET = .5f;

    private void Awake()
    {
        Instance = this;
        QuestsManager = GetComponent<QuestManager>();
    }

    private void Start()
    {
        dico.Add(NONE, AllStates.None);
        dico.Add(PLAIN, AllStates.Plain);
        dico.Add(DESERT, AllStates.Desert);
        dico.Add(WATER, AllStates.Water);
        dico.Add(TROPICAL, AllStates.Tropical);
        dico.Add(SAVANNA, AllStates.Savanna);
        dico.Add(GEYSER, AllStates.Geyser);
        dico.Add(SNOW, AllStates.Snow);
        dico.Add(POLAR_DESERT, AllStates.PolarDesert);
        dico.Add(TUNDRA, AllStates.Tundra);
        dico.Add(SWAMP, AllStates.Swamp);
        dico.Add(MOUNTAIN, AllStates.Mountain);

        InitializeMap();
        LastStateButtonSelected = AllStates.None;
    }

    private void InitializeMap()
    {
        // var mapName = $"{_levelName}{_currentLevel}";
        // var mapName = _levelData[_currentLevel].LevelName;
        var mapNameJson = _levelData[_currentLevel].LevelName;
        var mapFolderName = _levelData[_currentLevel].LevelFolder;
        var currentLvl = _levelData[_currentLevel];

        // Get the text map
        string mapPath = Application.streamingAssetsPath + $"/{mapFolderName}/{mapNameJson}.txt";
        var lineJson = File.ReadAllText(mapPath);
        _mapConstructData = JsonUtility.FromJson<MapConstructData>(lineJson);
        _mapInfo = _mapConstructData.Map.Split("\n");
        // _mapGridAllMove.Add(_mapInfo);

        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;
        
        // Init the grids
        _mapGrid = new GameObject[_mapSize.x, _mapSize.y];

        // Init Start energy
        EnergyManager.Instance.InitEnergy(currentLvl.EnergyAtStart);

        // Update if has inventory
        _hasInventory = currentLvl.HasInventory;
        SetupUIGround.Instance.UpdateInventory(_hasInventory);

        // Update if tile at start
        if (currentLvl.StartNbAllState != null && _hasInventory)
        {
            for (int i = 0; i < currentLvl.StartNbAllState.Length; i++)
            {
                for (int j = 0; j < currentLvl.StartNbAllState[i]; j++)
                {
                    SetupUIGround.Instance.AddNewGround(i, true);
                }
            }
        }

        // Update if has recycling
        _hasRecycling = currentLvl.HasRecycling;
        NbOfRecycling = currentLvl.NbOfRecycling;
        _hasInfinitRecycling = currentLvl.HasInfinitRecycling;
        SetupUIGround.Instance.SetIfHasRecycling(_hasRecycling);
        RecyclingManager.Instance.UpdateRecycling(_hasRecycling);
        if (_hasRecycling)
            RecyclingManager.Instance.InitNbRecycling(_hasInfinitRecycling);

        // Reset Wave Energy
        EnergyManager.Instance.StopWaveEffect();

        // Update if has Previsu
        _hasPrevisu = currentLvl.HasPrevisu;

        // Update if bloc last grounds swapped
        _blockLastGroundsSwapped = currentLvl.BlockLastSwap;

        // Update if tuto
        IsTuto = currentLvl.IsTuto;
        SetupUIGround.Instance.SetActiveBackwardsButton(!IsTuto);
        if (IsTuto)
        {
            // Set Preview message
            _previewMessageTuto = LanguageManager.Instance.Tongue == Language.Francais
                ? currentLvl.PreviewMessage
                : currentLvl.PreviewMessageEnglish;

            // Update if force 2 first bloc swap
            if (currentLvl.PlayerForceSwap != null)
            {
                if (currentLvl.PlayerForceSwap.Length != 0)
                {
                    _hasFirstSwap = false;
                    _isPlayerForceSwap = true;
                    _stockPlayerForceSwap.Clear();
                    _stockPlayerForceSwap.Add(currentLvl.PlayerForceSwap[0]);
                    _stockPlayerForceSwap.Add(currentLvl.PlayerForceSwap[1]);
                }

                if (_hasInventory)
                {
                    IsPlayerForcePoseBlocAfterSwap = currentLvl.HasForcePoseBlocAfterSwap;
                    _coordsForcePoseBloc = currentLvl.ForcePoseBlocCoord;
                }
                else
                {
                    IsPlayerForcePoseBlocAfterSwap = false;
                }
            }
        }
        else
        {
            _hasFirstSwap = true;
            _isPlayerForceSwap = false;
            IsPlayerForcePoseBlocAfterSwap = false;
            _stockPlayerForceSwap.Clear();
        }

        // Reset Order Number
        QuestsManager.ResetQuestNumbers();

        // Update description Order
        ScreensManager.Instance.InitOrderDescription(LanguageManager.Instance.Tongue == Language.Francais
            ? currentLvl.QuestDescription
            : currentLvl.QuestDescriptionEnglish);

        // Update if full floor quest
        if (currentLvl.QuestFloor.Length > 0)
        {
            QuestsManager.InitQuestFullFloor(currentLvl.QuestFloor[0]);
            _isFullFloorOrder = true;
            // Update Order Description
            ScreensManager.Instance.InitOrderGoal(0, currentLvl.QuestFloor[0], 0, false);
        }

        // Update if flower quest
        if (currentLvl.QuestFlower.Length > 0)
        {
            QuestsManager.InitQuestFlower(currentLvl.QuestFlower);

            // Check Nb of different state
            AllStates lastState = AllStates.None;
            var count = 0;
            foreach (var state in currentLvl.QuestFlower)
            {
                if (lastState != state)
                    count++;

                lastState = state;
            }

            if (count == 2)
                ScreensManager.Instance.ChangeSizeGridOrder(new Vector2(150, 150));
            if (count >= 3)
                ScreensManager.Instance.ChangeSizeGridOrder(new Vector2(125, 125));

            // Update Order Description
            for (int i = 0; i < currentLvl.QuestFlower.Length; i++)
            {
                ScreensManager.Instance.InitOrderGoal(1, currentLvl.QuestFlower[i], 1, true);
            }
        }

        // Update if No Specific Tile quest
        if (currentLvl.QuestNoSpecificTiles.Length > 0)
        {
            QuestsManager.InitQuestNoSpecificTiles(currentLvl.QuestNoSpecificTiles);

            // Update Order Description
            ScreensManager.Instance.InitOrderGoal(2, currentLvl.QuestNoSpecificTiles[0], 99, false);
        }

        // Update if Tile Chain quest
        if (currentLvl.QuestTileChain != null)
        {
            if (currentLvl.QuestTileChain.Length > 0)
            {
                QuestsManager.InitQuestTileChain(currentLvl.QuestTileChain[0], currentLvl.NumberTileChain);

                // Update Order Description
                ScreensManager.Instance.InitOrderGoal(3, currentLvl.QuestTileChain[0], currentLvl.NumberTileChain,
                    false);
            }
        }

        // Update if Tile Count
        if (currentLvl.QuestTileCount != null)
        {
            if (currentLvl.QuestTileCount.Length > 0)
            {
                QuestsManager.InitQuestTileCount(currentLvl.QuestTileCount[0], currentLvl.NumberTileCount);

                // Update Order Description
                ScreensManager.Instance.InitOrderGoal(4, currentLvl.QuestTileCount[0], currentLvl.NumberTileCount,
                    false);
            }
        }

        // Update if PopUp
        if (currentLvl.PopUpImages is { Length: > 0 }) PopUpManager.Instance.InitPopUp(currentLvl.PopUpImages);

        // Update Dialogs
        ScreensManager.Instance.SpawnNewDialogs(LanguageManager.Instance.Tongue == Language.Francais
            ? _levelData[_currentLevel].DialogBeginning
            : _levelData[_currentLevel].DialogBeginningEnglish, false, currentLvl.PopUpImages.Length > 0);

        if (_levelData[_currentLevel].CharacterName != String.Empty)
            ScreensManager.Instance.InitCharaName(_levelData[_currentLevel].CharacterName);


        // Init Level
        InitializeFloor(_mapSize);
    }

    private void InitializeFloor(Vector2Int sizeMap)
    {
        StartCoroutine(FloorSpawnTiming(sizeMap));
    }

    IEnumerator FloorSpawnTiming(Vector2Int sizeMap)
    {
        IsLoading = true;
        TransiManager.Instance.LaunchShrink();
        _countNbOfTile = 0;
        LastMoveManager.Instance.InitCurrentStateMap(_mapSize);

        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                // Get the string of the actual line
                string line = _mapInfo[y];
                // Get the actual char of the string of the actual line
                char whichEnvironment = line[x];

                if (dico[whichEnvironment] != AllStates.None)
                {
                    GameObject ground = Instantiate(_groundPrefab, _map.transform);
                    InitObj(ground, x, y, dico[whichEnvironment]);
                    yield return new WaitForSeconds(_timeToSpawnMap);
                }
                else
                {
                    LastMoveManager.Instance.UpdateCurrentStateMap(x, y, dico[whichEnvironment]);
                }
            }
        }

        // Update Quest
        if (_isFullFloorOrder)
        {
            ScreensManager.Instance.InitMaxNbFullFloor(_countNbOfTile);
            _countNbOfTile = 0;
        }

        QuestsManager.CheckQuest();
        // Save all actions
        LastMoveManager.Instance.InitMapGrid(_mapGrid);
        LastMoveManager.Instance.SaveNewMap();


        IsLoading = false;
    }

    private void InitObj(GameObject which, int x, int y, AllStates state)
    {
        if (state == AllStates.None) return;

        float hexOffset = 0;
        if (x % 2 == 1)
            hexOffset = HALF_OFFSET;

        // Tp ground to its position
        which.transform.position = new Vector3(x * _distance * QUARTER_OFFSET, 0, (y + hexOffset) * _distance);

        // Get Ground State Manager
        var ground = which.GetComponent<GroundStateManager>();
        // Change coords of the ground
        ground.ChangeCoords(new Vector2Int(x, y));

        //Init state of ground
        ground.InitState(state);

        // Update _mapGrid
        _mapGrid[x, y] = which;

        // Init if is Player Force Swap
        var coord = new Vector2Int(x, y);

        if (_isPlayerForceSwap)
        {
            ground.IsPlayerForceSwapBlocked = coord != _stockPlayerForceSwap[0];
            ground.UpdatePrevisuArrow(!(coord != _stockPlayerForceSwap[0]));
            // ground.UpdatePrevisuArrow(true);
        }
        else
            ground.UpdatePrevisuArrow(false);

        // Count Nb Of Tile for Full Floor Order
        if (state != AllStates.Mountain)
        {
            _countNbOfTile++;
        }

        // Init Crystal or not
        Vector2Int[] coordsByCurrentLvl = _mapConstructData.Coords.ToArray();
        foreach (var crystalsCoords in coordsByCurrentLvl)
        {
            if (crystalsCoords.x != x || crystalsCoords.y != y) continue;

            which.GetComponent<CrystalsGround>().InitCrystal();
            return;
        }

        which.GetComponent<CrystalsGround>().UpdateCrystals(false, true);
    }

    private void Update()
    {
        // Right click to Reset
        if (Input.GetMouseButtonDown(1))
        {
            ResetBig();
        }
    }

    public void UpdateMap()
    {
        UpdateGround?.Invoke();
    }

    public void UpdateSecondBlocForce()
    {
        if (_stockPlayerForceSwap.Count == 0) return;

        if (!GetHasFirstSwap())
        {
            ScreensManager.Instance.SpawnNewDialogs(_previewMessageTuto, false, false);
        }

        var secondGround = _mapGrid[_stockPlayerForceSwap[1].x, _stockPlayerForceSwap[1].y]
            .GetComponent<GroundStateManager>();
        secondGround.UpdatePrevisuArrow(true);
        secondGround.IsPlayerForceSwapBlocked = false;
    }

    private void ChangeLevel(bool nextlevel)
    {
        if (_currentLevel < _levelData.Length - 1 && nextlevel)
            _currentLevel++;

        InitializeMap();

        // StartCoroutine(WaitToChangeLevel());
    }

    public void ChangeActivatedButton(GameObject button)
    {
        // Activate or not the UI Button's indicator and update if one was selected or not
        if (IsGroundFirstSelected || ScreensManager.Instance.GetIsDialogTime()) return;

        if (IsPlayerForcePoseBlocAfterSwap)
        {
            if (button != null)
                UpdateAllGroundTutoForcePose(true);
        }

        // Activate Trash can
        if (button != null)
        {
            if (_hasRecycling && NbOfRecycling > 0)
                RecyclingManager.Instance.UpdateRecycling(true);
        }

        // Prevent to use an actual empty button
        if (button != null)
        {
            if (button.GetComponent<UIButton>().GetNumberLeft() <= 0)
                return;
        }

        // Deactivate the last one selected
        if (LastObjButtonSelected != null)
            LastObjButtonSelected.GetComponent<UIButton>().ActivateSelectedIcon(false);

        // Update the current selected or if no one was selected -> can be null
        LastObjButtonSelected = button;

        if (LastObjButtonSelected != null)
        {
            _isDragNDrop = false;
            LastObjButtonSelected.GetComponent<UIButton>().ActivateSelectedIcon(true);
            LastStateButtonSelected = LastObjButtonSelected.GetComponent<UIButton>().GetStateButton();
        }
        else
        {
            _isDragNDrop = true;
            LastStateButtonSelected = AllStates.None;
        }
    }

    public bool CanPoseBloc()
    {
        return LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() > 0;
    }

    public void DecreaseNumberButton()
    {
        LastObjButtonSelected.GetComponent<UIButton>().UpdateNumberLeft(-1);
    }

    private void GroundSwap(GameObject which, Vector2Int newCoords)
    {
        if (IsSwapping)
        {
            ResetGroundSelected();
            return;
        }

        // Bloc other Swap before it finished
        IsSwapping = true;

        StartCoroutine(GroundSwapCorou(which, newCoords));
    }

    private IEnumerator GroundSwapCorou(GameObject which, Vector2Int newCoords)
    {
        // Update if first swap
        if (!_hasFirstSwap)
        {
            _hasFirstSwap = true;

            if (!IsPlayerForcePoseBlocAfterSwap)
            {
                // IsTuto = false;
                ResetAllPlayerForceSwapped(true);
            }
            else
            {
                ResetAllPlayerForceSwapped(false);
                ScreensManager.Instance.UpdateTutoArrow(true);
            }
        }

        // Update map
        _mapGrid[newCoords.x, newCoords.y] = _lastGroundSelected;
        _mapGrid[_lastGroundCoordsSelected.x, _lastGroundCoordsSelected.y] = which;

        // Get position
        var lastSelecPos = _lastGroundSelected.transform.position;
        var whichPos = which.transform.position;
        // (_lastGroundSelected.transform.position, which.transform.position) =
        //     (which.transform.position, _lastGroundSelected.transform.position);

        // Get GroundStateManager 
        var gLastGroundSelected = _lastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Make them swapping true to security for non wished moves
        gLastGroundSelected.UpdateIsSwapping(true);
        gWhich.UpdateIsSwapping(true);

        // Kill their tween
        _lastGroundSelected.transform.DOKill();
        which.transform.DOKill();

        // Go to enter Y Pos
        gLastGroundSelected.GetIndicator().GetComponent<GroundIndicator>().OnEnterAnim(0);
        gWhich.GetIndicator().GetComponent<GroundIndicator>().OnEnterAnim(0);

        // Make them jump with tween 
        _lastGroundSelected.transform.DOJump(whichPos, 10, 1, _timeToSwap);
        which.transform.DOJump(lastSelecPos, 15, 1, _timeToSwap);

        // Kill the preview
        ResetPreview();

        // Wait til the jump is finished
        yield return new WaitForSeconds(_timeToSwap);

        // Remove security
        gLastGroundSelected.UpdateIsSwapping(false);
        gWhich.UpdateIsSwapping(false);

        // Protect these blocs a transformation
        gLastGroundSelected.IsProtected = true;
        gWhich.IsProtected = true;

        // Change coords inside of GroundManager
        gLastGroundSelected.ChangeCoords(newCoords);
        gWhich.ChangeCoords(_lastGroundCoordsSelected);

        // Update Ground Around
        gLastGroundSelected.UpdateGroundsAround(gLastGroundSelected.GetCurrentStateEnum());
        gWhich.UpdateGroundsAround(gWhich.GetCurrentStateEnum());

        // Update the current state map
        LastMoveManager.Instance.UpdateCurrentStateMap(newCoords, gLastGroundSelected.GetCurrentStateEnum());
        LastMoveManager.Instance.UpdateCurrentStateMap(_lastGroundCoordsSelected, gWhich.GetCurrentStateEnum());

        // Launch FX to update around them
        gLastGroundSelected.LaunchDropFX();
        gWhich.LaunchDropFX();

        // Get Bloc to UI
        if (_hasInventory)
        {
            var tileToAdd = ConditionManager.Instance.GetState(gLastGroundSelected.GetCurrentStateEnum(),
                gWhich.GetCurrentStateEnum());
            var infoGrndData = SetupUIGround.Instance.GetGroundUIData((int)tileToAdd);

            ItemCollectedManager.Instance.SpawnFBGroundCollected(infoGrndData.Icon, infoGrndData.ColorIcon,
                String.Empty, tileToAdd);
        }

        // Spend energy
        EnergyManager.Instance.ReduceEnergyBySwap();

        // yield return new WaitForSeconds(.01f);

        // Get crystals if have crystals
        which.GetComponent<CrystalsGround>().UpdateCrystals(false, false);
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<CrystalsGround>().UpdateCrystals(false, false);


        // Wait the FX is finished
        yield return new WaitForSeconds(1.75f);


        // Bloc for Next Swap
        if (_blockLastGroundsSwapped)
        {
            gWhich.JustBeenSwaped = true;
            gLastGroundSelected.JustBeenSwaped = true;
            gWhich.UpdateFBReloadEnergy(true);
            gLastGroundSelected.UpdateFBReloadEnergy(true);

            ResetTwoLastSwapped();

            _lastGroundSwapped[0] = gWhich;
            _lastGroundSwapped[1] = gLastGroundSelected;
            LastMoveManager.Instance.UpdateLastGroundSwapped(gWhich, gLastGroundSelected);
        }

        // Save all actions
        LastMoveManager.Instance.SaveNewMap();

        // Reset protect
        gLastGroundSelected.IsProtected = false;
        gWhich.IsProtected = false;

        //ResetLastSelected
        IsGroundFirstSelected = false;
        ResetGroundSelected();

        // Check Quest
        QuestsManager.CheckQuest();

        // Check Game Over is no recycling
        if (!_hasRecycling)
            CheckIfGameOver();

        // Allow next Swap
        IsSwapping = false;
    }

    public void GroundSwapPreview(GameObject which)
    {
        if (!_hasPrevisu) return;

        // Reset old ground entered
        ResetPreview();

        // Get GroundStateManager 
        var gLastGroundSelected = _lastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Protect these blocs a transformation
        gLastGroundSelected.IsProtectedPrevisu = true;
        gWhich.IsProtectedPrevisu = true;

        // Update Ground Around
        gWhich.UpdateGroundsAroundPreview(gLastGroundSelected.GetCurrentStateEnum());
        gLastGroundSelected.UpdateGroundsAroundPreview(gWhich.GetCurrentStateEnum());

        // Update new Tile in inventory
        if (_hasInventory)
        {
            var result = ConditionManager.Instance.GetState(gLastGroundSelected.GetCurrentStateEnum(),
                gWhich.GetCurrentStateEnum());
            SetupUIGround.Instance.UpdatePreviewInventory(true, result);
        }

        // Reset protect
        gLastGroundSelected.IsProtectedPrevisu = false;
        gWhich.IsProtectedPrevisu = false;

        // Update their previsu
        gWhich.ChangeStatePrevisu(gLastGroundSelected.GetCurrentStateEnum());
        gLastGroundSelected.ChangeStatePrevisu(gWhich.GetCurrentStateEnum());
    }

    public void GroundSwapPreviewButton(GameObject which, AllStates buttonState)
    {
        if (!_hasPrevisu) return;

        // Reset old ground entered
        ResetPreview();

        var gWhich = which.GetComponent<GroundStateManager>();

        gWhich.IsProtectedPrevisu = true;
        gWhich.UpdateGroundsAroundPreview(buttonState);
        gWhich.IsProtectedPrevisu = false;

        gWhich.ChangeStatePrevisu(buttonState);
    }

    public void UseRecycling()
    {
        if (ScreensManager.Instance.GetIsDialogTime()) return;

        if (LastObjButtonSelected == null || NbOfRecycling <= 0)
        {
            WantToRecycle();
            return;
        }

        if (!_hasInfinitRecycling)
            NbOfRecycling--;

        // Remove 1 from button
        LastObjButtonSelected.GetComponent<UIButton>().UpdateNumberLeft(-1);
        // Add 1 to number of interaction
        EnergyManager.Instance.EarnEnergyByRecycling();
        // Deactivate Follow Dnd
        SetupUIGround.Instance.FollowDndDeactivate();
        // Update Nb of Recycling Left
        RecyclingManager.Instance.UpdateNbRecyclingLeft();

        // Reset
        ResetButtonSelected();
        ResetTwoLastSwapped();
        _wantToRecycle = false;

        RecyclingManager.Instance.DeactivateButton();

        // Save all actions
        LastMoveManager.Instance.SaveNewMap();
    }

    private void WantToRecycle()
    {
        _wantToRecycle = true;
    }

    public void CheckIfWantToRecycle(GameObject which)
    {
        if (!_wantToRecycle) return;

        LastObjButtonSelected = which;

        UseRecycling();
    }

    private void CheckAroundGroundSelected(GameObject which, Vector2Int coords)
    {
        // Reset to start from scratch
        ResetGroundSelected();
        // Update lastSelected if need to call Swap() after
        _lastGroundSelected = which;
        _lastGroundCoordsSelected = coords;
    }

    public bool CheckIfButtonIsEmpty()
    {
        if (LastObjButtonSelected == null) return true;

        return LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() <= 0;
    }

    public void CheckIfGroundSelected(GameObject which, Vector2Int newCoords)
    {
        if (LastObjButtonSelected != null) return;

        if (_lastGroundSelected != null)
            GroundSwap(which, newCoords);
        else
            CheckAroundGroundSelected(which, newCoords);
    }

    public void CheckIfGameOver()
    {
        if (IsVictory) return;

        StartCoroutine(WaitLittleToCheckGameOver());
    }

    IEnumerator WaitLittleToCheckGameOver()
    {
        yield return new WaitForSeconds(.02f);

        // bool inventory = EnergyManager.Instance.IsEnergyInferiorToCostLandingGround() || !_hasInventory;
        //
        // if (EnergyManager.Instance.IsEnergyInferiorToCostSwap()
        //     && inventory
        //     && !SetupUIGround.Instance.CheckIfStillGround())
        // {
        //     ScreensManager.Instance.GameOver();
        // }

        if (IsVictory) yield break;

        if (EnergyManager.Instance.GetCurrentEnergy() <= 0)
        {
            if (!_hasInventory)
                ScreensManager.Instance.GameOver();
            else if (_hasInventory && !_hasRecycling)
                ScreensManager.Instance.GameOver();
            else if (_hasInventory && _hasRecycling && NbOfRecycling <= 0)
                ScreensManager.Instance.GameOver();
        }
    }

    public AllStates GetLastStateSelected()
    {
        return _lastGroundSelected != null
            ? _lastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum()
            : LastStateButtonSelected;
    }

    public AllStates GetSecondtateSelected()
    {
        return AllStates.Desert;
    }

    public GameObject[,] GetMapGrid()
    {
        return _mapGrid;
    }

    public bool GetIsDragNDrop()
    {
        return _isDragNDrop;
    }

    public bool GetHasGroundSelected()
    {
        return _lastGroundSelected;
    }

    public bool GetHasFirstSwap()
    {
        return _hasFirstSwap;
    }

    public string[] GetDialogAtVictory()
    {
        if (LanguageManager.Instance.Tongue == Language.Francais)
            return _levelData[_currentLevel].DialogEnd;

        return _levelData[_currentLevel].DialogEndEnglish;
    }

    // public void GoToLastMove()
    // {
    //     if (IsSwapping || IsPosing || IsVictory) return;
    //
    //     if (_stockStateMap.Count <= 1) return;
    //
    //     print("go to last move"); //" :  size of _currentStateMapStock before : " + _currentStateMapStock.Count);
    //
    //
    //     // Update floor and crystals
    //     for (int x = 0; x < _mapSize.x; x++)
    //     {
    //         for (int y = 0; y < _mapSize.y; y++)
    //         {
    //             if (_stockStateMap[0][x, y] != AllStates.None)
    //             {
    //                 // print($"ole / x : {x} - y : {y} - state :{_currentStateMap[x, y]}");
    //                 _mapGrid[x, y].GetComponent<GroundStateManager>().ForceChangeState(_stockStateMap.Count == 1
    //                     ? _stockStateMap[0][x, y]
    //                     : _stockStateMap[^2][x, y]);
    //
    //                 if (_stockCrystals.Count == 1)
    //                 {
    //                     if (_stockCrystals[0][x, y])
    //                     {
    //                         _mapGrid[x, y].GetComponent<CrystalsGround>().InitCrystal();
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (_stockCrystals[^2][x, y])
    //                     {
    //                         _mapGrid[x, y].GetComponent<CrystalsGround>().InitCrystal();
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //
    //     // Update Energy
    //     // print($"_stockEnergy[^2] : {_stockEnergy[^2]} -  Current Energy : {EnergyManager.Instance.GetCurrentEnergy()}");
    //     int getOldEnergy = 0;
    //
    //     if (_stockEnergy.Count == 1)
    //         getOldEnergy = _stockEnergy[0] - EnergyManager.Instance.GetCurrentEnergy();
    //     else
    //         getOldEnergy = _stockEnergy[^2] - EnergyManager.Instance.GetCurrentEnergy();
    //
    //     EnergyManager.Instance.UpdateEnergy(getOldEnergy);
    //
    //
    //     // Update Last Swaped Block
    //     if (_stockLastGroundSwaped.Count > 1)
    //     {
    //         if (_stockLastGroundSwaped[^2][0] != null)
    //             _stockLastGroundSwaped[^2][0].UpdateNoSwap(true);
    //         if (_stockLastGroundSwaped[^2][1] != null)
    //             _stockLastGroundSwaped[^2][1].UpdateNoSwap(true);
    //     }
    //
    //     if (_stockLastGroundSwaped[^1][0] != null)
    //         _stockLastGroundSwaped[^1][0].UpdateNoSwap(false);
    //     if (_stockLastGroundSwaped[^1][1] != null)
    //         _stockLastGroundSwaped[^1][1].UpdateNoSwap(false);
    //
    //     // Update Inventory
    //     if (_stockStateMap.Count > 1)
    //     {
    //         SetupUIGround.Instance.ResetAllButtons();
    //
    //         for (int i = 0; i < _stockTileButtonTest[^2].Count; i++)
    //         {
    //             for (int j = 0; j < _stockTileButtonTest[^2][i]; j++)
    //             {
    //                 SetupUIGround.Instance.AddNewGround(i, true);
    //             }
    //         }
    //     }
    //
    //     // Update Recycling
    //     NbOfRecycling = _stockNbRecycle.Count == 1 ? _stockNbRecycle[0] : _stockNbRecycle[^2];
    //     RecyclingManager.Instance.UpdateNbRecyclingLeft();
    //
    //     // Remove Last
    //     if (_stockStateMap.Count > 1)
    //     {
    //         _stockStateMap.RemoveAt(_stockStateMap.Count - 1);
    //         _stockEnergy.RemoveAt(_stockEnergy.Count - 1);
    //         _stockLastGroundSwaped.RemoveAt(_stockLastGroundSwaped.Count - 1);
    //         _stockTileButtonTest.RemoveAt(_stockTileButtonTest.Count - 1);
    //         _stockNbRecycle.RemoveAt(_stockNbRecycle.Count - 1);
    //         _stockCrystals.RemoveAt(_stockCrystals.Count - 1);
    //     }
    //
    //     QuestsManager.CheckQuest();
    //
    //     if (_stockStateMap.Count <= 1)
    //     {
    //         ScreensManager.Instance.UpdateBackwardsButton(false);
    //     }
    // }

    // public void UpdateCurrentStateMap(Vector2Int coords, AllStates newState)
    // {
    //     UpdateCurrentStateMap(coords.x, coords.y, newState);
    // }
    //
    // public void UpdateCurrentStateMap(int x, int y, AllStates newState)
    // {
    //     // print($"x : {x} - y : {y} - state :{newState}");
    //     _currentStateMap[x, y] = newState;
    // }
    //
    // private bool[,] UpdateCrystalMap()
    // {
    //     bool[,] newCrystalMap = new bool[_mapSize.x, _mapSize.y];
    //
    //     for (int x = 0; x < _mapSize.x; x++)
    //     {
    //         for (int y = 0; y < _mapSize.y; y++)
    //         {
    //             if (_currentStateMap[x, y] != AllStates.None)
    //             {
    //                 // bool hasCrystal = _mapGrid[x, y].GetComponent<CrystalsGround>().GetIfHasCrystal();
    //                 // newCrystalMap[x, y] = hasCrystal;
    //                 newCrystalMap[x, y] = false;
    //             }
    //             else
    //             {
    //                 newCrystalMap[x, y] = false;
    //             }
    //         }
    //     }
    //
    //     return newCrystalMap;
    // }
    //
    // public void SaveNewMap()
    // {
    //     // print("new current map");
    //     ScreensManager.Instance.UpdateBackwardsButton(_stockStateMap.Count != 0);
    //
    //     // Stock Floor
    //     AllStates[,] newMapState = new AllStates[_mapSize.x, _mapSize.y];
    //     Array.Copy(_currentStateMap, newMapState, _currentStateMap.Length);
    //     _stockStateMap.Add(newMapState);
    //
    //     // Stock Energy
    //     _stockEnergy.Add(EnergyManager.Instance.GetCurrentEnergy());
    //
    //     // Stock Crystals
    //     // bool[,] newCrystalMap = new bool[_mapSize.x, _mapSize.y];
    //     // Array.Copy(UpdateCrystalMap(), newCrystalMap, UpdateCrystalMap().Length);
    //     _stockCrystals.Add(UpdateCrystalMap());
    //
    //     // Stock Last Block Swaped
    //     GroundStateManager[] newLastBlocked = new GroundStateManager[2];
    //     Array.Copy(_lastGroundSwaped, newLastBlocked, _lastGroundSwaped.Length);
    //     _stockLastGroundSwaped.Add(newLastBlocked);
    //
    //     // Get Inventory
    //     List<GameObject> inventory = SetupUIGround.Instance.GetStockTileButton();
    //     int[] test = new int[10];
    //     foreach (var but in inventory)
    //     {
    //         var currentTile = but.GetComponent<UIButton>();
    //
    //         test[(int)currentTile.GetStateButton()] += currentTile.GetNumberLeft();
    //     }
    //
    //     _stockTileButtonTest.Add(test.ToList());
    //
    //     // Get Nb of Recycle
    //     _stockNbRecycle.Add(NbOfRecycling);
    // }

    public void ForceResetBig()
    {
        print("force reset all");

        AllReset();
    }

    public void ResetBig()
    {
        if (ScreensManager.Instance.GetIsDialogTime() || IsSwapping || IsPosing || IsOnUI ||
            MouseHitRaycast.Instance.IsOnGround) return;

        ScreensManager.Instance.UpdateTutoArrow(false);

        if (IsTuto) return;

        print("reset big");

        AllReset();
    }

    private void AllReset()
    {
        RecyclingManager.Instance.DeactivateButton();
        ResetButtonSelected();
        //RecyclingManager.Instance.UpdateRecycling(false);
        ResetPreview();
        ResetGroundSelected();
    }

    public void ResetAllMap(bool nextLevel)
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                Destroy(_mapGrid[x, y]);
                _mapGrid[x, y] = null;
            }
        }

        LastMoveManager.Instance.ResetGoToLastMove();
        SetupUIGround.Instance.ResetAllButtons();
        ItemCollectedManager.Instance.DeleteAllFB();

        IsVictory = false;
        _isFullFloorOrder = false;

        ChangeLevel(nextLevel);
    }

    public void RestartLevel()
    {
        // if(ScreensManager.Instance.GetIsDialogTime()) return;
        if (IsSwapping || IsPosing || IsLoading) return;

        //ResetAllMap(false);
        // ResetGoToLastMove();
        // print("laucnh restart");

        StartCoroutine(WaitToRestart());
        // InitializeMap();
    }

    IEnumerator WaitToRestart()
    {
        TransiManager.Instance.LaunchGrownOn();

        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForGrowOn());

        ResetAllSelection();
        ResetButtonSelected();
        ResetGroundSelected();
        SetupUIGround.Instance.ResetAllButtons();
        ScreensManager.Instance.RestartSceneOrLevel();
        ResetAllMap(false);
    }

    public void ResetButtonSelected()
    {
        ChangeActivatedButton(null);
    }

    public void ResetGroundSelected()
    {
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<GroundStateManager>().ResetIndicator();
        _lastGroundSelected = null;
        _lastGroundCoordsSelected = new Vector2Int(-1, -1);
    }

    public void ResetAllSelection()
    {
        ResetSelection?.Invoke();
    }

    public void ResetTwoLastSwapped()
    {
        if (_lastGroundSwapped[0] != null)
            _lastGroundSwapped[0].UpdateNoSwap(false);
        if (_lastGroundSwapped[1] != null)
            _lastGroundSwapped[1].UpdateNoSwap(false);

        _lastGroundSwapped[0] = null;
        _lastGroundSwapped[1] = null;
        
        LastMoveManager.Instance.UpdateLastGroundSwapped(null, null);
    }

    public void ResetPreview()
    {
        // if(_lastGroundPrevisuEntered != null)
        //     _lastGroundPrevisuEntered.ResetPrevisu();
        // if(_lastGroundSelected != null)
        //     _lastGroundSelected.GetComponent<GroundStateManager>().ResetPrevisu();

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_mapGrid[x, y] == null) continue;
                if (_mapGrid[x, y].GetComponent<GroundStateManager>() == null) continue;

                var ground = _mapGrid[x, y].GetComponent<GroundStateManager>();
                ground.ResetStockPrevisu();
            }
        }

        SetupUIGround.Instance.UpdatePreviewInventory(false, AllStates.None);
    }

    private void ResetAllPlayerForceSwapped(bool isTutoEnded)
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_mapGrid[x, y] == null) continue;
                if (_mapGrid[x, y].GetComponent<GroundStateManager>() == null) continue;

                var ground = _mapGrid[x, y].GetComponent<GroundStateManager>();
                if (isTutoEnded)
                    ground.IsPlayerForceSwapBlocked = false;
                ground.GetFbArrow().gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAllGroundTutoForcePose(bool blockAll)
    {
        ResetAllPlayerForceSwapped(true);

        ScreensManager.Instance.UpdateTutoArrow(false);

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_mapGrid[x, y] == null) continue;
                if (_mapGrid[x, y].GetComponent<GroundStateManager>() == null) continue;

                var ground = _mapGrid[x, y].GetComponent<GroundStateManager>();

                if (blockAll)
                {
                    if (_coordsForcePoseBloc.x == x && _coordsForcePoseBloc.y == y)
                    {
                        ground.IsPlayerNotForcePose = false;
                        ground.GetFbArrow().gameObject.SetActive(true);
                        ground.GetFbArrow().UpdateArrow(true);
                    }
                    else
                    {
                        ground.IsPlayerNotForcePose = true;
                        ground.GetFbArrow().gameObject.SetActive(false);
                    }
                }
                else
                {
                    ground.IsPlayerNotForcePose = false;
                    ground.GetFbArrow().gameObject.SetActive(false);
                }
            }
        }
    }
}