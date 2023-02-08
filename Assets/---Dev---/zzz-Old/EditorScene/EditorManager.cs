using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public event Action ChangeModeEvent;

    public WaterData[] waterData;
    public bool IsEditMode = false;

    [Header("Map")] [SerializeField] private Transform _levelParent;
    [SerializeField] private GameObject[] _environment;
    [Header("Map")] [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private TextMeshProUGUI[] _sizeMapText;

    [Header("Game Mode")] [SerializeField] private string[] _gameModeString;
    [SerializeField] private TextMeshProUGUI _isEditModeText = null;

    private string[] _mapInfo;

    private GameObject[,] _mapGrid;
    private string[] _mapSave;
    private List<GameObject> _tempGridList = new List<GameObject>();
    private GameObject _lastGroundSelected;
    private GameObject _lastUIGroundSelected;
    private GameObject _lastButtonSelected;
    private bool[] _emptyWaterData;
    private char _lastGroundCharSelected;

    private const char GROUND_WHITE = '0';
    private const char GROUND_GREY = '1';
    private const char GROUND_NAV = 'N';
    private const char GROUND_HARD = 'H';
    private const char GROUND_CANT_BE_MOVE = 'C';
    private const char WATER_FLOWING = 'W';
    private const char WATER_SOURCE = 'S';

    private const char DIR_NS = 'I';
    private const char DIR_WE = '-';

    private const char DIR_NW = '/';
    private const char DIR_NE = 'L';
    private const char DIR_SW = '>'; //ꓶ
    private const char DIR_SE = 'Γ';

    private const char DIR_NSW = 'b';
    private const char DIR_NSE = 'd';
    private const char DIR_NWE = '⊥';
    private const char DIR_SWE = 'T';

    private const char DIR_NSWE = '+';
    //I-L⅃<Γbd⊥T+


    public static EditorManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _mapGrid = new GameObject[40, 40];
        _mapSave = new string[40];

        string map = Application.dataPath + "/Map-Init/Level0.txt";
        _mapInfo = File.ReadAllLines(map);

        for (int i = 0; i < _mapInfo.Length; i++)
        {
            _mapSave[i] = _mapInfo[i];
        }
        // Get its size
        _mapSize.x = _mapInfo[0].Length;
        _mapSize.y = _mapInfo.Length;

        InitializeLevel(_mapSave);
    }

    private void InitializeLevel(string[] mapInfo) //Map creation
    {
        for (int y = 0; y < _mapSize.y; y++)
        {
            for (int x = 0; x < _mapSize.x; x++)
            {
                // Get the string of the actual line
                string line = mapInfo[y];
                // Get the actual char of the string of the actual line
                if (string.IsNullOrEmpty(line))
                    return;
                char whichEnvironment = line[x];

                switch (whichEnvironment)
                {
                    //Init GROUND
                    case GROUND_WHITE:
                        // Instantiate the good ground into the map parent
                        GameObject go = Instantiate(_environment[0], _levelParent);
                        InitObj(go, x, y);
                        break;

                    case GROUND_GREY:
                        GameObject go1 = Instantiate(_environment[1], _levelParent);
                        InitObj(go1, x, y);
                        break;

                    case GROUND_NAV:
                        GameObject go3 = Instantiate(_environment[3], _levelParent);
                        InitObj(go3, x, y);
                        break;

                    case GROUND_HARD:
                        GameObject go2 = Instantiate(_environment[2], _levelParent);
                        InitObj(go2, x, y);
                        break;

                    case GROUND_CANT_BE_MOVE:
                        GameObject go4 = Instantiate(_environment[4], _levelParent);
                        InitObj(go4, x, y);
                        break;

                    //Init WATER
                    case WATER_SOURCE:
                        GameObject go5 = Instantiate(_environment[6], _levelParent);
                        InitObj(go5, x, y);
                        break;

                    case DIR_NS:
                        GameObject ns = Instantiate(_environment[5], _levelParent);
                        InitObj(ns, x, y);
                        InitWater(ns, DIR_NS);
                        break;

                    case DIR_WE:
                        GameObject we = Instantiate(_environment[5], _levelParent);
                        InitObj(we, x, y);
                        InitWater(we, DIR_WE);
                        break;

                    case DIR_NW:
                        GameObject nw = Instantiate(_environment[5], _levelParent);
                        InitObj(nw, x, y);
                        InitWater(nw, DIR_NW);
                        break;

                    case DIR_NE:
                        GameObject ne = Instantiate(_environment[5], _levelParent);
                        InitObj(ne, x, y);
                        InitWater(ne, DIR_NE);
                        break;

                    case DIR_SW:
                        GameObject sw = Instantiate(_environment[5], _levelParent);
                        InitObj(sw, x, y);
                        InitWater(sw, DIR_SW);
                        break;

                    case DIR_SE:
                        GameObject se = Instantiate(_environment[5], _levelParent);
                        InitObj(se, x, y);
                        InitWater(se, DIR_SE);
                        break;

                    case DIR_NSW:
                        GameObject nsw = Instantiate(_environment[5], _levelParent);
                        InitObj(nsw, x, y);
                        InitWater(nsw, DIR_NSW);
                        break;

                    case DIR_NSE:
                        GameObject nse = Instantiate(_environment[5], _levelParent);
                        InitObj(nse, x, y);
                        InitWater(nse, DIR_NSE);
                        break;

                    case DIR_NWE:
                        GameObject nwe = Instantiate(_environment[5], _levelParent);
                        InitObj(nwe, x, y);
                        InitWater(nwe, DIR_NWE);
                        break;

                    case DIR_SWE:
                        GameObject swe = Instantiate(_environment[5], _levelParent);
                        InitObj(swe, x, y);
                        InitWater(swe, DIR_SWE);
                        break;

                    case DIR_NSWE:
                        GameObject nswe = Instantiate(_environment[5], _levelParent);
                        InitObj(nswe, x, y);
                        InitWater(nswe, DIR_NSWE);
                        break;
                }
            }
        }

        StartCoroutine(InitMode());
        UpdateGameModeText();
        UpdateSizeMapText();
    }
    // private void InitializeLevel(Vector2Int sizeMap) //Map creation
    // {
    //     for (int i = 0; i < sizeMap.x; i++)
    //     {
    //         for (int j = 0; j < sizeMap.y; j++)
    //         {
    //             GameObject go = Instantiate(_environment[0], _levelParent.transform);
    //             InitObj(go, i, j);
    //         }
    //     }
    //
    //     StartCoroutine(InitMode());
    //     UpdateGameModeText();
    //     UpdateSizeMapText();
    // }

    IEnumerator InitMode()
    {
        // Waiting to find a better system because it didn't work if it's launched just after the InitializeLevel
        yield return new WaitForSeconds(.001f);
        ChangeModeEvent?.Invoke();
    }

    private void InitObj(GameObject which, int x, int y)
    {
        // Tp ground to its position
        which.transform.position = new Vector3(-x, 0, y);
        _tempGridList.Add(which);
        which.GetComponent<GroundEditorManager>().ChangeCoords(new Vector2Int(x, y));
        // Update _mapGrid
        _mapGrid[x, y] = which;
    }

    private void InitWater(GameObject which, char letter)
    {
        for (int i = 0; i < waterData.Length; i++)
        {
            if (waterData[i].WaterName == letter)
            {
                //Get the good char et give it to the waterData
                which.GetComponent<WaterEditorManager>().ChangeWaterDir(waterData[i].DirectionsNSWE);
                break;
            }
        }
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
        foreach (var temp in _tempGridList)
        {
            Destroy(temp);
        }

        _tempGridList.Clear();
        UpdateSizeMapText();

        InitializeLevel(_mapSave);
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
        if (which == 1)
        {
            for (int i = 0; i < _mapSize.y; i++)
            {
                _mapSave[i] = _mapSave[i].Insert(_mapSize.x - 1, GROUND_WHITE.ToString());
                // print("apres ++ " + _mapSave[i]);
            }
        }
        else
        {
            for (int i = 0; i < _mapSize.y; i++)
            {
                _mapSave[i] = _mapSave[i].Substring(0, _mapSize.x);
                // print("apres sub " + _mapSave[i]);
            }
        }
        
        UpdateSizeMap();
    }

    public void ChangeHeight(int which)
    {
        if (_mapSize.y - Mathf.Abs(which) <= 0 && which < 0)
            return;
        _mapSize.y += which;

        if (which == 1)
        {
            string newLine = String.Empty;
            for (int i = 0; i < _mapSize.x; i++)
            {
                newLine = newLine.Insert(0, GROUND_WHITE.ToString());
            }

            _mapSave[_mapSize.y-1] = newLine;
            // print("newLine : " + _mapSave[_mapSize.y] + " a la ligne " + (_mapSize.y -1));
        }
        else
        {
            _mapSave[_mapSize.y] = String.Empty;
            // print("destroyLine : " + _mapSave[_mapSize.y]);
        }
        
        UpdateSizeMap();
    }

    public void ChooseUIGround(GameObject which)
    {
        _lastUIGroundSelected = which;
        ChangeGround();
    }

    public void ChangeActivatedButton(GameObject button)
    {
        if (_lastButtonSelected != null)
            _lastButtonSelected.GetComponent<GroundUIButton>().NeedActivateSelectedIcon(false);

        _lastButtonSelected = button;

        _lastButtonSelected.GetComponent<GroundUIButton>().NeedActivateSelectedIcon(true);
    }

    public void ChangeLastCharSelected(string which)
    {
        if (which == null)
            which = GROUND_WHITE.ToString();
        _lastGroundCharSelected = which[0];
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

        print("I get a UI swap");

        if (_lastUIGroundSelected.GetComponent<GroundWhite>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<GroundGrey>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<GroundRed>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<GroundHard>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<GroundNav>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<WaterSourceManager>())
            _lastGroundSelected.GetComponent<GroundEditorManager>()
                .EditorTransformTo(_lastUIGroundSelected, _emptyWaterData);
        else if (_lastUIGroundSelected.GetComponent<WaterFlowing>())
            _lastGroundSelected.GetComponent<GroundEditorManager>().EditorTransformTo(_lastUIGroundSelected,
                _lastButtonSelected.GetComponent<EditorUIWaterButtonDir>().WhichDir);


        _lastGroundSelected.GetComponent<GroundMainManager>().ResetMat();
        _lastGroundSelected.GetComponent<GroundMainManager>().IsSelected = false;
        _lastGroundSelected = null;
    }

    public void UpdateGridSwap(GameObject which, Vector2Int coords, char symbol)
    {
        _mapGrid[coords.x, coords.y] = which;
        _tempGridList.Add(which);
        
        string getLine = _mapSave[coords.y];
        char[] ch = getLine.ToCharArray();
        ch[coords.x] = _lastGroundCharSelected;
        print(_lastGroundCharSelected);
        _mapSave[coords.y] = new string (ch);
        print(_mapSave[coords.y] );
    }
}