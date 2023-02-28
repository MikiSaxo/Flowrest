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

    // public int TemperatureSelected { get; set; }
    public bool IsGroundFirstSelected { get; set; }

    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;
    [SerializeField] private float _distance;
    [SerializeField] private int _nbMaxLevel;

    private int _actualLevel;
    private bool _isDragNDrop;
    private string[] _mapInfo;
    private Vector2Int _lastGroundCoordsSelected;
    private GameObject _lastGroundSelected;
    // private GroundStateManager _currentEntered;

    // private Vector2Int[] _directionsOne = new Vector2Int[]
    // { new(0, 0), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    // private Vector2Int[] _directionsTwo = new Vector2Int[]
    // { new(2, 0), new(-2, 0), new(0, 2), new(0, -2) };

    // public int _lastNbButtonSelected;
    // private int[,] _tempGroundSelectedGrid;

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
    private const char SWAMP = 'M';

    private const float QUARTER_OFFSET = .75f;
    private const float HALF_OFFSET = .5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeMap();
        LastStateButtonSelected = AllStates.None;
    }

    private void InitializeMap()
    {
        var mapName = $"Level{_actualLevel}";
        // Get the text map
        string map = Application.streamingAssetsPath + $"/Map-Init/{mapName}.txt";
        _mapInfo = File.ReadAllLines(map);
        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;
        // Init the grids
        MapGrid = new GameObject[_mapSize.x, _mapSize.y];

        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap) //Map creation
    {
        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                // Get the string of the actual line
                string line = _mapInfo[y];
                // Get the actual char of the string of the actual line
                char whichEnvironment = line[x];

                switch (whichEnvironment)
                {
                    case PLAIN:
                        GameObject plains = Instantiate(_groundPrefab, _map.transform);
                        InitObj(plains, x, y, AllStates.Plain);
                        break;
                    case DESERT:
                        GameObject desert = Instantiate(_groundPrefab, _map.transform);
                        InitObj(desert, x, y, AllStates.Desert);
                        break;
                    case WATER:
                        GameObject water = Instantiate(_groundPrefab, _map.transform);
                        InitObj(water, x, y, AllStates.Water);
                        break;
                    case TROPICAL:
                        GameObject tropical = Instantiate(_groundPrefab, _map.transform);
                        InitObj(tropical, x, y, AllStates.Tropical);
                        break;
                    case SAVANNA:
                        GameObject savanna = Instantiate(_groundPrefab, _map.transform);
                        InitObj(savanna, x, y, AllStates.Savanna);
                        break;
                    case GEYSER:
                        GameObject geyser = Instantiate(_groundPrefab, _map.transform);
                        InitObj(geyser, x, y, AllStates.Geyser);
                        break;
                    case SNOW:
                        GameObject snow = Instantiate(_groundPrefab, _map.transform);
                        InitObj(snow, x, y, AllStates.Snow);
                        break;
                    case POLAR_DESERT:
                        GameObject polar = Instantiate(_groundPrefab, _map.transform);
                        InitObj(polar, x, y, AllStates.PolarDesert);
                        break;
                    case TUNDRA:
                        GameObject tundra = Instantiate(_groundPrefab, _map.transform);
                        InitObj(tundra, x, y, AllStates.Tundra);
                        break;
                    case SWAMP:
                        GameObject swamp = Instantiate(_groundPrefab, _map.transform);
                        InitObj(swamp, x, y, AllStates.Swamp);
                        break;
                    case NONE:
                        break;
                }
            }
        }
    }

    private void InitObj(GameObject which, int x, int y, AllStates state)
    {
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
        if (_actualLevel < _nbMaxLevel - 1)
            _actualLevel++;
        InitializeMap();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click to Reset
            ResetButtonSelected();
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

        // Prevent to use an actual empty button
        if (button != null)
        {
            if (button.GetComponent<UIButton>().GetNumberLeft() <= 0)
                return;
        }

        // Deactivate the last one selected
        if (LastObjButtonSelected != null)
            LastObjButtonSelected.GetComponent<UIButton>().NeedActivateSelectedIcon(false);
        // Update the current selected or if no one was selected -> can be null
        LastObjButtonSelected = button;

        if (LastObjButtonSelected != null)
        {
            _isDragNDrop = false;
            LastObjButtonSelected.GetComponent<UIButton>().NeedActivateSelectedIcon(true);
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

    // public void ChangeCurrentTemperature(int temperature)
    // {
    //     TemperatureSelected = temperature;
    // }

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

        // Change coords inside of GroundManager
        _lastGroundSelected.GetComponent<GroundStateManager>().ChangeCoords(newCoords);
        which.GetComponent<GroundStateManager>().ChangeCoords(_lastGroundCoordsSelected);

        // Reset selection's color of the two Grounds
        _lastGroundSelected.GetComponent<GroundStateManager>().ResetIndicator();
        which.GetComponent<GroundStateManager>().ResetIndicator();
        _lastGroundSelected.GetComponent<GroundStateManager>().UpdateGroundsAround();
        which.GetComponent<GroundStateManager>().UpdateGroundsAround();

        //Get Bloc to UI
        var tileToAdd = ConditionManager.Instance.GetState(
            _lastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum(),
            which.GetComponent<GroundStateManager>().GetCurrentStateEnum());
        SetupUIGround.Instance.AddNewGround((int)tileToAdd);
        GroundCollected.Instance.StartAnim(_lastGroundSelected.GetComponent<GroundStateManager>()
            .GetGroundPrevisu((int)tileToAdd));
        print(tileToAdd + " added");

        //ResetLastSelected
        IsGroundFirstSelected = false;
        ResetAroundSelectedPrevisu();
        ResetGroundSelected();
        // CheckForBiome();
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
            _lastGroundSelected.GetComponent<GroundStateManager>().SelectedActivateIconPrevisu(state);
    }

    // public void SetCurrentEntered(GroundStateManager ground)
    // {
    //     _currentEntered = ground;
    // }

    public bool GetIsDragNDrop()
    {
        return _isDragNDrop;
    }

    public int GetActualLevel()
    {
        return _actualLevel;
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

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public AllStates GetLastGroundSelected()
    {
        return _lastGroundSelected != null
            ? _lastGroundSelected.GetComponent<GroundStateManager>().GetCurrentStateEnum()
            : LastStateButtonSelected;
    }
}