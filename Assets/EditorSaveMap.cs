using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;

public class EditorSaveMap : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private TMP_InputField _inputFieldMapName;
    [SerializeField] private string _folderDestination;
    
    [Header("FB Text")]
    [SerializeField] private TMP_Text _fBText;
    [SerializeField] private Color _colorSave;
    [SerializeField] private Color _colorCantSave;

    private const string _saveNoName = "No map name written"; 
    private const string _saveSucceed = "Map saved!";
    private const float _durationDispawnText = 5f;
    private string _mapName;


    private void Start()
    {
        _fBText.text = String.Empty;
    }

    public void UpdateMapName(char[,] mapGrid)
    {
        _mapName = _inputFieldMapName.text;

        if (_inputFieldMapName.text == "")
        {
            UpdateFbText($"<color=#{_colorCantSave.ToHexString()}>{_saveNoName}");
            return;
        }
        
        CreateTextFile(mapGrid);

        _inputFieldMapName.text = "";
    }

    private void UpdateFbText(string text)
    {
        _fBText.DOFade(1,0);
        _fBText.text = text;
        _fBText.DOFade(0,_durationDispawnText);
    }

    private void CreateTextFile(char[,] mapGrid)
    {
        string textName = Application.streamingAssetsPath + $"/{_folderDestination}/" + _mapName + ".txt";

        if (File.Exists(textName))
            File.Delete(textName);

        string map = ConvertMapGridToString(mapGrid);
        File.WriteAllText(textName, map);

        UpdateFbText($"<color=#{_colorSave.ToHexString()}>{_saveSucceed} in {_folderDestination} folder");

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
    
    void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
