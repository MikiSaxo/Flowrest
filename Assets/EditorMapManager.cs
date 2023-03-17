using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMapManager : MonoBehaviour
{
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _map;
    [SerializeField] private GameObject _groundEditorPrefab;
    [SerializeField] private float _distance;

    private char[,] _mapGrid;

    private const char NONE = 'N';
    private const char PLAIN = 'P';
    private const char DESERT = 'D';
    private const char WATER = 'W';
    private const char TROPICAL = 'T';
    private const char SAVANNA = 'S';
    private const char GEYSER = 'H';
    private const char SNOW = 'G';
    private const char POLAR_DESERT = 'O';
    private const char TUNDRA = 'U';
    private const char SWAMP = 'A';
    private const char MOUNTAIN = 'M';

    private const float QUARTER_OFFSET = .85f;
    private const float HALF_OFFSET = .5f;

    private void Start()
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        // Init the grids
        _mapGrid = new char[_mapSize.x, _mapSize.y];

        // Init Level
        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap)
    {
        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                GameObject ground = Instantiate(_groundEditorPrefab, _map.transform);
                InitObj(ground, x, y);
            }
        }
    }

    private void InitObj(GameObject ground, int x, int y)
    {
        float hexOffset = 0;
        if (x % 2 == 1)
            hexOffset = HALF_OFFSET;

        ground.transform.position = new Vector3(x * _distance * QUARTER_OFFSET, 0, (y + hexOffset) * _distance);
        
        _mapGrid[x, y] = NONE;

    }
}