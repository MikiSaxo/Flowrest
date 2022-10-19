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
    [SerializeField] private GameObject _triggerAroundSelected = null;

    private string[] _mapInfo;
    private int _xSize = 0;
    private int _ySize = 0;
    private float _xOffset = 0;
    private float _yOffset = 0;
    private int[,] _solidGrid;
    private GameObject _lastBlocSelected;
    private Vector2 _lastCoordsSelected;
    private int _howManyGroundSelected = 0;


    private const char GROUND1 = '0';
    private const char GROUND2 = '1';
    private const char GROUND3 = '2';

    public static MapManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        string map = Application.dataPath + "/Map-DontMove/Map.txt";
        _mapInfo = File.ReadAllLines(map);
        _xSize = _mapInfo[0].Length;
        _ySize = _mapInfo.Length;

        _solidGrid = new int[_xSize, _ySize];

        _xOffset = _distance * _xSize * .5f - (_distance * .5f);
        _yOffset = _distance * _ySize - (_distance * .5f);
        InitializeLevel(_xSize, _ySize);
        ResetLastSelected();
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
                    case GROUND1:
                        GameObject go = Instantiate(_environment[0], _map.transform);
                        go.transform.position = new Vector3(j * _distance - _xOffset, 0,
                            _xSizeMap - 1 - i * _distance - _yOffset);
                        MapSpawnAnim(go);
                        _solidGrid[j, _xSizeMap - 1 - i] = 0;
                        go.GetComponent<GroundManager>().ChangeCoords(new Vector2(j, _xSizeMap - 1 - i));
                        break;

                    case GROUND2:
                        GameObject go2 = Instantiate(_environment[1], _map.transform);
                        go2.transform.position = new Vector3(j * _distance - _xOffset, 0,
                            _xSizeMap - 1 - i * _distance - _yOffset);
                        MapSpawnAnim(go2);
                        _solidGrid[j, _xSizeMap - 1 - i] = 1;
                        go2.GetComponent<GroundManager>().ChangeCoords(new Vector2(j, _xSizeMap - 1 - i));
                        break;
                    case GROUND3:
                        GameObject go3 = Instantiate(_environment[2], _map.transform);
                        go3.transform.position = new Vector3(j * _distance - _xOffset, 0,
                            _xSizeMap - 1 - i * _distance - _yOffset);
                        MapSpawnAnim(go3);
                        // _solidGrid[j, _xSizeMap - 1 - i] = 2;
                        // go3.GetComponent<GroundManager>().ChangeCoords(new Vector2(j, _xSizeMap - 1 - i));
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

    // ReSharper disable Unity.PerformanceAnalysis
    public void LookIfNextTo(GameObject which, Vector2 coords)
    {
        _howManyGroundSelected++;

        var getXCoord = Mathf.Abs(coords.x - _lastCoordsSelected.x);
        var getYCoord = Mathf.Abs(coords.y - _lastCoordsSelected.y);
        if (getXCoord == 1 && getYCoord == 0 || getXCoord == 0 && getYCoord == 1)
        {
            print("swap");

            var getLastBlocPos = _lastBlocSelected.transform.position;
            var getLastCoord = _lastCoordsSelected;

            _lastBlocSelected.transform.position = which.transform.position;
            _lastBlocSelected.GetComponent<GroundManager>().GroundCoords =
                which.GetComponent<GroundManager>().GroundCoords;
            _lastBlocSelected.GetComponent<GroundManager>().OnLeaved(true);

            which.transform.position = getLastBlocPos;
            which.GetComponent<GroundManager>().GroundCoords = getLastCoord;
            which.GetComponent<GroundManager>().OnLeaved(true);

            ResetLastSelected();
            _howManyGroundSelected = 0;
        }
        else
        {
            if (_howManyGroundSelected >= 2)
            {
                if (_lastBlocSelected != null)
                    _lastBlocSelected.GetComponent<GroundManager>().OnLeaved(true);
                ResetLastSelected();
            }

            _lastBlocSelected = which;
            _lastCoordsSelected = coords;
            _triggerAroundSelected.transform.position = _lastBlocSelected.transform.position;
        }
    }

    private void ResetLastSelected()
    {
        _lastBlocSelected = null;
        _lastCoordsSelected = Vector2.one * 500;
        _triggerAroundSelected.transform.position = new Vector3(0, -100, 0); 

        if(_howManyGroundSelected > 0)
            _howManyGroundSelected--;
    }
}