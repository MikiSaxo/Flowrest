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


    private const char FLOOR = '0';
    private const char WALL = '1';
    
    private void Start()
    {
        string map = Application.dataPath + "/Map.txt";
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

                    // case _startPlayer:
                    //     GameObject go3 = Instantiate(_environment[0], _map.transform);
                    //     go3.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     MapSpawnAnim(go3);
                    //
                    //     GameObject player = Instantiate(_dynamicObjects[0], _map.transform);
                    //     _player = player;
                    //     player.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     MapSpawnAnim(player);
                    //     _playerPos = new Vector2Int(j, _xSizeMap - 1 - i);
                    //     print(_playerPos);
                    //     break;
                    //
                    // case _blueBox:
                    //     GameObject go4 = Instantiate(_environment[0], _map.transform);
                    //     go4.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     MapSpawnAnim(go4);
                    //
                    //     GameObject blueBox = Instantiate(_dynamicObjects[1], _map.transform);
                    //     blueBox.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     _boxColor.Add(blueBox);
                    //     _boxColorPos.Add(new Vector2Int(j, _xSizeMap - 1 - i));
                    //     MapSpawnAnim(blueBox);
                    //     _boxGrid[j, _xSizeMap - 1 - i] = blueBox;
                    //     _dynamicGrid[j, _xSizeMap - 1 - i] = 2;
                    //     break;
                    //
                    // case _blueBoxEnd:
                    //     GameObject go5 = Instantiate(_environment[0], _map.transform);
                    //     go5.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     MapSpawnAnim(go5);
                    //
                    //     GameObject blueBoxEnd = Instantiate(_environment[2], _map.transform);
                    //     blueBoxEnd.transform.position = new Vector2(j * _distance - _xOffset, _xSizeMap - 1 - i * _distance - _yOffset);
                    //     _boxColorEndPos.Add(new Vector2Int(j, _xSizeMap - 1 - i));
                    //     MapSpawnAnim(blueBoxEnd);
                    //     _solidGrid[j, _xSizeMap - 1 - i] = 2;
                    //     break;
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
}
