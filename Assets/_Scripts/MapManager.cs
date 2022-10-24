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
        //Get the text map
        string map = Application.dataPath + "/Map-DontMove/Map.txt";
        _mapInfo = File.ReadAllLines(map);
        //Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;
        //Init the grids
        _mapGrid = new GameObject[_mapSize.x, _mapSize.y];
        _tempGrid = new int[_mapSize.x, _mapSize.y];
        //Init the offset
        _mapOffset.x = _distance * _mapSize.x * .5f - (_distance * .5f);
        _mapOffset.y = _distance * _mapSize.y - (_distance * .5f);
        //Start init
        InitializeLevel(_mapSize);
        //Reset the lastSelected and tempGrid
        ResetLastSelected();
        ResetTempGrid();
    }

    private void InitializeLevel(Vector2Int sizeMap) //Map creation
    {
        for (int i = 0; i < sizeMap.x; i++)
        {
            for (int j = 0; j < sizeMap.y; j++)
            {
                //Get the string of the actual line
                string line = _mapInfo[i];
                //Get the actual char of the string of the actual line
                char whichEnvironment = line[j];

                switch (whichEnvironment)
                {
                    case GROUND_WHITE:
                        //Instantiate the good ground into the map parent
                        GameObject go = Instantiate(_environment[0], _map.transform); 
                        //Tp ground to its position
                        go.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y); 
                        //Change coords of the ground
                        go.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i)); 
                        //Update _mapGrid
                        _mapGrid[j, sizeMap.x - 1 - i] = go; 
                        //Spawn anim
                        MapSpawnAnim(go);
                        break;

                    case GROUND_GREY:
                        GameObject go1 = Instantiate(_environment[1], _map.transform);
                        go1.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y);
                        go1.GetComponent<GroundManager>().ChangeCoords(new Vector2Int(j, sizeMap.x - 1 - i));
                        
                        _mapGrid[j, sizeMap.x - 1 - i] = go1;
                        MapSpawnAnim(go1);
                        break;
                    case GROUND_HARD:
                        GameObject go2 = Instantiate(_environment[2], _map.transform);
                        go2.transform.position = new Vector3(j * _distance - _mapOffset.x, 0,
                            sizeMap.x - 1 - i * _distance - _mapOffset.y);
                        
                        _mapGrid[j, sizeMap.x - 1 - i] = go2;
                        MapSpawnAnim(go2);
                        break;
                }
            }
        }
        //Rotate the map for the isometric view / need to do it after the creation or change the InitializeLevel
        _map.transform.Rotate(new Vector3(0, 45, 0));
    }

    private void MapSpawnAnim(GameObject which)
    {
        //Funny ground animation spawn
        which.transform.DOScale(Vector3.zero, 0f);
        which.transform.DOScale(Vector3.one, .5f);
    }

    public void CheckIfSelected(GameObject which, Vector2Int newCoords)
    {
        //If was checkAround -> go swap
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
        //Change position
        (_lastBlocSelected.transform.position, which.transform.position) = (which.transform.position, _lastBlocSelected.transform.position);
        //Change coords inside of GroundManager
        _lastBlocSelected.GetComponent<GroundManager>().GroundCoords = newCoords;
        which.GetComponent<GroundManager>().GroundCoords = _lastCoordsSelected;
        //Reset selection's color of the two Grounds
        _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
        which.GetComponent<GroundManager>().ResetMat();
        //Reset selection
        ResetTempGrid();
        ResetLastSelected();
    }

    private void CheckAround(GameObject which, Vector2Int coords)
    {
        //Reset to start from scratch
        ResetLastSelected();
        ResetTempGrid();
        //Update lastSelected if need to call Swap() after
        _lastBlocSelected = which;
        _lastCoordsSelected = coords;
        foreach (var dir in _directions)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            //Check if inside of array
            if (newPos.x < 0 || newPos.x >= _mapSize.x || newPos.y < 0 || newPos.y >= _mapSize.y) continue;
            //Check if something exist
            if (_mapGrid[newPos.x, newPos.y] == null) continue;
            //Check if has GroundManager
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>()) continue;
            //Check if ground CanBeMoved
            if (!_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().CanBeMoved) continue;
            //Check if not actually selected -> Security
            if (_mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().IsSelected) continue;
            //It's good
            _mapGrid[newPos.x, newPos.y].GetComponent<GroundManager>().OnArounded();
            _tempGrid[newPos.x, newPos.y] = 1;
            _groundArounded.Add(_mapGrid[newPos.x, newPos.y]);
        }
    }

    private const int BIG_COORDS = 5000;
    private void ResetLastSelected()
    {
        //Reset mat
        if(_lastBlocSelected != null)
            _lastBlocSelected.GetComponent<GroundManager>().ResetMat();
        //Reset lastSelected and put big big coords to avoid problem with x = 0 or y =0
        _lastBlocSelected = null;
        _lastCoordsSelected = Vector2Int.one * BIG_COORDS;
        //Reset mat of all ground selected before
        foreach (var ground in _groundArounded)
        {
            ground.GetComponent<GroundManager>().ResetMat();
        }
        //Clear the _groundArounded List
        _groundArounded.Clear();
    }
    private void ResetTempGrid()
    {
        //Reset the tempGrid for the ground arounded
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                _tempGrid[x, y] = 0;
            }
        }
    }
}