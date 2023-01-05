using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;


public class n_MapManager : MonoBehaviour
{
    public static n_MapManager Instance;

    public event Action UpdateGround;
    public event Action CheckBiome;
    public event Action ResetSelection;

    public Vector2Int _mapSize;
    public GameObject[,] MapGrid;
    public int LastNbButtonSelected { get; set; }
    public GameObject LastButtonSelected { get; set; }
    public int TemperatureSelected { get; set; }
    public bool IsGroundFirstSelected { get; set; }

    [Header("Setup")] [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject _groundPrefab = null;

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

    private const char PLAIN = 'P';
    private const char DESERT = 'D';
    private const char WATER = 'W';

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get the text map
        string map = Application.dataPath + "/Map-Init/nMap.txt";
        _mapInfo = File.ReadAllLines(map);
        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;
        // Init the grids
        MapGrid = new GameObject[_mapSize.x, _mapSize.y];
        //_tempGroundSelectedGrid = new int[_mapSize.x, _mapSize.y];
        // _tempGroundSelectedGrid = new int[_mapSize.x, _mapSize.y];
        InitializeLevel(_mapSize);
        // ResetLastSelected();
        // ResetTempGrid();
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
                        InitObj(plains, x, y, 0);
                        break;
                    case DESERT:
                        GameObject desert = Instantiate(_groundPrefab, _map.transform);
                        InitObj(desert, x, y, 1);
                        break;
                    case WATER:
                        GameObject water = Instantiate(_groundPrefab, _map.transform);
                        InitObj(water, x, y, 2);
                        break;
                }
            }
        }
    }

    private void InitObj(GameObject which, int x, int y, int stateNb)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(x * 10, 0, y * 10);
        // Change coords of the ground
        which.GetComponent<GroundStateManager>().ChangeCoords(new Vector2Int(x, y));
        //Init state of ground
        which.GetComponent<GroundStateManager>().InitState(stateNb);
        // Update _mapGrid
        MapGrid[x, y] = which;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click to Reset
        {
            ResetButtonSelected();
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

        if (button != null) // Prevent to use an actual empty button
        {
            if (button.GetComponent<nUIButton>().GetNumberLeft() <= 0)
                return;
        }

        if (LastButtonSelected != null) // Deactivate the last one selected
            LastButtonSelected.GetComponent<nUIButton>().NeedActivateSelectedIcon(false);
        // Update the current selected or if no one was selected -> can be null
        LastButtonSelected = button;

        if (LastButtonSelected != null)
        {
            _isDragNDrop = false;
            LastButtonSelected.GetComponent<nUIButton>().NeedActivateSelectedIcon(true);

            if (!LastButtonSelected.GetComponent<nUIButton>().GetIsTemperature())
            {
                LastNbButtonSelected = LastButtonSelected.GetComponent<nUIButton>().GetStateButton();
                TemperatureSelected = 0;
            }
            else
                TemperatureSelected = LastButtonSelected.GetComponent<nUIButton>().GetHisTemperature();
            // FollowMouseDND.Instance.CanMove = true;
        }
        else
        {
            _isDragNDrop = true;
            LastNbButtonSelected = -1;
            TemperatureSelected = 0;
        }
    }

    public void ChangeCurrentTemperature(int temperature)
    {
        TemperatureSelected = temperature;
    }

    public bool CanPoseBloc()
    {
        return LastButtonSelected.GetComponent<nUIButton>().GetNumberLeft() > 0;
    }

    public void DecreaseNumberButton()
    {
        LastButtonSelected.GetComponent<nUIButton>().ChangeNumberLeft(-1);
    }

    public bool CheckIfButtonIsEmpty()
    {
        return LastButtonSelected.GetComponent<nUIButton>().GetNumberLeft() <= 0;
    }

    public void CheckIfGroundSelected(GameObject which, Vector2Int newCoords)
    {
        if (LastButtonSelected != null) return;

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

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}