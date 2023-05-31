using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine.Networking;


public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    public event Action UpdateGround;

    // public event Action CheckBiome;
    public event Action ResetSelection;

    #region Prop

    public AllStates LastStateButtonSelected { get; set; }
    public GameObject LastObjButtonSelected { get; set; }
    public GameObject LastGroundSelected { get; set; }
    public bool IsGroundFirstSelected { get; set; }
    public bool IsVictory { get; set; }
    public QuestManager QuestsManager { get; private set; }
    public int NbOfRecycling { get; set; }
    public bool IsSwapping { get; private set; }
    public bool IsPosing { get; set; }
    public bool IsLoading { get; set; }
    public bool IsOnUI { get; set; }
    public bool IsTuto { get; set; }
    public bool IsTutoRecycling { get; set; }
    public bool HasInventory { get; set; }
    public bool IsPlayerForcePoseBlocAfterSwap { get; private set; }
    public bool OpenMemo { get; private set; }

    #endregion

    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;
    [SerializeField] private float _distance;

    [Header("Android")] public bool IsAndroid;

    [Header("HTML")] [SerializeField] private GameObject _loadingText;

    [Header("Choose Start Level Index")] [SerializeField]
    private int _currentLevel;

    [Header("Timing")] [SerializeField] private float _timeToSwap;
    [SerializeField] private float _timeToSpawnMap;
    [SerializeField] private float _timeWaitBetweenDropFX;
    [SerializeField] private float _timeWaitEndSwap = 1.5f;
    [Header("Data")] [SerializeField] private LevelData[] _levelData;

    private bool _hasRecycling;
    private bool _hasInfinitRecycling;
    private bool _hasPrevisu;
    private bool _blockLastGroundsSwapped;
    private bool _isPlayerForceSwap;
    private bool _hasFirstSwap;
    private List<Vector2Int> _stockPlayerForceSwap = new List<Vector2Int>();
    private bool _isDragNDrop;
    private bool _wantToRecycle;
    private string[] _mapInfo;
    private string[] _previewMessageTuto;
    private bool _isFullFloorOrder;

    private Vector2Int _mapSize;
    private Vector2Int _lastGroundCoordsSelected;
    private Vector2Int _coordsForcePoseBloc;
    private GameObject[,] _mapGrid;
    private Image _recycleImg;
    private AllStates _secondLastGroundSelected;
    private int _countNbOfTile;
    private int _countTilesWithCrystal;
    private bool _tileHasCrystal;
    private bool _forceSwapHasFirstTile;
    private bool _forceSwapHasSecondTile;
    Sprite[] _charaSpritesBegininng = new Sprite[0];
    Sprite[] _charaSpritesEnd = new Sprite[0];


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
        dico.Add(PLAIN, AllStates.__Grassias__);
        dico.Add(DESERT, AllStates.Desert);
        dico.Add(WATER, AllStates.__Hydros__);
        dico.Add(TROPICAL, AllStates.Tropical);
        dico.Add(SAVANNA, AllStates.__Calcid__);
        dico.Add(GEYSER, AllStates.Geyser);
        dico.Add(SNOW, AllStates.Snow);
        dico.Add(POLAR_DESERT, AllStates.PolarDesert);
        dico.Add(TUNDRA, AllStates.Tundra);
        dico.Add(SWAMP, AllStates.__Viscosa__);
        dico.Add(MOUNTAIN, AllStates.__Pyreneos__);

        if (LevelProgressionManager.Instance != null)
        {
            _currentLevel = LevelProgressionManager.Instance.CurrentLevel;
            // Check if not too High
            if (_currentLevel >= _levelData.Length)
                _currentLevel = _levelData.Length - 1;
        }

        if (_currentLevel >= _levelData.Length)
        {
            ScreensManager.Instance.LaunchCredits();
            return;
        }

        StartCoroutine(CheckFileMap());
    }

    IEnumerator CheckFileMap()
    {
        var mapNameJson = _levelData[_currentLevel].LevelName;
        var mapFolderName = _levelData[_currentLevel].LevelFolder;
        var mapPath = $"{mapFolderName}/{mapNameJson}.txt";

#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(LoadTextFileHTML(mapPath));
        yield return new WaitForSeconds(2.5f);
        _loadingText.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        _loadingText.SetActive(false);
#else
        LoadTextFileNormal(mapPath);
        yield return new WaitForSeconds(.1f);
#endif


        InitializeMap();
        LastStateButtonSelected = AllStates.None;
    }

    IEnumerator LoadTextFileHTML(string filePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filePath);

        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string textt = www.downloadHandler.text;
            // Faites quelque chose avec le contenu du fichier texte ici

            var testMapConstructData = JsonUtility.FromJson<MapConstructData>(textt);
            _mapConstructData = testMapConstructData;
        }
    }

    private void LoadTextFileNormal(string mapPath)
    {
        // Initialize
        BetterStreamingAssets.Initialize();
        // Get the text map
        if (!BetterStreamingAssets.FileExists(mapPath))
        {
            Debug.LogErrorFormat("Streaming asset not found: {0}", mapPath);
        }

        var lineJson = BetterStreamingAssets.ReadAllText(mapPath);
        _mapConstructData = JsonUtility.FromJson<MapConstructData>(lineJson);
    }

    private void InitializeMap()
    {
        var currentLvl = _levelData[_currentLevel];

        // Old
        //string mapPath2 = Application.streamingAssetsPath + $"/{mapFolderName}/{mapNameJson}.txt";
        // var lineJson = File.ReadAllText(mapPath);


        _mapInfo = _mapConstructData.Map.Split("\n");

        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;

        // Init the grids
        _mapGrid = new GameObject[_mapSize.x, _mapSize.y];

        // Reset Energy and Wave 
        EnergyManager.Instance.ResetEnergy();
        EnergyManager.Instance.StopWaveEffect();

        // Init Start energy
        var startEnergy = currentLvl.EnergyAtStart;
        var startNbRecycling = currentLvl.NbOfRecycling;
        _countTilesWithCrystal = _mapConstructData.Coords.Count;
        var reduceBySwap = (_countTilesWithCrystal) / 2;
        if (_countTilesWithCrystal == 1)
            reduceBySwap = 1;
        var maxEnergy = startEnergy + _countTilesWithCrystal * 2 - reduceBySwap + startNbRecycling;
        EnergyManager.Instance.InitEnergy(startEnergy, maxEnergy);
        EnergyManager.Instance.LaunchAnimEnergy();

        // Update if has inventory
        HasInventory = currentLvl.HasInventory;
        SetupUIGround.Instance.UpdateOpacityInventory(0);

        // Update if tile at start
        if (currentLvl.StartNbAllState != null && HasInventory)
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

        // Update if open memo
        OpenMemo = currentLvl.OpenMemo;

        // Update if has Preview
        _hasPrevisu = currentLvl.HasPrevisu;

        // Update if bloc last grounds swapped
        _blockLastGroundsSwapped = currentLvl.BlockLastSwap;

        // Update if tuto
        IsTuto = currentLvl.IsTuto;
        SetupUIGround.Instance.SetActiveBackwardsButton(!IsTuto);
        IsTutoRecycling = currentLvl.IsTutoRecycling;

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
                    _forceSwapHasFirstTile = false;
                    _forceSwapHasSecondTile = false;
                }

                if (HasInventory)
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
        if (LanguageManager.Instance.Tongue == Language.Francais)
            ScreensManager.Instance.InitOrderDescription(currentLvl.QuestDescription);
        else if (LanguageManager.Instance.Tongue == Language.English)
            ScreensManager.Instance.InitOrderDescription(currentLvl.QuestDescriptionEnglish);

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
        bool hasPopUp = false;
        if (currentLvl.PopUpInfos != null)
        {
            if (currentLvl.PopUpInfos.Length > 0)
                PopUpManager.Instance.InitPopUp(currentLvl.PopUpInfos);
            
            // Update Dialogs
            hasPopUp = currentLvl.PopUpInfos.Length > 0;
        }

        if (currentLvl.CharacterSpritesBeginning != null)
            _charaSpritesBegininng = currentLvl.CharacterSpritesBeginning;
        if (currentLvl.CharacterSpritesEnd != null)
            _charaSpritesEnd = currentLvl.CharacterSpritesEnd;

        if (LanguageManager.Instance.Tongue == Language.Francais)
            ScreensManager.Instance.SpawnNewDialogs(_levelData[_currentLevel].DialogBeginning, false, hasPopUp, _charaSpritesBegininng);
        else if (LanguageManager.Instance.Tongue == Language.English)
            ScreensManager.Instance.SpawnNewDialogs(_levelData[_currentLevel].DialogBeginningEnglish, false, hasPopUp, _charaSpritesBegininng);

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
        yield return new WaitForSeconds(TransiManager.Instance.GetTimeForShrink() / 2);
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

        // Update if full floor order 
        if (_isFullFloorOrder)
        {
            ScreensManager.Instance.InitMaxNbFullFloor(_countNbOfTile);
            _countNbOfTile = 0;
        }

        QuestsManager.CheckQuest();
        // Save all actions
        LastMoveManager.Instance.InitMapGrid(_mapGrid);
        ResetTwoLastSwapped();
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
            // ground.UpdatePrevisuArrow(!(coord != _stockPlayerForceSwap[0]));
            ground.UpdatePrevisuArrow(false);
        }
        else
            ground.UpdatePrevisuArrow(false);

        // Count Nb Of Tile for Full Floor Order
        if (state != AllStates.__Pyreneos__)
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
            if (IsTuto) return;

            ForceResetBig();
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
            Sprite[] charaNoChangement = new Sprite[0];
            ScreensManager.Instance.SpawnNewDialogs(_previewMessageTuto, false, false, charaNoChangement);
        }

        var secondGround = _mapGrid[_stockPlayerForceSwap[1].x, _stockPlayerForceSwap[1].y]
            .GetComponent<GroundStateManager>();
        // secondGround.UpdatePrevisuArrow(true);
        secondGround.IsPlayerForceSwapBlocked = false;
    }

    private void ChangeLevel(bool nextlevel)
    {
        var checkNextLvl = _currentLevel + 1;
        if (checkNextLvl >= _levelData.Length && nextlevel)
        {
            ScreensManager.Instance.LaunchCredits();
            return;
        }

        if (_currentLevel < _levelData.Length - 1 && nextlevel)
        {
            _currentLevel++;

            if (LevelProgressionManager.Instance != null)
            {
                LevelProgressionManager.Instance.CurrentLevel++;
            }
        }

        StartCoroutine(CheckFileMap());

        //InitializeMap();

        // StartCoroutine(WaitToChangeLevel());
    }

    public void ChangeActivatedButton(GameObject button)
    {
        // Activate or not the UI Button's indicator and update if one was selected or not
        if (IsGroundFirstSelected || ScreensManager.Instance.GetIsDialogTime()) return;

        if (LastObjButtonSelected != null && button == LastObjButtonSelected)
        {
            ChangeActivatedButton(null);
            return;
        }

        if (IsPlayerForcePoseBlocAfterSwap)
        {
            if (button != null)
                UpdateAllGroundTutoForcePose(true);
        }

        if (IsTutoRecycling && button != null)
        {
            if (!RecyclingManager.Instance.HasInitTutoRecycling)
            {
                ScreensManager.Instance.UpdateTutoArrow(false);
                RecyclingManager.Instance.UpdateArrowTuto(true);
            }
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
        AudioManager.Instance.PlaySFX("Swap");

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
        _mapGrid[newCoords.x, newCoords.y] = LastGroundSelected;
        _mapGrid[_lastGroundCoordsSelected.x, _lastGroundCoordsSelected.y] = which;

        // Get position
        var lastSelecPos = LastGroundSelected.transform.position;
        var whichPos = which.transform.position;
        // (_lastGroundSelected.transform.position, which.transform.position) =
        //     (which.transform.position, _lastGroundSelected.transform.position);

        // Get GroundStateManager 
        var gLastGroundSelected = LastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Make them swapping true to security for non wished moves
        gLastGroundSelected.UpdateIsSwapping(true);
        gWhich.UpdateIsSwapping(true);

        // Kill their tween
        LastGroundSelected.transform.DOKill();
        which.transform.DOKill();

        // Go to enter Y Pos
        gLastGroundSelected.GetIndicator().GetComponent<GroundIndicator>().OnEnterAnim(0);
        gWhich.GetIndicator().GetComponent<GroundIndicator>().OnEnterAnim(0);

        // Make them jump with tween 
        LastGroundSelected.transform.DOJump(whichPos, 10, 1, _timeToSwap);
        which.transform.DOJump(lastSelecPos, 15, 1, _timeToSwap);

        // Kill the preview
        ResetPreview();

        // Spend Energy
        EnergyManager.Instance.ReduceEnergyBySwap();


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
        //Launch FX
        gLastGroundSelected.LaunchDropFX();
        // Earn energy if has Crystals
        if (LastGroundSelected != null)
        {
            if (LastGroundSelected.GetComponent<CrystalsGround>().GetIfHasCrystal())
            {
                // _TileHasCrystal = true;
                LastGroundSelected.GetComponent<CrystalsGround>().UpdateCrystals(false, false);
                ItemCollectedManager.Instance.SpawnFBEnergyCollected(1, LastGroundSelected.transform.position);
            }
        }

        yield return new WaitForSeconds(_timeWaitBetweenDropFX / 2);

        // Earn energy if has Crystals
        if (which.GetComponent<CrystalsGround>().GetIfHasCrystal())
        {
            // _TileHasCrystal = true;
            which.GetComponent<CrystalsGround>().UpdateCrystals(false, false);
            ItemCollectedManager.Instance.SpawnFBEnergyCollected(1, which.transform.position);
        }

        yield return new WaitForSeconds(_timeWaitBetweenDropFX / 2);

        // Update Ground Around
        gWhich.UpdateGroundsAround(gWhich.GetCurrentStateEnum());
        //Launch FX
        gWhich.LaunchDropFX();

        // Update the current state map
        LastMoveManager.Instance.UpdateCurrentStateMap(newCoords, gLastGroundSelected.GetCurrentStateEnum());
        LastMoveManager.Instance.UpdateCurrentStateMap(_lastGroundCoordsSelected, gWhich.GetCurrentStateEnum());

        // Get Bloc to UI
        if (HasInventory)
        {
            var tileToAdd = ConditionManager.Instance.GetState(gLastGroundSelected.GetCurrentStateEnum(),
                gWhich.GetCurrentStateEnum());
            var infoGrndData = SetupUIGround.Instance.GetGroundUIData((int)tileToAdd);

            ItemCollectedManager.Instance.SpawnFBGroundCollected(infoGrndData.Icon, infoGrndData.ColorIcon,
                String.Empty, tileToAdd);
        }

        // Bloc for Next Swap
        if (_blockLastGroundsSwapped)
        {
            gWhich.JustBeenSwaped = true;
            gLastGroundSelected.JustBeenSwaped = true;
            gWhich.UpdateFBReloadEnergy(true);
            gLastGroundSelected.UpdateFBReloadEnergy(true);

            ResetTwoLastSwapped();

            AudioManager.Instance.PlaySFX("TileBored");

            UpdateTwoLastSwapped(gWhich, gLastGroundSelected);
            LastMoveManager.Instance.UpdateLastGroundSwapped(gWhich, gLastGroundSelected);
        }


        // Wait the FX is finished
        yield return new WaitForSeconds(_timeWaitEndSwap);


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

        // ResetBig();
    }

    public void GroundSwapPreview(GameObject which)
    {
        if (!_hasPrevisu) return;

        // Reset old ground entered
        ResetPreview();

        // Get GroundStateManager 
        var gLastGroundSelected = LastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Protect these blocs a transformation
        gLastGroundSelected.IsProtectedPrevisu = true;
        gWhich.IsProtectedPrevisu = true;

        // Update Ground Around
        gWhich.UpdateGroundsAroundPreview(gLastGroundSelected.GetCurrentStateEnum());
        gLastGroundSelected.UpdateGroundsAroundPreview(gWhich.GetCurrentStateEnum());

        // Update new Tile in inventory
        if (HasInventory)
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

    public void UpdateTwoLastSwapped(GroundStateManager gWhich, GroundStateManager gLastGroundSelected)
    {
        _lastGroundSwapped[0] = gWhich;
        _lastGroundSwapped[1] = gLastGroundSelected;
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

        AudioManager.Instance.PlaySFX("EnergyGain");

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

        RecyclingManager.Instance.DeselectRecycle();
        if (IsTutoRecycling)
        {
            RecyclingManager.Instance.HasInitTutoRecycling = true;
            RecyclingManager.Instance.UpdateArrowTuto(false);
        }

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
        LastGroundSelected = which;
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

        if (LastGroundSelected != null)
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

        if (IsVictory) yield break;

        // If no energy left
        if (EnergyManager.Instance.GetCurrentEnergy() <= 0)
        {
            // If no Inventory
            if (!HasInventory)
                ScreensManager.Instance.GameOver();
            // If has Inventory and no recycling
            else if (HasInventory && !_hasRecycling)
                ScreensManager.Instance.GameOver();
            // If has Inventory and has recycling and no ground left and nb recycling more than 0
            else if (HasInventory && _hasRecycling && !SetupUIGround.Instance.CheckIfStillGround() && NbOfRecycling > 0)
                ScreensManager.Instance.GameOver();
            // If has Inventory and has recycling and no recycling left
            else if (HasInventory && _hasRecycling && NbOfRecycling <= 0)
                ScreensManager.Instance.GameOver();
        }
    }

    public AllStates GetLastStateSelected()
    {
        return LastGroundSelected != null
            ? LastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum()
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
        return LastGroundSelected;
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

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public Sprite[] GetCharaSpritesEnd()
    {
        return _charaSpritesEnd;
    }

    public Vector2Int GetTileStockForceSwap(int index)
    {
        return _stockPlayerForceSwap[index];
    }

    public void ActivateArrowIfForceSwap()
    {
        if (_stockPlayerForceSwap.Count == 0) return;

        if (!_forceSwapHasFirstTile)
        {
            _mapGrid[_stockPlayerForceSwap[0].x, _stockPlayerForceSwap[0].y].GetComponent<GroundStateManager>()
                .UpdatePrevisuArrow(true);
            _forceSwapHasFirstTile = true;
            return;
        }

        if (!_forceSwapHasSecondTile)
        {
            _mapGrid[_stockPlayerForceSwap[1].x, _stockPlayerForceSwap[1].y].GetComponent<GroundStateManager>()
                .UpdatePrevisuArrow(true);
            _forceSwapHasSecondTile = true;
        }
    }

    public void ForceResetBig()
    {
        print("force reset all");

        AllReset();
    }

    public void ResetBig()
    {
        if (ScreensManager.Instance.GetIsDialogTime() || IsSwapping || IsPosing || IsOnUI ||
            MouseHitRaycast.Instance.IsOnGround) return;

        //ScreensManager.Instance.UpdateTutoArrow(false);

        if (IsTuto) return;

        print("reset big");

        AllReset();
    }

    private void AllReset()
    {
        RecyclingManager.Instance.DeselectRecycle();
        ResetButtonSelected();
        ResetPreview();
        ResetGroundSelected();
        //SetupUIGround.Instance.EndFb();
        MouseHitRaycast.Instance.ResetLastGroundHit();
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
        ScreensManager.Instance.CloseCommandMenu();

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
        // _wantToRecycle = false;
    }

    public void ResetGroundSelected()
    {
        if (LastGroundSelected != null)
            LastGroundSelected.GetComponent<GroundStateManager>().ResetIndicator();
        LastGroundSelected = null;
        _lastGroundCoordsSelected = new Vector2Int(-1, -1);

        // MouseHitRaycast.Instance.ResetLastGroundHit();
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

    public void ResetWantToRecycle()
    {
        _wantToRecycle = false;
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