using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using TMPro;

public class EditorMapManager : MonoBehaviour
{
    public static EditorMapManager Instance;

    [SerializeField] private float _distance;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _groundsParent;
    [SerializeField] private GameObject _groundEditorPrefab;
    [SerializeField] private GameObject[] _hexGroundMeshes;

    private char _currentCharSelected;
    private char[,] _mapGrid;
    private string _mapName;
    private GameObject[,] _mapGridGround;
    private GameObject _lastObjButtonSelected { get; set; }
    private readonly Dictionary<char, GameObject> _groundDico = new Dictionary<char, GameObject>();

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
    
    private const char ENERGY = 'C';

    private const float QUARTER_OFFSET = .85f;
    private const float HALF_OFFSET = .5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _groundDico.Add(NONE, null);
        _groundDico.Add(PLAIN, _hexGroundMeshes[0]);
        _groundDico.Add(DESERT, _hexGroundMeshes[1]);
        _groundDico.Add(WATER, _hexGroundMeshes[2]);
        _groundDico.Add(TROPICAL, _hexGroundMeshes[3]);
        _groundDico.Add(SAVANNA, _hexGroundMeshes[4]);
        _groundDico.Add(GEYSER, _hexGroundMeshes[5]);
        _groundDico.Add(SNOW, _hexGroundMeshes[6]);
        _groundDico.Add(POLAR_DESERT, _hexGroundMeshes[7]);
        _groundDico.Add(TUNDRA, _hexGroundMeshes[8]);
        _groundDico.Add(SWAMP, _hexGroundMeshes[9]);
        _groundDico.Add(MOUNTAIN, _hexGroundMeshes[10]);

        _currentCharSelected = NONE;

        InitializeMap();
    }

    private void InitializeMap()
    {
        // Init the grids
        _mapGrid = new char[_mapSize.x, _mapSize.y];
        _mapGridGround = new GameObject[_mapSize.x, _mapSize.y];

        // Init Level
        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap)
    {
        for (int x = 0; x < sizeMap.x; x++)
        {
            for (int y = 0; y < sizeMap.y; y++)
            {
                GameObject ground = Instantiate(_groundEditorPrefab, _groundsParent.transform);
                InitObj(ground, x, y);
            }
        }
    }

    private void InitObj(GameObject ground, int x, int y)
    {
        ground.transform.DORotate(Vector3.zero,0);

        float hexOffset = 0;
        if (x % 2 == 1)
            hexOffset = HALF_OFFSET;

        ground.transform.position = new Vector3(x * _distance * QUARTER_OFFSET, 0, (y + hexOffset) * _distance);

        ground.GetComponent<EditorGroundManager>().UpdateCoords(x, y);

        _mapGrid[x, y] = NONE;
        _mapGridGround[x, y] = ground;
    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(1))
        // {
        //     if(_lastObjButtonSelected != null)
        //         _lastObjButtonSelected.GetComponent<UIButton>().ActivateSelectedIcon(false);
        //     _currentCharSelected = NONE;
        // }
    }

    public void SaveMap()
    {
        EditorSaveMap.Instance.UpdateMapName(_mapGrid);
    }

    public void UpdateCharSelected(string letter)
    {
        _currentCharSelected = letter[0];
    }

    public void UpdateMap(char letter, int x, int y)
    {
        _mapGrid[x, y] = letter;
    }

    public void UpdateMap(char letter, Vector2Int coords)
    {
        UpdateMap(letter, coords.x, coords.y);
    }

    public void ChangeActivatedButton(GameObject button)
    {
        // Deactivate the last one selected
        if (_lastObjButtonSelected != null)
            _lastObjButtonSelected.GetComponent<UIButton>().ActivateSelectedIcon(false);
        // Update the current selected or if no one was selected -> can be null
        _lastObjButtonSelected = button;

        if (_lastObjButtonSelected != null)
            _lastObjButtonSelected.GetComponent<UIButton>().ActivateSelectedIcon(true);
    }

    public void ResetAllMap()
    {
        for (int y = 0; y < _mapGrid.GetLength(1); y++) 
        {
            for (int x = 0; x < _mapGrid.GetLength(0); x++) 
            {
                _mapGridGround[x, y].GetComponent<EditorGroundManager>().DestroyGround();
            }
        }
    }

    public GameObject GetObjSelectedButton()
    {
        return _groundDico[_currentCharSelected];
    }

    public char GetCharSelectedButton()
    {
        return _currentCharSelected;
    }
}