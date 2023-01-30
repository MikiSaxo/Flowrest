using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AllStates
{
    None = -1,
    Plain = 0,
    Desert = 1,
    Water = 2,
    Tropical = 3,
    Savane = 4,
    HotSpring = 5
}

public class GroundStateManager : MonoBehaviour
{
    public int IdOfBloc { get; set; }
    public bool IsTreated { get; set; }
    public bool IsBiome { get; set; }

    private AllStates _allStatesEnum;

    private GroundBaseState currentState;
    private GroundPlainState _plainState = new GroundPlainState();
    private GroundDesertState _desertState = new GroundDesertState();
    private GroundWaterState _waterState = new GroundWaterState();
    private GroundTropicalState _tropicalState = new GroundTropicalState();
    private GroundSavaneState _savaneState = new GroundSavaneState();
    private GroundHotSpringState _hotSpringState = new GroundHotSpringState();

    [Header("Setup")] [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject[] _meshes;

    [Tooltip("This is the minimum number to have a biome after verified a square of 3x3")] [SerializeField]
    private int _minNbAroundBiome;

    //[Header("Characteristics")] public float Temperature;
    //[Range(0, 100)] public float Humidity;

    private readonly List<GroundBaseState> _allState = new List<GroundBaseState>();
    private GameObject _meshCurrent;
    [SerializeField] private Vector2Int _coords;
    private float _temperatureAround;
    private float _humidityAround;
    private float _countBlocAround;
    private float _countSameBlocAround;
    private float _countIfEnoughBloc;
    private bool _isUpdating;

    private List<GameObject> _groundInBiome = new List<GameObject>();

    private readonly Vector2Int[] _crossDirections = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

    private readonly Vector2Int[] _hexOddDirections = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, 1), new(1, 1) };

    private readonly Vector2Int[] _hexPeerDirections = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(1, -1), new(-1, -1) };

    private void Awake()
    {
        _allState.Add(_plainState);
        _allState.Add(_desertState);
        _allState.Add(_waterState);
        _allState.Add(_tropicalState);
        _allState.Add(_savaneState);
        _allState.Add(_hotSpringState);
    }

    private void Start()
    {
        n_MapManager.Instance.UpdateGround += GetValuesAround;
        n_MapManager.Instance.CheckBiome += LaunchCheckForBiome;
        n_MapManager.Instance.ResetSelection += ResetIndicator;

        // ResetBiome();
    }

    public void InitState(AllStates state)
    {
        _allState[(int)state].InitState(this);
        ChangeState(state);
    }

    public void ChangeState(AllStates state)
    {
        currentState = _allState[(int)state];
        currentState.EnterState(this);

        //LaunchCheckForBiome();
    }

    private void LaunchCheckForBiome()
    {
        //StartCoroutine(WaitToCheckForBiome());
    }

    IEnumerator WaitToCheckForBiome()
    {
        yield return new WaitForSeconds(.01f);
        ResetBiome();
    }

    public void ChangeMesh(int meshNb)
    {
        Destroy(_meshCurrent);
        GameObject go = Instantiate(_meshes[meshNb], _meshParent.transform);
        _meshCurrent = go;
    }

    // public void ChangeValues(float humidity, float temperature)
    // {
    //     Humidity = humidity;
    //     Temperature = temperature;
    // }

    public void ChangeCoords(Vector2Int coords)
    {
        _coords = coords;
    }

    public void UpdateGroundsAround()
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);

            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= n_MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= n_MapManager.Instance.MapGrid.GetLength(1)) continue;
            // Check if something exist
            if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;

            var grnd = n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            grnd.currentState.CheckUpdate(grnd, currentState);
            //n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().GetValuesAround();
        }
    }

    public void GetValuesAround() // Get the average temperature and humidity from his 6* neighbors
    {
        if (_isUpdating) return;
        _isUpdating = true;

        // _temperatureAround = 0;
        // _humidityAround = 0;
        // _countBlocAround = 0;

        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= n_MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= n_MapManager.Instance.MapGrid.GetLength(1)) continue;
            // Check if something exist
            if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
            // It's good
            
            
            
            //_temperatureAround += n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().Temperature;
            //_humidityAround += n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().Humidity;
            //_countBlocAround++;
        }

        //StartCoroutine(WaitToChange());
    }

    private IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(.01f);
        //var newHumidity = (_humidityAround / _countBlocAround + Humidity) / 2;
        //var newTemperature = (_temperatureAround / _countBlocAround + Temperature) / 2;
        //print("old humi : " + Humidity + " / old tempe : " + Temperature + " ----- " + "humi : " + newHumidity + " / tempe : " + newTemperature);
        //ChangeValues(newHumidity, newTemperature);
        CheckIfNeedUpdate();
        _isUpdating = false;
    }

    private void CheckIfNeedUpdate() // "System" to transform bloc's state according to its temperature and humidity
    {
        // switch (Temperature)
        // {
        //     // Plain
        //     case >= 1 and < 47 when Humidity is < 50 and > 11:
        //         ChangeState(0);
        //         break;
        //     // Desert
        //     case >= 33 when Humidity <= 10:
        //         ChangeState(1);
        //         break;
        //     // Water
        //     case >= 1 and < 47 when Humidity >= 26:
        //         ChangeState(2);
        //         break;
        // }
        
    }

    public void OnSelected() // When bloc is Selected by the player
    {
        n_MapManager.Instance.CheckIfGroundSelected(gameObject, _coords);
    }

    public GroundBaseState GetActualState()
    {
        return currentState;
    }

    public void ResetIndicator() // Bridge to the indicator and Map_Manager
    {
        _indicator.GetComponent<GroundIndicator>().ResetIndicator();

        StartCoroutine(WaitToCheckForBiome());
    }

    private void ResetBiome()
    {
        // print("reseet biome");
        IsBiome = false;

        foreach (var getScript in _groundInBiome.Select(ground => ground.GetComponent<GroundStateManager>()))
        {
            getScript.GetMeshParent().GetComponentInChildren<MeshBiomeManager>().TransformTo(false);

            getScript.GetComponent<GroundStateManager>().IsBiome = false;
            getScript.GetComponent<GroundStateManager>().IsTreated = false;
        }

        _groundInBiome.Clear();
        _countSameBlocAround = 0;
        _countIfEnoughBloc = 0;

        FirstCheckIfBiome();
    }

    private void FirstCheckIfBiome()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int newPos = new Vector2Int(_coords.x + i, _coords.y + j);
                // No need to count the actual
                if (i == 0 && j == 0) continue;
                // Check if inside of array
                if (newPos.x < 0 || newPos.x >= n_MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                    newPos.y >= n_MapManager.Instance.MapGrid.GetLength(1)) continue;
                // Check if something exist
                if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
                // Check if has GroundManager
                if (!n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
                // Check if same state
                if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IdOfBloc !=
                    IdOfBloc) continue;
                // It's good
                _countSameBlocAround++;
            }
        }
        // print("_countSameBlocAround " + _countSameBlocAround);

        if (_countSameBlocAround > 7)
            CheckAllSameBlocConnected(n_MapManager.Instance.MapGrid, _coords);
    }

    private void CheckAllSameBlocConnected(GameObject[,] mapGrid, Vector2Int coords)
    {
        foreach (var dir in _crossDirections)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1)) continue;
            // Check if not null
            if (mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundStateManager
            if (!mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
            // Check if same state
            if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IdOfBloc !=
                IdOfBloc) continue;
            // Check if has been already treated
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IsTreated) continue;
            var canContinue = true;
            foreach (var ground in _groundInBiome)
            {
                if (ground != n_MapManager.Instance.MapGrid[newPos.x, newPos.y]) continue;
                canContinue = false;
                break;
            }

            if (!canContinue)
                continue;

            // It's good 
            _countIfEnoughBloc++;

            mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IsTreated = true;
            // Add it to the list to reboot it for a future test
            _groundInBiome.Add(mapGrid[newPos.x, newPos.y]);
            // Restart the recursive
            CheckAllSameBlocConnected(mapGrid, newPos);
        }
        // print("_countIfEnoughBloc " + _countIfEnoughBloc);

        if (_countIfEnoughBloc > _countSameBlocAround + _minNbAroundBiome)
            StartCoroutine(TransformToBiome());
        // print(_coords + " / " + _countIfEnoughBloc);
    }

    private IEnumerator TransformToBiome()
    {
        if (IsBiome) yield break;

        yield return new WaitForSeconds(.01f);
        // print("salam les khyoa : " + _groundInBiome.Count);
        IsBiome = true;
        foreach (var getScript in _groundInBiome.Select(ground => ground.GetComponent<GroundStateManager>()))
        {
            getScript.GetMeshParent().GetComponentInChildren<MeshBiomeManager>().TransformTo(true);

            getScript.GetComponent<GroundStateManager>().IsTreated = false;
            getScript.GetComponent<GroundStateManager>().IsBiome = true;
        }
    }

    public void ForceEnteredIndicator()
    {
        _indicator.GetComponent<GroundIndicator>().ForceEntered();
    }

    // public void ChangeTemperature(int howMany)
    // {
    //     Temperature += howMany;
    // }

    public GameObject GetMeshParent()
    {
        return _meshParent;
    }

    public Vector2Int GetCoords()
    {
        return _coords;
    }

    public void EnabledWaterCubes(bool which)
    {
        gameObject.GetComponentInChildren<WaterMesh>().IsEnabled(which);
    }

    private void OnDisable()
    {
        n_MapManager.Instance.UpdateGround -= GetValuesAround;
        n_MapManager.Instance.CheckBiome -= LaunchCheckForBiome;
        n_MapManager.Instance.ResetSelection -= ResetIndicator;
    }
}