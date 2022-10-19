using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject _map = null;
    [SerializeField] private GameObject[] _environment = null;
    [SerializeField] private float _distance = 0;

    private string[] _mapInfo;
    private int _xSize = 0;
    private int _ySize = 0;
    private float _xOffset = 0;
    private float _yOffset = 0;
    private int[,] _solidGrid;
    private GameObject _lastBlocSelected; 


    private const char FLOOR = '0';
    private const char WALL = '1';

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string map = Application.dataPath + "/Map-DontTouch/Map.txt";
        _mapInfo = File.ReadAllLines(map);
        _xSize = _mapInfo[0].Length;
        _ySize = _mapInfo.Length;

        _solidGrid = new int[_xSize, _ySize];

        _xOffset = _distance * _xSize * .5f - (_distance * .5f);
        _yOffset = _distance * _ySize - (_distance * .5f);
        InitializeLevel(_xSize, _ySize);
    }

    private void InitializeLevel(int _xSizeMap, int _ySizeMap)
    {
        for (int i = 0; i < _xSizeMap; i++)
        {
            for (int j = 0; j < _ySizeMap; j++)
            {
                string line = _mapInfo[i];
                char _whichEnvironment = line[j];

                switch (_whichEnvironment)
                {
                    case FLOOR:
                        GameObject go = Instantiate(_environment[0], _map.transform);
                        go.transform.position = new Vector3(j * _distance - _xOffset, 0, _xSizeMap - 1 - i * _distance - _yOffset);
                        MapSpawnAnim(go);
                        _solidGrid[j, _xSizeMap - 1 - i] = 0;
                        break;

                    case WALL:
                        GameObject go2 = Instantiate(_environment[1], _map.transform);
                        go2.transform.position = new Vector3(j * _distance - _xOffset, 0, _xSizeMap - 1 - i * _distance - _yOffset);
                        MapSpawnAnim(go2);
                        _solidGrid[j, _xSizeMap - 1 - i] = 1;
                        break;

                }
            }
        }
        
        _map.transform.Rotate(new Vector3(0,45,0));
    }

    private void MapSpawnAnim(GameObject _which)
    {
        _which.transform.DOScale(Vector3.zero, 0f);
        _which.transform.DOScale(Vector3.one, .5f);
    }

    public void DeselectedOldBloc(GameObject which)
    {
        if(_lastBlocSelected != null)
            _lastBlocSelected.GetComponent<GroundManager>().OnLeaved(true);
        _lastBlocSelected = which;
    }
}
