using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class EditorSaveMap : MonoBehaviour
{
    public static EditorSaveMap Instance;

    [Header("Setup Save")] [SerializeField]
    private TMP_InputField _inputFieldMapName;

    [SerializeField] private TMP_InputField _inputFieldEnergyAtStart;
    [SerializeField] private string _folderDestination;

    [Header("FB Text")] [SerializeField] private GameObject _fBTextPrefab;
    [SerializeField] private GameObject _fBTextParent;
    [SerializeField] private Color _colorGood;
    [SerializeField] private Color _colorNotGood;

    private string _mapName;
    public MapConstructData _currentMapConstructData;
    private string _hexColorGood;
    private string _hexColorNotGood;

    private const string _saveNoName = "No map name written";
    private const string _saveSucceed = "saved";
    private const float _durationDispawnText = 5f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _hexColorGood = $"<color=#{_colorGood.ToHexString()}>";
        _hexColorNotGood = $"<color=#{_colorNotGood.ToHexString()}>";

        // _currentMapConstructData = new MapConstructData();
        //SaveJson();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            SaveJson();
    }

    private void UpdateMapName(char[,] mapGrid)
    {
        _mapName = _inputFieldMapName.text;

        if (_inputFieldMapName.text == "")
        {
            SpawnFbText($"{_hexColorNotGood}{_saveNoName}");
            return;
        }

        //CreateTextFile(mapGrid);

        _inputFieldMapName.text = "";
    }

    private void SpawnFbText(string text)
    {
        GameObject go = Instantiate(_fBTextPrefab, _fBTextParent.transform);
        var fBText = go.GetComponent<TMP_Text>();
        
        fBText.DOFade(1, 0);
        fBText.text = text;
        fBText.DOFade(0, _durationDispawnText);
        Destroy(go, _durationDispawnText);
    }

    private void CreateTextFile(char[,] mapGrid)
    {
        string textName = $"{Application.streamingAssetsPath}/{_folderDestination}/{_mapName}.txt";

        if (File.Exists(textName))
            File.Delete(textName);

        string map = ConvertMapGridToString(mapGrid);
        File.WriteAllText(textName, map);
        if (map != null)
            _currentMapConstructData.Map = map;
    }

    private void GetMap()
    {
        string map = ConvertMapGridToString(EditorMapManager.Instance.GetMapGrid());
        if (map != null)
            _currentMapConstructData.Map = map;
    }
    private string ConvertMapGridToString(char[,] mapGrid)
    {
        var str = String.Empty;

        for (int y = 0; y < mapGrid.GetLength(1); y++)
        {
            for (int x = 0; x < mapGrid.GetLength(0); x++)
            {
                str += mapGrid[x, y];
            }

            if(y == mapGrid.GetLength(1)-1)
                continue;
            
            str += "\n";
        }

        return str;
    }
    
    public void SaveMap()
    {
        UpdateMapName(EditorMapManager.Instance.GetMapGrid());
        GetMap();

        if (_mapName == "") return;
        
        SaveJson();
        
        SpawnFbText($"{_hexColorGood}{_mapName} {_saveSucceed} in {_folderDestination} folder!");
        RefreshEditorProjectWindow();
    }

    private void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void AddCoordsEnergy(Vector2Int coords)
    {
        if(!_currentMapConstructData.Coords.Contains(coords))
            _currentMapConstructData.Coords.Add(coords);
    }

    public void DestroyCoordsEnergy(Vector2Int coords)
    {
        if(_currentMapConstructData.Coords.Contains(coords))
            _currentMapConstructData.Coords.Remove(coords);
    }

    private void SaveJson()
    {
        MapConstructData mapConstructData = new MapConstructData();
        mapConstructData = _currentMapConstructData;

        string json = JsonUtility.ToJson(mapConstructData);
        File.WriteAllText($"{Application.streamingAssetsPath}/{_folderDestination}/{_mapName}.txt", json);
        RefreshEditorProjectWindow();
    }
    
    // private void UpdateEnergyAtStart()
    // {
    //     if (_inputFieldMapName.text == "")
    //     {
    //         _currentMapConstructData.EnergyAtStart = 0;
    //         SpawnFbText($"{_hexColorNotGood}No Energy At Start set");
    //         return;
    //     }
    //     
    //     _currentMapConstructData.EnergyAtStart = int.Parse(_inputFieldEnergyAtStart.text);
    // }
    //
    // public void UpdateHasInventory(Toggle toggle)
    // {
    //     _currentMapConstructData.HasInventory = toggle.isOn;
    // }
    // public void UpdateHasTrashCan(Toggle toggle)
    // {
    //     _currentMapConstructData.HasTrashCan = toggle.isOn;
    // }
    // public void UpdateHasBlockLastGroundsSwapped(Toggle toggle)
    // {
    //     _currentMapConstructData.BlockLastGroundsSwapped = toggle.isOn;
    // }
}