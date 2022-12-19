using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine.UIElements;
using UnityEngine.AI;
using TMPro;

public class n_MapManager : MonoBehaviour
{
    public event Action UpdateGround;

    public GameObject[,] MapGrid;

    [Header("Setup")] [SerializeField] private GameObject _map = null;

    [SerializeField] private GameObject[] _environment = null;

    private string[] _mapInfo;
    public Vector2Int _mapSize;

    // private GameObject _lastGroundSelected;
    // private Vector2Int _lastGroundCoordsSelected;
    // private int[,] _tempGroundSelectedGrid;

    // private List<GameObject> _groundArounded = new List<GameObject>();
    // private List<GameObject> _groundAroundedPlayer = new List<GameObject>();

    // private Vector2Int[] _directionsOne = new Vector2Int[]
    // { new(0, 0), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    // private Vector2Int[] _directionsTwo = new Vector2Int[]
    // { new(2, 0), new(-2, 0), new(0, 2), new(0, -2) };

    private const char PLAINS = 'P';
    private const char DESERT = 'D';
    private const char WATER = 'W';


    public static n_MapManager Instance;

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
                    case PLAINS:
                        GameObject plains = Instantiate(_environment[0], _map.transform);
                        InitObj(plains, x, y, 0);
                        plains.GetComponent<GroundStateManager>().ChangeValues(50, 20);
                        plains.GetComponent<GroundStateManager>().SwitchState(new GroundPlainsState());
                        break;
                    case DESERT:
                        GameObject desert = Instantiate(_environment[0], _map.transform);
                        InitObj(desert, x, y, 1);
                        desert.GetComponent<GroundStateManager>().ChangeValues(0, 35);
                        desert.GetComponent<GroundStateManager>().SwitchState(new GroundDesertState());
                        break;
                    case WATER:
                        GameObject water = Instantiate(_environment[0], _map.transform);
                        water.GetComponent<GroundStateManager>().ChangeValues(100, 10);
                        water.GetComponent<GroundStateManager>().SwitchState(new GroundWaterState());
                        InitObj(water, x, y, 2);
                        break;
                }
            }
        }
    }

    private void InitObj(GameObject which, int x, int y, int materialNb)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(x, 0, y);
        // Change coords of the ground
        which.GetComponent<GroundStateManager>().ChangeMaterials(materialNb);
        which.GetComponent<GroundStateManager>().ChangeCoords(new Vector2Int(x, y));
        // Update _mapGrid
        MapGrid[x, y] = which;
    }

    public void UpdateMap()
    {
        UpdateGround?.Invoke();
    }
}