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
    public event Action CheckBiome;
    public event Action ResetSelection;

    public Vector2Int _mapSize;
    public GameObject[,] MapGrid;
    public AllStates LastStateButtonSelected { get; set; }

    public GameObject LastObjButtonSelected { get; set; }

    public bool IsGroundFirstSelected { get; set; }
    public bool HasInventory { get; private set; }
    public bool HasTrashCan { get; private set; }
    public QuestManager QuestsManager;

    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;
    [SerializeField] private float _distance;

    [Header("Level")] [SerializeField] private string _levelName;
    [SerializeField] private int _levelTotalNumber;

    [Header("Data")] [SerializeField] private LevelData[] _levelData;

    private int _currentLevel;
    private bool _isDragNDrop;
    private string[] _mapInfo;
    private Vector2Int _lastGroundCoordsSelected;
    private GameObject _lastGroundSelected;

    private Dictionary<char, AllStates> dico = new Dictionary<char, AllStates>();

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

    private const float QUARTER_OFFSET = .85f;
    private const float HALF_OFFSET = .5f;

    // [SerializeField] private float _quaterOffset; 
    // [SerializeField] private float _halfOffset; 

    private void Awake()
    {
        Instance = this;
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
        var mapName = $"{_levelName}{_currentLevel}";

        // Get the text map
        string map = Application.streamingAssetsPath + $"/Map-Init/{mapName}.txt";
        _mapInfo = File.ReadAllLines(map);

        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;

        // Init the grids
        MapGrid = new GameObject[_mapSize.x, _mapSize.y];

        // Init energy
        CrystalsManager.Instance.InitEnergy(_levelData[_currentLevel].EnergyAtStart);

        // Update if has inventory
        HasInventory = _levelData[_currentLevel].HasInventory;
        if (!HasInventory)
            SetupUIGround.Instance.NoInventory();

        // Update if has trash can
        HasTrashCan = _levelData[_currentLevel].HasTrashCan;
        SetupUIGround.Instance.SetIfHasInvetory(HasTrashCan);

        // Update if full floor quest
        if (_levelData[_currentLevel].IsFullFloor)
            QuestsManager.InitQuestFullFloor(_levelData[_currentLevel].WhichStateFloor);
        
        // Update if flower quest
        if(_levelData[_currentLevel].IsFlower)
            QuestsManager.InitQuestFlower(_levelData[_currentLevel].WhichStateFlower);
        
        // Update if No Specific Tile quest
        if(_levelData[_currentLevel].IsNoSpecificTiles)
            QuestsManager.InitQuestNoSpecificTiles(_levelData[_currentLevel].WhichStateNoSpecificTiles);
        
        // Update Dialogs
        ScreensManager.Instance.InitDialogs(_levelData[_currentLevel].DialogToDisplayAtTheBeginning, true);

        // Init Level
        InitializeLevel(_mapSize);
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
                    GameObject test = Instantiate(_groundPrefab, _map.transform);
                    InitObj(test, x, y, dico[whichEnvironment]);
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

        // Change coords of the ground
        which.GetComponent<GroundStateManager>().ChangeCoords(new Vector2Int(x, y));

        //Init state of ground
        which.GetComponent<GroundStateManager>().InitState(state);

        // Update _mapGrid
        MapGrid[x, y] = which;

        // Init Crystal or not
        Vector2Int[] coordsByCurrentLvl = _levelData[_currentLevel].Coords;
        foreach (var crystalsCoords in coordsByCurrentLvl)
        {
            if (crystalsCoords.x != x || crystalsCoords.y != y) continue;

            which.GetComponent<CrystalsGround>().UpdateCrystals(true, true);
            return;
        }

        which.GetComponent<CrystalsGround>().UpdateCrystals(false, true);
    }

    public void ResetAllMap()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                Destroy(MapGrid[x, y]);
                MapGrid[x, y] = null;
            }
        }

        ChangeLevel();
    }

    private void ChangeLevel()
    {
        if (_currentLevel < _levelTotalNumber - 1)
            _currentLevel++;

        InitializeMap();
    }

    private void Update()
    {
        // Right click to Reset
        if (Input.GetMouseButtonDown(1))
        {
            ResetButtonSelected();
            ResetAroundSelectedPrevisu();
            TrashCrystalManager.Instance.UpdateTrashCan(false);
        }
    }

    public void UpdateMap()
    {
        UpdateGround?.Invoke();
    }

    public void CheckForBiome()
    {
        CheckBiome?.Invoke();
    }

    // Activate or not the UI Button's indicator and update if one was selected or not
    public void ChangeActivatedButton(GameObject button)
    {
        if (IsGroundFirstSelected) return;

        // Activate Trash can
        if (button != null)
        {
            if (HasTrashCan)
                TrashCrystalManager.Instance.UpdateTrashCan(true);
            SetupUIGround.Instance.GroundStockage.ForcedOpen = true;
        }
        else
            SetupUIGround.Instance.GroundStockage.ForcedOpen = false;

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

            // TemperatureSelected = 0;
            // if (!LastObjButtonSelected.GetComponent<UIButton>().GetIsTemperature())
            // {
            // }
            // else
            //     TemperatureSelected = LastObjButtonSelected.GetComponent<UIButton>().GetHisTemperature();
            //FollowMouseDND.Instance.CanMove = true;
        }
        else
        {
            _isDragNDrop = true;
            LastStateButtonSelected = AllStates.None;
            // TemperatureSelected = 0;
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

    private void GroundSwap(GameObject which, Vector2Int newCoords)
    {
        // Update map
        MapGrid[newCoords.x, newCoords.y] = _lastGroundSelected;
        MapGrid[_lastGroundCoordsSelected.x, _lastGroundCoordsSelected.y] = which;

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
        gLastGroundSelected.UpdateGroundsAround();
        gWhich.UpdateGroundsAround();

        // Get Bloc to UI
        if (HasInventory)
        {
            var tileToAdd = ConditionManager.Instance.GetState(gLastGroundSelected.GetCurrentStateEnum(),
                gWhich.GetCurrentStateEnum());
            SetupUIGround.Instance.AddNewGround((int)tileToAdd);
            ItemCollectedManager.Instance.SpawnFBGroundCollected(gLastGroundSelected.GetGroundPrevisu((int)tileToAdd),
                String.Empty);
        }

        // Spend energy
        CrystalsManager.Instance.ReduceEnergyBySwap();

        // Get crystals if have crystals
        which.GetComponent<CrystalsGround>().UpdateCrystals(false, false);
        _lastGroundSelected.GetComponent<CrystalsGround>().UpdateCrystals(false, false);

        //ResetLastSelected
        IsGroundFirstSelected = false;
        ResetAroundSelectedPrevisu();
        ResetGroundSelected();
        // CheckForBiome();
        
        QuestsManager.CheckQuest();

        // Reset protect
        gLastGroundSelected.IsProtected = false;
        gWhich.IsProtected = false;
    }

    private void CheckAroundGroundSelected(GameObject which, Vector2Int coords)
    {
        // Reset to start from scratch
        ResetGroundSelected();
        // Update lastSelected if need to call Swap() after
        _lastGroundSelected = which;
        _lastGroundCoordsSelected = coords;
    }

    public void PrevisuAroundSelected(AllStates state)
    {
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<GroundStateManager>().SelectedLaunchAroundPrevisu(state);
    }

    public void UseTrashCan()
    {
        // print("hello trash");

        if (LastObjButtonSelected == null) return;

        LastObjButtonSelected.GetComponent<UIButton>().UpdateNumberLeft(-1);
        CrystalsManager.Instance.EarnEnergyByRecycling();
        if(!_isDragNDrop)
            FollowMouseDND.Instance.AnimDeactivateObject();
        TrashCrystalManager.Instance.UpdateTrashCan(false);
        ResetButtonSelected();
    }

    public void CheckIfGameOver()
    {
        if (CrystalsManager.Instance.IsEnergyInferiorToCostSwap() &&
            CrystalsManager.Instance.IsEnergyInferiorToCostLandingGround() && !SetupUIGround.Instance.CheckIfGround())
        {
            ScreensManager.Instance.GameOver();
        }
    }

    public bool GetIsDragNDrop()
    {
        return _isDragNDrop;
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }

    public string[] GetDialogAtVictory()
    {
        return _levelData[_currentLevel].DialogToDisplayAtTheEnd;
    }

    public void RestartLevel()
    {
        ResetAllMap();
        ResetAllSelection();
        ResetButtonSelected();
        ResetGroundSelected();
        SetupUIGround.Instance.ResetAllButtons();
        ScreensManager.Instance.RestartSceneOrLevel();

        InitializeMap();
    }

    // public void ResetCurrentEntered()
    // {
    //     _currentEntered = null;
    // }

    // public void ResetCurrentAroundSelectedPrevisu()
    // {
    //     if (_currentEntered != null)
    //         _currentEntered.ResetAroundSelectedPrevisu();
    // }

    public void ResetAroundSelectedPrevisu()
    {
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<GroundStateManager>().ResetAroundSelectedPrevisu();
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

    // public void RestartScene()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    // }

    public AllStates GetLastStateSelected()
    {
        return _lastGroundSelected != null
            ? _lastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum()
            : LastStateButtonSelected;
    }
}