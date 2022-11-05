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

public class MapManager : MonoBehaviour
{
    public static bool IsEditMode = false;
    public GameObject[,] MapGrid;

    [Header("Setup")] [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _map = null;

    [SerializeField] private GameObject[] _environment = null;

    //[SerializeField] private float _distance = 0;
    [SerializeField] private TextMeshProUGUI _isEditModeText = null;
    [Range(1, 4)] [SerializeField] private int _distanceGroundAroundPlayer;

    [Header("UI")] [SerializeField] private string _exploModeString;
    [SerializeField] private string _editModeString;

    private string[] _mapInfo;

    private Vector2Int _mapSize;

    //private Vector2 _mapOffset;
    private GameObject _lastGroundSelected;
    private Vector2Int _lastGroundCoordsSelected;
    private int[,] _tempGroundSelectedGrid;
    private GameObject _spawnPoint = null;
    private Vector2 _coordsSpawnPoint = Vector2.zero;

    private List<GameObject> _groundArounded = new List<GameObject>();
    private List<GameObject> _groundAroundedPlayer = new List<GameObject>();

    private Vector2Int[] _directionsOne = new Vector2Int[]
        { new(0, 0), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private Vector2Int[] _directionsTwo = new Vector2Int[]
        { new(2, 0), new(-2, 0), new(0, 2), new(0, -2) };

    private const char GROUND_WHITE = '0';
    private const char GROUND_GREY = '1';
    private const char GROUND_NAV = 'N';
    private const char GROUND_HARD = 'H';
    private const char GROUND_CANT_BE_MOVE = 'C';
    private const char WATER_FLOWING = 'W';
    private const char WATER_SOURCE = 'S';

    private const char DIR_NS = 'I';
    private const char DIR_WE = '-';

    private const char DIR_NW = '⅃';
    private const char DIR_NE = 'L';
    private const char DIR_SW = '<';//ꓶ
    private const char DIR_SE = 'Γ';

    private const char DIR_NSW = 'b';
    private const char DIR_NSE = 'd';
    private const char DIR_NWE = '⊥';
    private const char DIR_SWE = 'T';

    private const char DIR_NSWE = '+';
    //I-L⅃<Γbd⊥T+

    public WaterData[] waterData;
    public event Action CheckWaterSource;
    public event Action ChangeModeEvent;

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get the text map
        string map = Application.dataPath + "/Map-DontMove/Map.txt";
        _mapInfo = File.ReadAllLines(map);
        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;
        // Init the grids
        MapGrid = new GameObject[_mapSize.x, _mapSize.y];
        _tempGroundSelectedGrid = new int[_mapSize.x, _mapSize.y];
        // Init the offset
        //_mapOffset.x = _distance * _mapSize.x * .5f - (_distance * .5f);
        //_mapOffset.y = _distance * _mapSize.y; // - (_distance * .5f);
        // Start init
        InitializeLevel(_mapSize);
        // Reset the lastSelected and tempGrid
        ResetLastSelected();
        ResetTempGrid();
        // Actualize the EditMode Button
        _isEditModeText.text = IsEditMode ? _exploModeString : _editModeString;
    }

    private void InitializeLevel(Vector2Int sizeMap) //Map creation
    {
        for (int i = 0; i < sizeMap.x; i++)
        {
            for (int j = 0; j < sizeMap.y; j++)
            {
                // Get the string of the actual line
                string line = _mapInfo[i];
                // Get the actual char of the string of the actual line
                char whichEnvironment = line[j];

                switch (whichEnvironment)
                {
                    case GROUND_WHITE:
                        // Instantiate the good ground into the map parent
                        GameObject go = Instantiate(_environment[0], _map.transform);
                        InitObj(go, i, j, true);
                        break;

                    case GROUND_GREY:
                        GameObject go1 = Instantiate(_environment[1], _map.transform);
                        InitObj(go1, i, j, true);
                        break;

                    case GROUND_NAV:
                        GameObject go3 = Instantiate(_environment[3], _map.transform);
                        InitObj(go3, i, j, true);
                        // Only this block create a NavMesh
                        NavigationBaker.Instance.surfaces.Add(go3.GetComponent<NavMeshSurface>());
                        _spawnPoint = go3;
                        break;

                    case GROUND_HARD:
                        GameObject go2 = Instantiate(_environment[2], _map.transform);
                        InitObj(go2, i, j, false);
                        break;
                    case WATER_SOURCE:
                        GameObject go5 = Instantiate(_environment[6], _map.transform);
                        InitObj(go5, i, j, false);
                        go5.GetComponent<WaterSourceManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        break;

                    case GROUND_CANT_BE_MOVE:
                        GameObject go4 = Instantiate(_environment[4], _map.transform);
                        InitObj(go4, i, j, true);
                        break;

                    case WATER_FLOWING:
                        GameObject go6 = Instantiate(_environment[5], _map.transform);
                        InitObj(go6, i, j, true);
                        break;
                    case DIR_NS:
                        GameObject ns = Instantiate(_environment[5], _map.transform);
                        InitObj(ns, i, j, true);
                        InitWater(ns, DIR_NS);
                        break;
                    case DIR_WE:
                        GameObject we = Instantiate(_environment[5], _map.transform);
                        InitObj(we, i, j, true);
                        InitWater(we, DIR_WE);
                        break;
                    case DIR_NW:
                        GameObject nw = Instantiate(_environment[5], _map.transform);
                        InitObj(nw, i, j, true);
                        InitWater(nw, DIR_NW);
                        break;
                    case DIR_NE:
                        GameObject ne = Instantiate(_environment[5], _map.transform);
                        InitObj(ne, i, j, true);
                        InitWater(ne, DIR_NE);
                        break;
                    case DIR_SW:
                        GameObject sw = Instantiate(_environment[5], _map.transform);
                        InitObj(sw, i, j, true);
                        InitWater(sw, DIR_SW);
                        break;
                    case DIR_SE:
                        GameObject se = Instantiate(_environment[5], _map.transform);
                        InitObj(se, i, j, true);
                        InitWater(se, DIR_SE);
                        break;
                    case DIR_NSW:
                        GameObject nsw = Instantiate(_environment[5], _map.transform);
                        InitObj(nsw, i, j, true);
                        InitWater(nsw, DIR_NSW);
                        break;
                    case DIR_NSE:
                        GameObject nse = Instantiate(_environment[5], _map.transform);
                        InitObj(nse, i, j, true);
                        InitWater(nse, DIR_NSE);
                        break;
                    case DIR_NWE:
                        GameObject nwe = Instantiate(_environment[5], _map.transform);
                        InitObj(nwe, i, j, true);
                        InitWater(nwe, DIR_NWE);
                        break;
                    case DIR_SWE:
                        GameObject swe = Instantiate(_environment[5], _map.transform);
                        InitObj(swe, i, j, true);
                        InitWater(swe, DIR_SWE);
                        break;
                    case DIR_NSWE:
                        GameObject nswe = Instantiate(_environment[5], _map.transform);
                        InitObj(nswe, i, j, true);
                        InitWater(nswe, DIR_NSWE);
                        break;
                }
            }
        }
        // Rotate the map for the isometric view / need to do it after the creation or change the InitializeLevel
        //_map.transform.Rotate(new Vector3(0, 45, 0));
        //Update spawn point coords
        _coordsSpawnPoint = new Vector2(_spawnPoint.transform.position.x, _spawnPoint.transform.position.z);
        // Build the NavMesh
        NavigationBaker.Instance.BuildNavSurface();
        //Active the player
        _player.SetActive(true);
        // Change the coords of the Player
        _player.GetComponent<PlayerMovement>()
            .ChangeCoords(new Vector3(_coordsSpawnPoint.x, 0, _coordsSpawnPoint.y));
        // Update map to clean the start
        StartCoroutine(InitMap2());
    }

    private void InitObj(GameObject which, int i, int j, bool hasGroundManager)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(j, 0, _mapSize.x - 1 - i);
        // Change coords of the ground
        if (hasGroundManager)
            which.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, _mapSize.x - 1 - i));
        // Update _mapGrid
        MapGrid[j, _mapSize.x - 1 - i] = which;
        // Spawn anim
        MapSpawnAnim(which);
    }

    private void InitWater(GameObject which, char letter)
    {
        for (int i = 0; i < waterData.Length; i++)
        {
            if (waterData[i].WaterName == letter)
            {
                which.GetComponent<WaterFlowing>().waterData = waterData[i];
                break;
            }
            Debug.LogWarning("Didn't find the good waterData");
        }
    }

    IEnumerator InitMap2()
    {
        // Waiting to find a better system because it didn't work if it's launched just after the InitializeLevel
        yield return new WaitForSeconds(.1f);
        CheckWaterSource?.Invoke();
        ChangeModeEvent?.Invoke();
    }

    private void MapSpawnAnim(GameObject which)
    {
        // Funny ground animation spawn
        which.transform.DOScale(Vector3.zero, 0f);
        which.transform.DOScale(Vector3.one, .5f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ChangeMode();
        }
    }

    public void CheckIfGroundSelected(GameObject which, Vector2Int newCoords)
    {
        // If was checkAround -> go swap
        if (_tempGroundSelectedGrid[newCoords.x, newCoords.y] == 1)
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
        _lastGroundSelected.GetComponent<GroundManager>().GroundCoords = newCoords;
        which.GetComponent<GroundManager>().GroundCoords = _lastGroundCoordsSelected;
        // Reset selection's color of the two Grounds
        _lastGroundSelected.GetComponent<GroundManager>().ResetMat();
        which.GetComponent<GroundManager>().ResetMat();
        // Reset selection
        ResetTempGrid();
        ResetLastSelected();
        ResetLastSelectedPlayer();
        // Reboot the detection zone 
        CheckAroundPlayer();
        // Call the event to test all the water
        CheckWaterSource?.Invoke();
    }

    private void CheckAroundGroundSelected(GameObject which, Vector2Int coords)
    {
        // Reset to start from scratch
        ResetLastSelected();
        ResetTempGrid();
        ResetLastSelectedPlayer();
        CheckAroundPlayer();
        // Update lastSelected if need to call Swap() after
        _lastGroundSelected = which;
        _lastGroundCoordsSelected = coords;
        foreach (var dir in _directionsOne)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) continue;
            // Check if something exist
            if (MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!MapGrid[newPos.x, newPos.y].GetComponent<GroundManager>()) continue;
            // Check if ground CanBeMoved
            if (!MapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().CanBeMoved) continue;
            // Check if not actually selected -> Security
            if (MapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().IsSelected) continue;
            // It's good
            MapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().OnAroundedSelected();
            // Update temp grid
            _tempGroundSelectedGrid[newPos.x, newPos.y] = 1;
            // Add to the_groundArounded list to reset mat after
            _groundArounded.Add(MapGrid[newPos.x, newPos.y]);
        }
    }

    private const int IMPOSSIBLE_GROUND_COORDS = 5000;

    public void ChangeMode() // Called By EditMode Button
    {
        // Change the mode
        IsEditMode = !IsEditMode;
        // Change the EditMode button text
        _isEditModeText.text = IsEditMode ? _exploModeString : _editModeString;
        // Make a reset of everything
        ResetLastSelected();
        ResetTempGrid();
        ResetLastSelectedPlayer();
        // If EditMode create a detection zone
        if (IsEditMode) CheckAroundPlayer();
        // Call the event to activate or not the indicators
        ChangeModeEvent?.Invoke();
    }

    private void CheckAroundPlayer()
    {
        // Get the LOCAL pos of the player
        var position = _player.transform.localPosition;
        // Get the int of player's coords
        Vector2Int coords = new Vector2Int((int)Mathf.Round(position.x), (int)Mathf.Round(position.z));
        // Create a different zone according to the _distanceGroundAroundPlayer  
        switch (_distanceGroundAroundPlayer)
        {
            case 1:
                foreach (var dir in _directionsOne)
                {
                    Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
                    CheckIfGoodAroundedPlayer(newPos);
                }

                break;
            case 2:
                foreach (var dir in _directionsTwo)
                {
                    Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
                    CheckIfGoodAroundedPlayer(newPos);
                }

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int newPos = new Vector2Int(coords.x + x, coords.y + y);
                        CheckIfGoodAroundedPlayer(newPos);
                    }
                }

                break;
        }
    }

    private void CheckIfGoodAroundedPlayer(Vector2Int newPos)
    {
        // Check if inside of array
        if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) return;
        // Check if something exist
        var map = MapGrid[newPos.x, newPos.y];
        if (map == null) return;
        // Check if has GroundManager
        if (!map.GetComponent<GroundManager>()) return;
        // Check if ground CanBeMoved
        if (!map.GetComponent<GroundManager>().CanBeMoved) return;
        // Make it arounded
        map.GetComponent<GroundManager>().OnAroundedPlayer();
        // Add to the_groundAroundedPlayer list to reset mat after
        _groundAroundedPlayer.Add(MapGrid[newPos.x, newPos.y]);
    }

    private void ResetLastSelected()
    {
        // Reset mat
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<GroundManager>().ResetMat();
        // Reset lastSelected and put big big coords to avoid problem with x = 0 or y =0
        _lastGroundSelected = null;
        _lastGroundCoordsSelected = Vector2Int.one * IMPOSSIBLE_GROUND_COORDS;
        // Reset mat of all ground selected before
        foreach (var ground in _groundArounded)
        {
            ground.GetComponent<GroundManager>().ResetMat();
        }

        // Clear the _groundArounded List
        _groundArounded.Clear();
    }

    private void ResetTempGrid()
    {
        // Reset the tempGrid for the ground arounded
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                _tempGroundSelectedGrid[x, y] = 0;
            }
        }
    }

    private void ResetLastSelectedPlayer()
    {
        // Reset mat of all ground selected before
        foreach (var ground in _groundAroundedPlayer)
        {
            ground.GetComponent<GroundManager>().ResetBaseMat();
        }

        // Clear the _groundAroundedPlayer List
        _groundAroundedPlayer.Clear();
    }
}