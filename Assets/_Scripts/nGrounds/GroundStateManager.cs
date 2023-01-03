using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundStateManager : MonoBehaviour
{
    public int IdOfBloc { get; set; }
    public bool IsTreated { get; set; }
    public bool IsBiome { get; set; }
    

    private GroundBaseState currentState;
    private GroundPlainState plainState = new GroundPlainState();
    private GroundDesertState desertState = new GroundDesertState();
    private GroundWaterState waterState = new GroundWaterState();

    [Header("Setup")] [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject[] _meshes;

    [Tooltip("This is the minimum number to have a biome after verified a square of 3x3")] [SerializeField]
    private int _minNbAroundBiome;

    [Header("Characteristics")] public float Temperature;
    [Range(0, 100)] public float Humidity;

    private readonly List<GroundBaseState> _allState = new List<GroundBaseState>();
    private GameObject _meshCurrent;
    private Vector2Int _coords;
    private float _temperatureAround;
    private float _humidityAround;
    private float _countBlocAround;
    private float _countSameBlocAround;
    private float _countIfEnoughBloc;

    private List<GameObject> _groundInBiome = new List<GameObject>();

    private readonly Vector2Int[] _directions = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

    private void Awake()
    {
        _allState.Add(plainState);
        _allState.Add(desertState);
        _allState.Add(waterState);
    }

    private void Start()
    {
        n_MapManager.Instance.UpdateGround += GetValuesAround;
        n_MapManager.Instance.CheckBiome += LaunchCheckForBiome;

        // ResetBiome();
    }

    public void InitState(int stateNb)
    {
        _allState[stateNb].InitState(this);
        ChangeState(stateNb);
    }

    private void ChangeState(int whichState)
    {
        currentState = _allState[whichState];
        currentState.EnterState(this);

        LaunchCheckForBiome();
    }

    private void LaunchCheckForBiome()
    {
        StartCoroutine(WaitToCheckForBiome());
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

    public void ChangeValues(float humidity, float temperature)
    {
        Humidity = humidity;
        Temperature = temperature;
    }

    public void ChangeCoords(Vector2Int coords)
    {
        _coords = coords;
    }

    private void GetValuesAround() // Get the average temperature and humidity from his 8 neighbors
    {
        _temperatureAround = 0;
        _humidityAround = 0;
        _countBlocAround = 0;

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int newPos = new Vector2Int(_coords.x + i, _coords.y + j);
                if (i == 0 && j == 0) continue;
                // Check if inside of array
                if (newPos.x < 0 || newPos.x >= n_MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                    newPos.y >= n_MapManager.Instance.MapGrid.GetLength(1)) continue;
                // Check if something exist
                if (n_MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
                // Check if has GroundManager
                if (!n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
                // It's good
                _temperatureAround += n_MapManager.Instance.MapGrid[newPos.x, newPos.y]
                    .GetComponent<GroundStateManager>().Temperature;
                _humidityAround += n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()
                    .Humidity;
                _countBlocAround++;
            }
        }

        StartCoroutine(WaitToChange());
    }

    private IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(.01f);
        var newHumidity = (_humidityAround / _countBlocAround + Humidity) / 2;
        var newTemperature = (_temperatureAround / _countBlocAround + Temperature) / 2;
        //print("old humi : " + Humidity + " / old tempe : " + Temperature + " ----- " + "humi : " + newHumidity + " / tempe : " + newTemperature);
        ChangeValues(newHumidity, newTemperature);
        CheckIfNeedUpdate();
    }

    private void CheckIfNeedUpdate() // "System" to transform bloc's state according to its temperature and humidity
    {
        switch (Temperature)
        {
            case >= 0 and < 30 when Humidity is < 80 and > 10:
                ChangeState(0);
                break;
            case >= 30 when Humidity <= 10:
                ChangeState(1);
                break;
            case >= 0 when Humidity >= 80:
                ChangeState(2);
                break;
        }
    }

    public void OnSelected() // When bloc is Selected by the player
    {
        n_MapManager.Instance.CheckIfGroundSelected(gameObject, _coords);
    }

    public void ResetMatIndicator() // Bridge to the indicator and Map_Manager
    {
        _indicator.GetComponent<GroundIndicator>().ResetMat();
        
        StartCoroutine(WaitToCheckForBiome());
    }

    public GroundBaseState GetActualState()
    {
        return currentState;
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
        foreach (var dir in _directions)
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

    public GameObject GetMeshParent()
    {
        return _meshParent;
    }

    private void OnDisable()
    {
        n_MapManager.Instance.UpdateGround -= GetValuesAround;
        n_MapManager.Instance.CheckBiome -= LaunchCheckForBiome;
    }
}