using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum AllStates
{
    None = -1,
    Plain = 0,
    Desert = 1,
    Water = 2,
    Tropical = 3,
    Savanna = 4,
    Geyser = 5,
    Snow = 6,
    PolarDesert = 7,
    Tundra = 8,
    Swamp = 9,
    Mountain = 10
}

public class GroundStateManager : MonoBehaviour
{
    public int IdOfBloc { get; set; }
    public bool IsTreated { get; set; }
    public bool IsBiome { get; set; }
    public bool IsProtected { get; set; }
    public bool IsProtectedPrevisu { get; set; }

    private AllStates _statesEnum;
    private AllStates _statePrevisu;

    private GroundBaseState currentState;
    private GroundPlainState _plainState = new GroundPlainState();
    private GroundDesertState _desertState = new GroundDesertState();
    private GroundWaterState _waterState = new GroundWaterState();
    private GroundTropicalState _tropicalState = new GroundTropicalState();
    private GroundSavannaState _savannaState = new GroundSavannaState();
    private GroundGeyserState _geyserState = new GroundGeyserState();
    private GroundSnowState _snowState = new GroundSnowState();
    private GroundPolarDesertState _polarDesertState = new GroundPolarDesertState();
    private GroundTundraState _tundraState = new GroundTundraState();
    private GroundSwampState _swampState = new GroundSwampState();
    private GroundMountainState _mountainState = new GroundMountainState();

    [Header("Setup")] [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject[] _meshes;
    [SerializeField] private GroundPrevisu _fB_Previsu;

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

    private List<GroundStateManager> _stockPrevisu = new List<GroundStateManager>();

    private void Awake()
    {
        _allState.Add(_plainState);
        _allState.Add(_desertState);
        _allState.Add(_waterState);
        _allState.Add(_tropicalState);
        _allState.Add(_savannaState);
        _allState.Add(_geyserState);
        _allState.Add(_snowState);
        _allState.Add(_polarDesertState);
        _allState.Add(_tundraState);
        _allState.Add(_swampState);
        _allState.Add(_mountainState);
    }

    private void Start()
    {
        MapManager.Instance.CheckBiome += LaunchCheckForBiome;
        MapManager.Instance.ResetSelection += ResetIndicator;
    }

    public void InitState(AllStates state)
    {
        if (state == AllStates.None) return;
        
        _allState[(int)state].InitState(this);
        ChangeState(state);
    }

    public void ChangeState(AllStates state)
    {
        if (IsProtected) return;
        
        _statesEnum = state;
        currentState = _allState[(int)state];
        // print(currentState);
        currentState.EnterState(this);
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
            if (newPos.x < 0 || newPos.x >= MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= MapManager.Instance.MapGrid.GetLength(1)) continue;
            // Check if something exist
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
            // Check if not a Mountain
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.Mountain) continue; 

            var grnd = MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            // grnd.currentState.CheckUpdate(grnd, currentState);
            var newState = ConditionManager.Instance.GetState(_statesEnum, grnd.GetCurrentStateEnum());
            grnd.ChangeState(newState);
        }
    }

    public void OnSelected() // When bloc is Selected by the player
    {
        MapManager.Instance.CheckIfGroundSelected(gameObject, _coords);
    }

    public GroundBaseState GetCurrentState()
    {
        return currentState;
    }

    public AllStates GetCurrentStateEnum()
    {
        return _statesEnum;
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
        return;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int newPos = new Vector2Int(_coords.x + i, _coords.y + j);
                // No need to count the actual
                if (i == 0 && j == 0) continue;
                // Check if inside of array
                if (newPos.x < 0 || newPos.x >= MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                    newPos.y >= MapManager.Instance.MapGrid.GetLength(1)) continue;
                // Check if something exist
                if (MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
                // Check if has GroundManager
                if (!MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
                // Check if same state
                if (MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IdOfBloc !=
                    IdOfBloc) continue;
                // It's good
                _countSameBlocAround++;
            }
        }
        // print("_countSameBlocAround " + _countSameBlocAround);

        if (_countSameBlocAround > 7)
            CheckAllSameBlocConnected(MapManager.Instance.MapGrid, _coords);
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
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IdOfBloc !=
                IdOfBloc) continue;
            // Check if has been already treated
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IsTreated) continue;
            var canContinue = true;
            foreach (var ground in _groundInBiome)
            {
                if (ground != MapManager.Instance.MapGrid[newPos.x, newPos.y]) continue;
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

    // Called when entered second indicator and it's around the one selected
    public void LookingNewPrevisu()
    {
        var getState = MapManager.Instance.GetLastStateSelected();
        if (getState == AllStates.None) return;

        GetNewPrevisu(getState);
    }

    public void GetNewPrevisu(AllStates state)
    {
        int resultStateNumber = (int)ConditionManager.Instance.GetState(state, GetCurrentStateEnum());

        if (_fB_Previsu.IsIconActivated())
        {
            int currentPrevisuState = _fB_Previsu.GetIndexActualIcon();
            resultStateNumber = (int)ConditionManager.Instance.GetState((AllStates)currentPrevisuState, state);
            // print("old = " + currentPrevisuState + " new = " + resultState);
        }
        
        if (resultStateNumber == (int)GetCurrentStateEnum())
            return;

        if(!IsProtectedPrevisu)
            ActivatePrevisu(resultStateNumber);
    }

    // Called when this bloc is selected
    public void SelectedLaunchAroundPrevisu(AllStates state)
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= MapManager.Instance.MapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= MapManager.Instance.MapGrid.GetLength(1)) continue;
            // Check if something exist
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>())
                continue;
            // Check if not a Mountain
            if (MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.Mountain) continue; 

            var ground = MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            ground.GetNewPrevisu(state);

            _stockPrevisu.Add(ground);
        }

        ActivatePrevisu((int)state);
        _stockPrevisu.Add(this);
    }

    public void ActivatePrevisu(int resultStateNumber)
    {
        if (resultStateNumber == (int)GetCurrentStateEnum())
            return;
        // print("result = " + resultStateNumber);
        _fB_Previsu.ActivateIcon(resultStateNumber);
    }

    public void DeactivatePrevisu()
    {
        _fB_Previsu.DeactivateIcon();
    }

    public void ResetAroundSelectedPrevisu()
    {
        foreach (var previsu in _stockPrevisu)
        {
            previsu.DeactivatePrevisu();
        }

        _stockPrevisu.Clear();
    }

    public Sprite GetGroundPrevisu(int index)
    {
        return _fB_Previsu.GetIconTile(index);
    }

    private void OnDisable()
    {
        MapManager.Instance.CheckBiome -= LaunchCheckForBiome;
        MapManager.Instance.ResetSelection -= ResetIndicator;
    }
}