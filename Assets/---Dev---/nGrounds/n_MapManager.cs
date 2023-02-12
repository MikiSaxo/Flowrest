using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


public class n_MapManager : MonoBehaviour
{
    public static n_MapManager Instance;
    public event Action UpdateGround;
    public event Action CheckBiome;
    public event Action ResetSelection;

    public Vector2Int _mapSize;
    public GameObject[,] MapGrid;
    public AllStates LastStateButtonSelected { get; set; }
    public GameObject LastObjButtonSelected { get; set; }
    public int TemperatureSelected { get; set; }
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
    private const char SAVANE = 'S';
    private const char HOT_SPRING = 'H';

    private const float QUARTER_OFFSET = .75f;
    private const float HALF_OFFSET = .5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeMap();
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
                    case SAVANE:
                        GameObject savane = Instantiate(_groundPrefab, _map.transform);
                        InitObj(savane, x, y, AllStates.Savanna);
                        break;
                    case HOT_SPRING:
                        GameObject hotSpring = Instantiate(_groundPrefab, _map.transform);
                        InitObj(hotSpring, x, y, AllStates.Geyser);
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
        if (_actualLevel < _nbMaxLevel-1)
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

        if (button != null) // Prevent to use an actual empty button
        {
            if (button.GetComponent<nUIButton>().GetNumberLeft() <= 0)
                return;
        }

        if (LastObjButtonSelected != null) // Deactivate the last one selected
            LastObjButtonSelected.GetComponent<nUIButton>().NeedActivateSelectedIcon(false);
        // Update the current selected or if no one was selected -> can be null
        LastObjButtonSelected = button;

        if (LastObjButtonSelected != null)
        {
            _isDragNDrop = false;
            LastObjButtonSelected.GetComponent<nUIButton>().NeedActivateSelectedIcon(true);

            if (!LastObjButtonSelected.GetComponent<nUIButton>().GetIsTemperature())
            {
                LastStateButtonSelected = LastObjButtonSelected.GetComponent<nUIButton>().GetStateButton();
                TemperatureSelected = 0;
            }
            else
                TemperatureSelected = LastObjButtonSelected.GetComponent<nUIButton>().GetHisTemperature();
            // FollowMouseDND.Instance.CanMove = true;
        }
        else
        {
            _isDragNDrop = true;
            LastStateButtonSelected = AllStates.None;
            TemperatureSelected = 0;
        }
    }

    public void ChangeCurrentTemperature(int temperature)
    {
        TemperatureSelected = temperature;
    }

    public bool CanPoseBloc()
    {
        return LastObjButtonSelected.GetComponent<nUIButton>().GetNumberLeft() > 0;
    }

    public void DecreaseNumberButton()
    {
        LastObjButtonSelected.GetComponent<nUIButton>().ChangeNumberLeft(-1);
    }

    public bool CheckIfButtonIsEmpty()
    {
        return LastObjButtonSelected.GetComponent<nUIButton>().GetNumberLeft() <= 0;
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
        //ResetLastSelected
        IsGroundFirstSelected = false;
        ResetGroundSelected();

        CheckForBiome();
    }

    private void CheckAroundGroundSelected(GameObject which, Vector2Int coords)
    {
        // Reset to start from scratch
        ResetGroundSelected();
        // Update lastSelected if need to call Swap() after
        _lastGroundSelected = which;
        _lastGroundCoordsSelected = coords;
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

    public bool GetIsDragNDrop()
    {
        return _isDragNDrop;
    }

    public int GetActualLevel()
    {
        return _actualLevel;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}