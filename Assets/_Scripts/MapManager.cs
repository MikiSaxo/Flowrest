using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UIElements;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject[] _environment = null;

    [SerializeField] private float _distance = 0;
    // [SerializeField] private GameObject _triggerAroundSelected = null;

    private string[] _mapInfo;
    private Vector2Int _mapSize;
    private Vector2 _mapOffset;
    private GameObject _lastBlocSelected;
    private Vector2Int _lastCoordsSelected;
    private GameObject[,] _mapGrid;
    private int[,] _tempGrid;

    private List<GameObject> _groundArounded = new List<GameObject>();
    private Vector2Int[] _directions = new Vector2Int[]
        { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private const char GROUND_WHITE = '0';
    private const char GROUND_GREY = '1';
    private const char GROUND_HARD = 'H';

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string map = Application.dataPath + "/Map-DontMove/Map.txt";
        _mapInfo = File.ReadAllLines(map);
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;

        _mapGrid = new GameObject[_mapSize.x, _mapSize.y];
        _tempGrid = new int[_mapSize.x, _mapSize.y];

        _mapOffset.x = _distance * _mapSize.x * .5f - (_distance * .5f);
        _mapOffset.y = _distance * _mapSize.y - (_distance * .5f);
        InitializeLevel(_mapSize);
        ResetLastSelected();
        ResetTempGrid();
    }

    private void InitializeLevel(Vector2Int sizeMap)
    {
        for (int i = 0; i < sizeMap.x; i++)
        {
            for (int j = 0; j < sizeMap.y; j++)
            {
                string line = _mapInfo[i];
                char whichEnvironment = line[j];

                switch (whichEnvironment)
                {
                    case GROUND_WHITE:
                        GameObject go = Instantiate(_environment[0], _map.transform);
                        go.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y);
                        MapSpawnAnim(go);
                        _mapGrid[j, sizeMap.x - 1 - i] = go;
                        go.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        break;

                    case GROUND_GREY:
                        GameObject go1 = Instantiate(_environment[1], _map.transform);
                        go1.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y);
                        MapSpawnAnim(go1);
                        _mapGrid[j, sizeMap.x - 1 - i] = go1;
                        go1.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        break;
                    case GROUND_HARD:
                        GameObject go2 = Instantiate(_environment[2], _map.transform);
                        go2.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y);
                        MapSpawnAnim(go2);
                        _mapGrid[j, sizeMap.x - 1 - i] = go2;
                        break;
                }
            }
        }

        _map.transform.Rotate(new Vector3(0, 45, 0));
    }

    private void MapSpawnAnim(GameObject which)
    {
        which.transform.DOScale(Vector3.zero, 0f);
        which.transform.DOScale(Vector3.one, .5f);
    }

    public void CheckIfSelected(GameObject which, Vector2Int newCoords)
    {
        //if was check around -> go swap
        if (_tempGrid[newCoords.x, newCoords.y] == 1)
            Swap(which, newCoords);
        else
            CheckAround(which, newCoords);
    }

    private void Swap(GameObject which, Vector2Int newCoords)
    {
        //update map
        _mapGrid[newCoords.x, newCoords.y] = _lastBlocSelected;
        _mapGrid[_lastCoordsSelected.x, _lastCoordsSelected.y] = which;
        //change position
        (_lastBlocSelected.transform.position, which.transform.position) = (which.transform.position, _lastBlocSelected.transform.position);
        //change coords inside of GroundManager
        _lastBlocSelected.GetComponent<GroundManager>().GroundCoords = newCoords;
        which.GetComponent<GroundManager>().GroundCoords = _lastCoordsSelected;
        //reset selection's color of the two Grounds
        _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
        which.GetComponent<GroundManager>().ResetMat();
        //reset selection
        ResetTempGrid();
        ResetLastSelected();
    }

    private void CheckAround(GameObject which, Vector2Int coords)
    {
        ResetLastSelected();
        ResetTempGrid();
        _lastBlocSelected = which;
        _lastCoordsSelected = coords;
        foreach (var dir in _directions)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            //check if inside of array
            if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) continue;
            //check if something exist
            if (_mapGrid[newPos.x, newPos.y] == null) continue;
            //check if has GroundManager
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>()) continue;
            //check if ground CanBeMoved
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().CanBeMoved) continue;
            //check if not actually selected -> Security
            if (_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().IsSelected) continue;
            //it's okay
            _mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().OnArounded();
            _tempGrid[newPos.x, newPos.y] = 1;
            _groundArounded.Add(_mapGrid[newPos.x, newPos.y]);
        }
    }

    private void ResetLastSelected()
    {
        if(_lastBlocSelected != null)
            _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
            
        _lastBlocSelected = null;
        _lastCoordsSelected = Vector2Int.one * 500;

        foreach (var ground in _groundArounded)
        {
            ground.GetComponent<GroundManager>().ResetMat();
        }
        _groundArounded.Clear();
    }
    private void ResetTempGrid()
    {
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                _tempGrid[x, y] = 0;
            }
        }
    }
}