using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public event Action ChangeModeEvent;

    public bool IsEditMode = false;

    [Header("Map")] [SerializeField] private Transform _levelParent;
    [SerializeField] private GameObject[] _environment;
    [Header("Map")] [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private TextMeshProUGUI[] _sizeMapText;

    [Header("Game Mode")] [SerializeField] private string[] _gameModeString;
    [SerializeField] private TextMeshProUGUI _isEditModeText = null;

    private GameObject[,] _mapGrid;
    private List<GameObject> _tempGrid = new List<GameObject>();
    private GameObject _lastGroundSelected;
    private GameObject _lastUIGroundSelected;
    private GameObject _lastButtonSelected;
    private bool[] _emptyWaterData;

    public static EditorManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _mapGrid = new GameObject[40, 40];
        InitializeLevel(_mapSize);
    }

    private void InitializeLevel(Vector2Int sizeMap) //Map creation
    {
        for (int i = 0; i < sizeMap.x; i++)
        {
            for (int j = 0; j < sizeMap.y; j++)
            {
                GameObject go = Instantiate(_environment[0], _levelParent.transform);
                InitObj(go, i, j);
            }
        }

        StartCoroutine(InitMode());
        UpdateGameModeText();
        UpdateSizeMapText();
    }

    IEnumerator InitMode()
    {
        // Waiting to find a better system because it didn't work if it's launched just after the InitializeLevel
        yield return new WaitForSeconds(.001f);
        ChangeModeEvent?.Invoke();
    }

    private void InitObj(GameObject which, int i, int j)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(-j, 0, i);
        _tempGrid.Add(which);
        // Update _mapGrid
        _mapGrid[j, _mapSize.x - 1 - i] = which;
    }

    public void ChangeMode() // Called By EditMode Button
    {
        // Change the mode
        IsEditMode = !IsEditMode;
        // Change the EditMode button text
        UpdateGameModeText();
        // Call the event to activate or not the indicators
        ChangeModeEvent?.Invoke();
    }

    private void UpdateGameModeText()
    {
        _isEditModeText.text = IsEditMode ? _gameModeString[0] : _gameModeString[1];
    }

    private const string SizeMapText = "Size : ";

    private void UpdateSizeMap()
    {
        foreach (var temp in _tempGrid)
        {
            Destroy(temp);
        }

        _tempGrid.Clear();
        UpdateSizeMapText();
        InitializeLevel(_mapSize);
    }

    private void UpdateSizeMapText()
    {
        _sizeMapText[0].text = SizeMapText + _mapSize.x;
        _sizeMapText[1].text = SizeMapText + _mapSize.y;
    }

    public void ChangeWidth(int which)
    {
        if (_mapSize.x - Mathf.Abs(which) <= 0 && which < 0)
            return;
        _mapSize.x += which;
        UpdateSizeMap();
    }

    public void ChangeHeight(int which)
    {
        if (_mapSize.y - Mathf.Abs(which) <= 0 && which < 0)
            return;
        _mapSize.y += which;
        UpdateSizeMap();
    }

    public void ChooseUIGround(GameObject which)
    {
        _lastUIGroundSelected = which;
        ChangeGround();
    }

    public void ChangeActivatedButton(GameObject button)
    {
        if(_lastButtonSelected != null)
            _lastButtonSelected.GetComponent<GroundUIButton>().NeedActivateSelectedIcon(false);

        _lastButtonSelected = button;
        
        _lastButtonSelected.GetComponent<GroundUIButton>().NeedActivateSelectedIcon(true);
    }

    public void ChangeLastGroundSelected(GameObject which)
    {
        if (_lastGroundSelected != null)
            _lastGroundSelected.GetComponent<GroundMainManager>().ResetMat();
        
        _lastGroundSelected = which;
        ChangeGround();
    }

    private void ChangeGround()
    {
        if (_lastUIGroundSelected == null || _lastGroundSelected == null) return;

        print("I get a swap");
        
        if(_lastUIGroundSelected.GetComponent<GroundWhite>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if(_lastUIGroundSelected.GetComponent<GroundGrey>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if(_lastUIGroundSelected.GetComponent<GroundRed>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if(_lastUIGroundSelected.GetComponent<GroundHard>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if(_lastUIGroundSelected.GetComponent<GroundNav>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if(_lastUIGroundSelected.GetComponent<WaterSourceManager>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<WaterFlowing>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected, _lastButtonSelected.GetComponent<EditorUIWaterButtonDir>().WhichDir);
        
        
        _lastGroundSelected.GetComponent<GroundMainManager>().ResetMat();
        _lastGroundSelected.GetComponent<GroundMainManager>().IsSelected = false;
        _lastGroundSelected = null;
    }
}