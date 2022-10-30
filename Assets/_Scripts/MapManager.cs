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
    private GameObject _lastBlocSelected;
    private Vector2Int _lastCoordsSelected;
    private GameObject[,] _mapGrid;
    private int[,] _tempGrid;
    private GameObject _spawnPoint = null;
    private Vector2 _coordsSpawnPoint = Vector2.zero;

    private List<GameObject> _groundArounded = new List<GameObject>();

    private Vector2Int[] _directionsOne = new Vector2Int[]
        { new(0, 0),new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private Vector2Int[] _directionsTwo = new Vector2Int[]
        { new(2, 0), new(-2, 0), new(0, 2), new(0, -2) };

    private const char GROUND_WHITE = '0';
    private const char GROUND_GREY = '1';
    private const char GROUND_NAV = 'N';
    private const char GROUND_HARD = 'H';
    private const char GROUND_CANT_BE_MOVE = 'C';

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
        _mapGrid = new GameObject[_mapSize.x, _mapSize.y];
        _tempGrid = new int[_mapSize.x, _mapSize.y];
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
                        // Tp ground to its position
                        go.transform.position = new Vector3(j, 0,
                            sizeMap.x - 1 - i);
                        // Change coords of the ground
                        go.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        // Update _mapGrid
                        _mapGrid[j, sizeMap.x - 1 - i] = go;
                        // Spawn anim
                        MapSpawnAnim(go);
                        break;

                    case GROUND_GREY:
                        GameObject go1 = Instantiate(_environment[1], _map.transform);
                        go1.transform.position = new Vector3(j, 0,
                            sizeMap.x - 1 - i);
                        go1.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));

                        _mapGrid[j, sizeMap.x - 1 - i] = go1;
                        MapSpawnAnim(go1);
                        break;
                    case GROUND_NAV:
                        GameObject go3 = Instantiate(_environment[3], _map.transform);
                        var position = go3.transform.position;
                        position = new Vector3(j, 0,
                            sizeMap.x - 1 - i);
                        go3.transform.position = position;
                        go3.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        // Only this block create a NavMesh
                        NavigationBaker.Instance.surfaces.Add(go3.GetComponent<NavMeshSurface>());
                        // // Init the spawn point coords
                        // _coordsSpawnPoint = new Vector2(position.x, position.z);
                        // print("_coordsSpawnPoint" + _coordsSpawnPoint);
                        _spawnPoint = go3;

                        _mapGrid[j, sizeMap.x - 1 - i] = go3;
                        MapSpawnAnim(go3);
                        break;
                    case GROUND_HARD:
                        GameObject go2 = Instantiate(_environment[2], _map.transform);
                        go2.transform.position = new Vector3(j, 0,
                            sizeMap.x - 1 - i);

                        _mapGrid[j, sizeMap.x - 1 - i] = go2;
                        MapSpawnAnim(go2);
                        break;
                    case GROUND_CANT_BE_MOVE:
                        GameObject go4 = Instantiate(_environment[4], _map.transform);
                        go4.transform.position = new Vector3(j, 0,
                            sizeMap.x - 1 - i);
                        go4.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));

                        _mapGrid[j, sizeMap.x - 1 - i] = go4;
                        MapSpawnAnim(go4);
                        break;
                }
            }
        }

        // Rotate the map for the isometric view / need to do it after the creation or change the InitializeLevel
        _map.transform.Rotate(new Vector3(0, 45, 0));
        //Update spawn point coords
        _coordsSpawnPoint = new Vector2(_spawnPoint.transform.position.x, _spawnPoint.transform.position.z);
        // Build the NavMesh
        NavigationBaker.Instance.BuildNavSurface();
        //Active the player
        _player.SetActive(true);
        // Change the coords of the Player
        _player.GetComponent<PlayerMovement>().ChangeCoords(new Vector3(_coordsSpawnPoint.x - .9f, 0, _coordsSpawnPoint.y - .2f));
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

    public void CheckIfSelected(GameObject which, Vector2Int newCoords)
    {
        // If was checkAround -> go swap
        if (_tempGrid[newCoords.x, newCoords.y] == 1)
            Swap(which, newCoords);
        else
            CheckAroundGroundSelected(which, newCoords);
    }

    private void Swap(GameObject which, Vector2Int newCoords)
    {
        // Update map
        _mapGrid[newCoords.x, newCoords.y] = _lastBlocSelected;
        _mapGrid[_lastCoordsSelected.x, _lastCoordsSelected.y] = which;
        // Change position
        (_lastBlocSelected.transform.position, which.transform.position) =
            (which.transform.position, _lastBlocSelected.transform.position);
        // Change coords inside of GroundManager
        _lastBlocSelected.GetComponent<GroundManager>().GroundCoords = newCoords;
        which.GetComponent<GroundManager>().GroundCoords = _lastCoordsSelected;
        // Reset selection's color of the two Grounds
        _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
        which.GetComponent<GroundManager>().ResetMat();
        // Reset selection
        ResetTempGrid();
        ResetLastSelected();
    }

    private void CheckAroundGroundSelected(GameObject which, Vector2Int coords)
    {
        // Reset to start from scratch
        ResetLastSelected();
        ResetTempGrid();
        // Update lastSelected if need to call Swap() after
        _lastBlocSelected = which;
        _lastCoordsSelected = coords;
        foreach (var dir in _directionsOne)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) continue;
            // Check if something exist
            if (_mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>()) continue;
            // Check if ground CanBeMoved
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().CanBeMoved) continue;
            // Check if not actually selected -> Security
            if (_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().IsSelected) continue;
            // It's good
            _mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().OnAroundedSelected();
            _tempGrid[newPos.x, newPos.y] = 1;
            _groundArounded.Add(_mapGrid[newPos.x, newPos.y]);
        }
    }

    private const int IMPOSSIBLE_GROUND_COORDS = 5000;

    // ReSharper disable Unity.PerformanceAnalysis
    private void ResetLastSelected()
    {
        // Reset mat
        if (_lastBlocSelected != null)
            _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
        // Reset lastSelected and put big big coords to avoid problem with x = 0 or y =0
        _lastBlocSelected = null;
        _lastCoordsSelected = Vector2Int.one * IMPOSSIBLE_GROUND_COORDS;
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
                _tempGrid[x, y] = 0;
            }
        }
    }

    public void ChangeMode() // Called By EditMode Button
    {
        IsEditMode = !IsEditMode;
        _isEditModeText.text = IsEditMode ? _exploModeString : _editModeString;
        ResetLastSelected();
        ResetTempGrid();
        if (IsEditMode) CheckAroundPlayer();
    }

    private void CheckAroundPlayer()
    {
        print("CheckAroundPlayer");
        var position = _player.transform.localPosition;
        Vector2Int coords = new Vector2Int((int)Mathf.Round(position.x), (int)Mathf.Round(position.z));
        switch (_distanceGroundAroundPlayer)
        {
            case 1:
                foreach (var dir in _directionsOne)
                {
                    Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
                    // Check if inside of array
                    if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) continue;
                    // Check if something exist
                    var map = _mapGrid[newPos.x, newPos.y];
                    if (map == null) continue;
                    // Check if has GroundManager
                    if (!map.GetComponent<GroundManager>()) continue;
                    // Check if ground CanBeMoved
                    if (!map.GetComponent<GroundManager>().CanBeMoved) continue;
                    // Make it arounded
                    map.GetComponent<GroundManager>().OnAroundedPlayer();
                }

                break;
            case 2:
                break;
        }
    }
}