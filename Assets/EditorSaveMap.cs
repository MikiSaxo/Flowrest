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
    private LvlData _currentLvlData;
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

        _currentLvlData = new LvlData();
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

        CreateTextFile(mapGrid);

        _inputFieldMapName.text = "";
    }

    private void UpdateEnergyAtStart()
    {
        if (_inputFieldMapName.text == "")
        {
            _currentLvlData.EnergyAtStart = 0;
            SpawnFbText($"{_hexColorNotGood}No Energy At Start set");
            return;
        }
        
        _currentLvlData.EnergyAtStart = int.Parse(_inputFieldEnergyAtStart.text);
    }

    public void UpdateHasInventory(Toggle toggle)
    {
        _currentLvlData.HasInventory = toggle.isOn;
    }
    public void UpdateHasTrashCan(Toggle toggle)
    {
        _currentLvlData.HasTrashCan = toggle.isOn;
    }
    public void UpdateHasBlockLastGroundsSwapped(Toggle toggle)
    {
        _currentLvlData.BlockLastGroundsSwapped = toggle.isOn;
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
            _currentLvlData.Map = map;
    }

    public void SaveMap()
    {
        UpdateMapName(EditorMapManager.Instance.GetMapGrid());
        UpdateEnergyAtStart();

        if (_mapName == "") return;
        
        SaveJson();
        
        SpawnFbText($"{_hexColorGood}{_mapName} {_saveSucceed} in {_folderDestination} folder!");
        RefreshEditorProjectWindow();
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

            str += "\n";
        }

        return str;
    }

    private void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void UpdateCoordsEnergy(Vector2Int coords)
    {
        _currentLvlData.Coords.Add(coords);
    }

    private void SaveJson()
    {
        // var mapgrid = EditorMapManager.Instance.GetMapGrid(); 
        // string map = ConvertMapGridToString(mapgrid);
        // _currentLvlData.Map = map;


        LvlData lvlData = new LvlData();
        lvlData = _currentLvlData;

        string json = JsonUtility.ToJson(lvlData);
        File.WriteAllText($"{Application.streamingAssetsPath}/{_folderDestination}/{_mapName}-Json.txt", json);
        RefreshEditorProjectWindow();
    }
}