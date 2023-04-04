using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


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
    public int NbOfRecycling { get; private set; }


    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;
    [SerializeField] private float _distance;

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
    private int _currentLevel;
    private string[] _mapInfo;

    private Vector2Int _mapSize;
    private Vector2Int _lastGroundCoordsSelected;
    private GameObject[,] _mapGrid;
    private GameObject _lastGroundSelected;

    private MapConstructData _mapConstructData;

    private GroundStateManager[] _lastGroundSwaped = new GroundStateManager[2];

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
        if (currentLvl.StartNbAllState != null)
        {
            for (int i = 0; i < currentLvl.StartNbAllState.Length; i++)
            {
                for (int j = 0; j < currentLvl.StartNbAllState[i]; j++)
                {
                    SetupUIGround.Instance.AddNewGround(i);
                }
            }
        }

        // Update if has recycling
        _hasRecycling = currentLvl.HasRecycling;
        NbOfRecycling = currentLvl.NbOfRecycling;
        _hasInfinitRecycling = currentLvl.HasInfinitRecycling;
        SetupUIGround.Instance.SetIfHasInventory(_hasRecycling);
        RecyclingManager.Instance.InitNbRecycling(NbOfRecycling, _hasInfinitRecycling);

        // Update if has Previsu
        _hasPrevisu = currentLvl.HasPrevisu;

        // Update if bloc last grounds swapped
        _blockLastGroundsSwapped = currentLvl.BlockLastSwap;

        // Update if force 2 first bloc swap
        if (currentLvl.PlayerForceSwap.Length != 0)
        {
            _isPlayerForceSwap = true;
            _stockPlayerForceSwap.Add(currentLvl.PlayerForceSwap[0]);
            _stockPlayerForceSwap.Add(currentLvl.PlayerForceSwap[1]);
        }
        else
        {
            _hasFirstSwap = true;
            _isPlayerForceSwap = false;
            _stockPlayerForceSwap.Clear();
        }

        // Reset Quest Number
        QuestsManager.ResetQuestNumbers();

        // Update if full floor quest
        if (currentLvl.QuestFloor.Length > 0)
            QuestsManager.InitQuestFullFloor(currentLvl.QuestFloor[0]);

        // Update if flower quest
        if (_levelData[_currentLevel].QuestFlower.Length > 0)
            QuestsManager.InitQuestFlower(currentLvl.QuestFlower);

        // Update if No Specific Tile quest
        if (currentLvl.QuestNoSpecificTiles.Length > 0)
            QuestsManager.InitQuestNoSpecificTiles(currentLvl.QuestNoSpecificTiles);

        // Update if Tile Chain quest
        if (currentLvl.QuestTileChain != null)
        {
            if (currentLvl.QuestTileChain.Length > 0)
                QuestsManager.InitQuestTileChain(currentLvl.QuestTileChain[0], currentLvl.NumberTileChain);
        }

        // Update if Tile Count
        if (currentLvl.QuestTileCount != null)
        {
            if (currentLvl.QuestTileCount.Length > 0)
                QuestsManager.InitQuestTileCount(currentLvl.QuestTileCount[0], currentLvl.NumberTileCount);
        }

        // Update Dialogs
        ScreensManager.Instance.InitDialogs(_levelData[_currentLevel].DialogBeginning, true);
        ScreensManager.Instance.InitCharaName(_levelData[_currentLevel].CharacterName);
        ScreensManager.Instance.InitQuestDescription(_levelData[_currentLevel].QuestDescription,
            _levelData[_currentLevel].QuestImage);

        // Init Level
        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap)
    {
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
                }
            }
        }
    }

    private void InitObj(GameObject which, int x, int y, AllStates state)
    {
        if (state == AllStates.None) return;

        float hexOffset = 0;
        if (x % 2 == 1)
            hexOffset = HALF_OFFSET;

        // Tp ground to its position
        which.transform.position = new Vector3(x * _distance * QUARTER_OFFSET, 0, (y + hexOffset) * _distance);

        // Get Groudn State Manager
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

        // Init Crystal or not
        Vector2Int[] coordsByCurrentLvl = _mapConstructData.Coords.ToArray();
        foreach (var crystalsCoords in coordsByCurrentLvl)
        {
            if (crystalsCoords.x != x || crystalsCoords.y != y) continue;

            which.GetComponent<CrystalsGround>().UpdateCrystals(true, true);
            return;
        }

        which.GetComponent<CrystalsGround>().UpdateCrystals(false, true);
    }

    private void Update()
    {
        // Right click to Reset
        if (Input.GetMouseButtonDown(1))
        {
            ResetButtonSelected();
            // ResetAroundSelectedPrevisu();
            RecyclingManager.Instance.UpdateRecycling(false);

            ResetPrevisu();
        }
    }

    public void UpdateMap()
    {
        UpdateGround?.Invoke();
    }

    public void UpdateSecondBlocForce()
    {
        if (_stockPlayerForceSwap.Count == 0) return;

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
    }

    public void ChangeActivatedButton(GameObject button)
    {
        // Activate or not the UI Button's indicator and update if one was selected or not

        if (IsGroundFirstSelected) return;

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
        // Update if first swap
        if (!_hasFirstSwap)
        {
            _hasFirstSwap = true;
            ResetAllPlayerForceSwaped();
        }

        // Update map
        _mapGrid[newCoords.x, newCoords.y] = _lastGroundSelected;
        _mapGrid[_lastGroundCoordsSelected.x, _lastGroundCoordsSelected.y] = which;

        // Change position
        (_lastGroundSelected.transform.position, which.transform.position) =
            (which.transform.position, _lastGroundSelected.transform.position);

        // Get GroundStateManager 
        var gLastGroundSelected = _lastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Protect these blocs a transformation
        gLastGroundSelected.IsProtected = true;
        gWhich.IsProtected = true;

        // Change coords inside of GroundManager
        gLastGroundSelected.ChangeCoords(newCoords);
        gWhich.ChangeCoords(_lastGroundCoordsSelected);

        // Reset selection's color of the two Grounds
        gLastGroundSelected.ResetIndicator();
        gWhich.ResetIndicator();

        // Update Ground Around
        gLastGroundSelected.UpdateGroundsAround();
        gWhich.UpdateGroundsAround();

        // Get Bloc to UI
        if (_hasInventory)
        {
            var tileToAdd = ConditionManager.Instance.GetState(gLastGroundSelected.GetCurrentStateEnum(),
                gWhich.GetCurrentStateEnum());
            // SetupUIGround.Instance.AddNewGround((int)tileToAdd);
            ItemCollectedManager.Instance.SpawnFBGroundCollected(gLastGroundSelected.GetGroundPrevisu((int)tileToAdd),
                String.Empty, tileToAdd);
        }

        // Spend energy
        EnergyManager.Instance.ReduceEnergyBySwap();

        // Get crystals if have crystals
        which.GetComponent<CrystalsGround>().UpdateCrystals(false, false);
        _lastGroundSelected.GetComponent<CrystalsGround>().UpdateCrystals(false, false);

        // Bloc for Next Swap
        if (_blockLastGroundsSwapped)
        {
            gWhich.JustBeenSwaped = true;
            gLastGroundSelected.JustBeenSwaped = true;
            gWhich.UpdateFBReloadEnergy(true);
            gLastGroundSelected.UpdateFBReloadEnergy(true);

            ResetTwoLastSwapped();

            _lastGroundSwaped[0] = gWhich;
            _lastGroundSwaped[1] = gLastGroundSelected;
        }


        //ResetLastSelected
        IsGroundFirstSelected = false;
        // ResetAroundSelectedPrevisu();
        ResetGroundSelected();
        ResetPrevisu();
        // CheckForBiome();

        QuestsManager.CheckQuest();

        // Check Game Over is no recycling
        if (!_hasRecycling)
            CheckIfGameOver();

        // Reset protect
        gLastGroundSelected.IsProtected = false;
        gWhich.IsProtected = false;
    }

    public void GroundSwapPrevisu(GameObject which)
    {
        if (!_hasPrevisu) return;

        // Reset old ground entered
        ResetPrevisu();

        // Get GroundStateManager 
        var gLastGroundSelected = _lastGroundSelected.GetComponent<GroundStateManager>();
        var gWhich = which.GetComponent<GroundStateManager>();

        // Protect these blocs a transformation
        gLastGroundSelected.IsProtectedPrevisu = true;
        gWhich.IsProtectedPrevisu = true;

        // Update Ground Around
        gWhich.UpdateGroundsAroundPrevisu(gLastGroundSelected.GetCurrentStateEnum());
        gLastGroundSelected.UpdateGroundsAroundPrevisu(gWhich.GetCurrentStateEnum());

        // Reset protect
        gLastGroundSelected.IsProtectedPrevisu = false;
        gWhich.IsProtectedPrevisu = false;

        // Update their previsu
        gWhich.ChangeStatePrevisu(gLastGroundSelected.GetCurrentStateEnum());
        gLastGroundSelected.ChangeStatePrevisu(gWhich.GetCurrentStateEnum());
    }

    public void GroundSwapPrevisuButton(GameObject which, AllStates buttonState)
    {
        if (!_hasPrevisu) return;

        // Reset old ground entered
        ResetPrevisu();
        
        var gWhich = which.GetComponent<GroundStateManager>();

        gWhich.IsProtectedPrevisu = true;
        gWhich.UpdateGroundsAroundPrevisu(buttonState);
        gWhich.IsProtectedPrevisu = false;

        gWhich.ChangeStatePrevisu(buttonState);
    }

    public void UseRecycling()
    {
        if (LastObjButtonSelected == null) return;

        if(!_hasInfinitRecycling)
            NbOfRecycling--;
        LastObjButtonSelected.GetComponent<UIButton>().UpdateNumberLeft(-1);
        EnergyManager.Instance.EarnEnergyByRecycling();
        SetupUIGround.Instance.FollowDndDeactivate();
        RecyclingManager.Instance.UpdateRecycling(false);
        RecyclingManager.Instance.UpdateNbRecyclingLeft();
        ResetButtonSelected();
        ResetTwoLastSwapped();
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
        return LastObjButtonSelected.GetComponent<UIButton>().GetNumberLeft() <= 0;
    }

    public void CheckIfGroundSelected(GameObject which, Vector2Int newCoords)
    {
        if (LastObjButtonSelected != null) return;

        // If was checkAround -> go swap
        if (_lastGroundSelected != null)
            GroundSwap(which, newCoords);
        else
            CheckAroundGroundSelected(which, newCoords);
    }

    public void CheckIfGameOver()
    {
        if (IsVictory) return;

        StartCoroutine(WaitLittleToCheck());
    }

    IEnumerator WaitLittleToCheck()
    {
        yield return new WaitForSeconds(.02f);

        bool inventory = EnergyManager.Instance.IsEnergyInferiorToCostLandingGround() || !_hasInventory;

        if (EnergyManager.Instance.IsEnergyInferiorToCostSwap()
            && inventory
            && !SetupUIGround.Instance.CheckIfStillGround())
        {
            ScreensManager.Instance.GameOver();
        }
    }

    public AllStates GetLastStateSelected()
    {
        return _lastGroundSelected != null
            ? _lastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum()
            : LastStateButtonSelected;
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
        return _levelData[_currentLevel].DialogEnd;
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

        SetupUIGround.Instance.ResetAllButtons();
        ItemCollectedManager.Instance.DeleteAllFB();
        IsVictory = false;

        ChangeLevel(nextLevel);
    }

    public void RestartLevel()
    {
        ResetAllMap(false);
        ResetAllSelection();
        ResetButtonSelected();
        ResetGroundSelected();
        SetupUIGround.Instance.ResetAllButtons();
        ScreensManager.Instance.RestartSceneOrLevel();
        ResetAllMap(false);

        // InitializeMap();
    }

    public void ResetButtonSelected()
    {
        ChangeActivatedButton(null);
    }

    public void ResetGroundSelected()
    {
        _lastGroundSelected = null;
        _lastGroundCoordsSelected = new Vector2Int(-1, -1);
    }

    public void ResetAllSelection()
    {
        ResetSelection?.Invoke();
    }

    public void ResetTwoLastSwapped()
    {
        if (_lastGroundSwaped[0] != null)
            _lastGroundSwaped[0].UpdateNoSwap(false);
        if (_lastGroundSwaped[1] != null)
            _lastGroundSwaped[1].UpdateNoSwap(false);

        _lastGroundSwaped[0] = null;
        _lastGroundSwaped[1] = null;
    }

    public void ResetPrevisu()
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
    }

    private void ResetAllPlayerForceSwaped()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_mapGrid[x, y] == null) continue;
                if (_mapGrid[x, y].GetComponent<GroundStateManager>() == null) continue;

                var ground = _mapGrid[x, y].GetComponent<GroundStateManager>();
                ground.IsPlayerForceSwapBlocked = false;
                ground.GetFbArrow().gameObject.SetActive(false);
            }
        }
    }

    // private void OldInitializeLevel(Vector2Int sizeMap) //Map creation
    // {
    //     for (int x = 0; x < sizeMap.x; x++)
    //     {
    //         for (int y = 0; y < sizeMap.y; y++)
    //         {
    //             // Get the string of the actual line
    //             string line = _mapInfo[y];
    //             // Get the actual char of the string of the actual line
    //             char whichEnvironment = line[x];
    //
    //             switch (whichEnvironment)
    //             {
    //                 case PLAIN:
    //                     GameObject plains = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(plains, x, y, AllStates.Plain);
    //                     break;
    //                 case DESERT:
    //                     GameObject desert = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(desert, x, y, AllStates.Desert);
    //                     break;
    //                 case WATER:
    //                     GameObject water = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(water, x, y, AllStates.Water);
    //                     break;
    //                 case TROPICAL:
    //                     GameObject tropical = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(tropical, x, y, AllStates.Tropical);
    //                     break;
    //                 case SAVANNA:
    //                     GameObject savanna = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(savanna, x, y, AllStates.Savanna);
    //                     break;
    //                 case GEYSER:
    //                     GameObject geyser = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(geyser, x, y, AllStates.Geyser);
    //                     break;
    //                 case SNOW:
    //                     GameObject snow = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(snow, x, y, AllStates.Snow);
    //                     break;
    //                 case POLAR_DESERT:
    //                     GameObject polar = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(polar, x, y, AllStates.PolarDesert);
    //                     break;
    //                 case TUNDRA:
    //                     GameObject tundra = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(tundra, x, y, AllStates.Tundra);
    //                     break;
    //                 case SWAMP:
    //                     GameObject swamp = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(swamp, x, y, AllStates.Swamp);
    //                     break;
    //                 case MOUNTAIN:
    //                     GameObject mountain = Instantiate(_groundPrefab, _map.transform);
    //                     InitObj(mountain, x, y, AllStates.Mountain);
    //                     break;
    //                 case NONE:
    //                     break;
    //             }
    //         }
    //     }
    // }

    // public void PrevisuAroundSelected(AllStates state)
    // {
    //     if (_lastGroundSelected != null)
    //         _lastGroundSelected.GetComponent<GroundStateManager>().SelectedLaunchAroundPrevisu(state);
    // }

    // public void ResetCurrentEntered()
    // {
    //     _currentEntered = null;
    // }

    // public void ResetCurrentAroundSelectedPrevisu()
    // {
    //     if (_currentEntered != null)
    //         _currentEntered.ResetAroundSelectedPrevisu();
    // }

    // public void ResetAroundSelectedPrevisu()
    // {
    //     if (_lastGroundSelected != null)
    //         _lastGroundSelected.GetComponent<GroundStateManager>().ResetAroundSelectedPrevisu();
    // }
}