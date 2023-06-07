using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public enum AllStates
{
    None = -1,
    __Grassias__ = 0,
    Desert = 1,
    __Hydros__ = 2,
    Tropical = 3,
    __Calcid__ = 4,
    Geyser = 5,
    Snow = 6,
    PolarDesert = 7,
    Tundra = 8,
    __Viscosa__ = 9,
    __Pyreneos__ = 10
}

public class GroundStateManager : MonoBehaviour
{
    public int IdOfBloc { get; set; }
    public bool IsProtected { get; set; }
    public bool IsProtectedPrevisu { get; set; }

    public bool JustBeenSwaped { get; set; }

    public bool IsTreated { get; set; }

    public bool IsPlayerForceSwapBlocked { get; set; }
    public bool IsPlayerNotForcePose { get; set; }
    public bool IsBored { get; private set; }

    public AllStates StockStatePreview { get; set; }

    public MeshManager CurrentMeshManager { get; set; }


    [Header("Setup")] [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject[] _meshes;

    [Header("Feedbacks")] [SerializeField] private GroundPrevisu _fbPrevisu;
    [SerializeField] private FB_Arrow _fbArrow;

    [Header("Anim values")] [SerializeField]
    private float _bottomBounceValue;

    [SerializeField] private float _timeBounceValue;

    [Header("Temp coords just to see them")] [SerializeField]
    private Vector2Int _coords;

    [Header("FX")] [SerializeField] private GameObject _fXDrop = null;
    [SerializeField] private float _paddingFXDrop;
    [SerializeField] private GameObject _fxTileFree;
    [SerializeField] private GameObject _fxTileBored;


    private AllStates _currentState;
    private AllStates _tempCurrentState;
    private float _startYPosMeshParent;
    private GameObject _meshCurrent;
    private readonly List<GroundBaseState> _allState = new List<GroundBaseState>();
    private int _countTileChain;
    private List<GroundStateManager> _stockTileChain = new List<GroundStateManager>();
    private List<GroundStateManager> _stockGroundPrevisu = new List<GroundStateManager>();
    private Dictionary<GroundStateManager, int> _saveGrndToUpdate = new Dictionary<GroundStateManager, int>();

    private Color _colorOtherSwap;


    #region AllState

    private GroundBaseState currentGroundBase;
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

    #endregion

    private readonly Vector2Int[] _hexOddDirections = new Vector2Int[]
        { new(0, 1), new(1, 1), new(1, 0), new(0, -1), new(-1, 0), new(-1, 1) }; //new
    // { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, 1), new(1, 1) }; //old

    private readonly Vector2Int[] _hexPeerDirections = new Vector2Int[]
        { new(0, 1), new(1, 0), new(1, -1), new(0, -1), new(-1, -1), new(-1, 0) }; //new
    // { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(1, -1), new(-1, -1) }; //old

    const float OFFSET_TIMING = .0003f;

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
        MapManager.Instance.ResetSelection += ResetIndicator;

        _startYPosMeshParent = 0;
    }

    public void InitState(AllStates state)
    {
        if (state == AllStates.None) return;

        _allState[(int)state].InitState(this);
        ChangeState(state);
    }

    private void ChangeState(AllStates state)
    {
        if (IsProtected) return;

        ForceChangeState(state);
    }

    public void ForceChangeState(AllStates state)
    {
        if (state != _currentState)
        {
            BounceAnim();
        }

        LastMoveManager.Instance.UpdateCurrentStateMap(_coords, state);

        _currentState = state;
        currentGroundBase = _allState[(int)state];
        currentGroundBase.EnterState(this);
        StockStatePreview = _currentState;
        _tempCurrentState = _currentState;

        AudioManager.Instance.PlaySFX("Pop");
    }

    public void ChangeStatePrevisu(AllStates state)
    {
        if (IsProtectedPrevisu) return;

        _fbPrevisu.ActivateIcon((int)state);
        // if (state == _currentState)
        //     _fbPrevisu.transform.DOScale(2, 0);
        // else
        //     _fbPrevisu.transform.DOScale(3, 0);
        //     _fbPrevisu.DeactivateIcon();
    }

    public void ChangeMesh(int meshNb)
    {
        Destroy(_meshCurrent);
        GameObject go = Instantiate(_meshes[meshNb], _meshParent.transform);
        _meshCurrent = go;
        CurrentMeshManager = _meshCurrent.GetComponent<MeshManager>();
    }

    public void ChangeCoords(Vector2Int coords)
    {
        _coords = coords;
    }

    public bool CheckIfFlower()
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;
        int count = 0;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            var mapGrid = MapManager.Instance.GetMapGrid();
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1)) continue;
            // Check if something exist
            if (mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundManager
            if (!mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>())
                continue;
            // Check if same as itself
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()
                    .GetCurrentStateEnum() != GetCurrentStateEnum())
                continue;

            // Good
            count++;
        }

        // print(gameObject.name + " count " + count);
        return count >= 6;
    }

    public int CountSameTileConnected(GameObject[,] mapGrid, Vector2Int coords, AllStates state)
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var dir in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);

            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1)) continue;
            // Check if not null
            if (mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has GroundStateManager
            if (!mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;
            // Check if has been already treated
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IsTreated) continue;
            // Check if same State
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().GetCurrentStateEnum() != state) continue;

            // It's good 
            _countTileChain++;
            mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IsTreated = true;
            _stockTileChain.Add(mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>());

            // Restart the recursive
            CountSameTileConnected(mapGrid, newPos, state);
        }

        return _countTileChain;
    }

    public void UpdatePrevisuArrow(bool state)
    {
        _fbArrow.UpdateArrow(state);
    }

    public void UpdateGroundsAround(AllStates otherState)
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;


        var angle = 0;
        _saveGrndToUpdate.Clear();

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            var mapGrid = MapManager.Instance.GetMapGrid();


            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1))
            {
                angle += 60;
                continue;
            }

            // Check if something exist
            if (mapGrid[newPos.x, newPos.y] == null)
            {
                angle += 60;
                continue;
            }

            // Check if has GroundManager
            if (!mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>())
            {
                angle += 60;
                continue;
            }

            // Check if not a Mountain
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()
                    .GetCurrentStateEnum() == AllStates.__Pyreneos__)
            {
                angle += 60;
                continue;
            }


            var grnd = mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            var newState = ConditionManager.Instance.GetState(otherState, grnd.GetCurrentTempStateEnum());

            grnd._tempCurrentState = newState;

            _saveGrndToUpdate.Add(grnd, angle);

            angle += 60;
        }
    }

    public void LaunchDropFX()
    {
        _colorOtherSwap = SetupUIGround.Instance.GetGroundUIData((int)GetCurrentStateEnum()).ColorIcon;
        _colorOtherSwap = Color.white;
        // _colorOtherSwap.r -= .1f;
        // _colorOtherSwap.g -= .1f;
        // _colorOtherSwap.b -= .1f;

        foreach (var grnd in _saveGrndToUpdate)
        {
            if (grnd.Key.IsProtected) continue;

            grnd.Key.LaunchCorouDropFX(grnd.Key.GetCurrentTempStateEnum(), grnd.Value, transform, _colorOtherSwap);
        }
    }

    private void LaunchCorouDropFX(AllStates newState, float angle, Transform parent, Color color)
    {
        StartCoroutine(CorouDropFX(newState, angle, parent, color));
    }

    IEnumerator CorouDropFX(AllStates newState, float angle, Transform parent, Color color)
    {
        // if (_currentState == newState) yield break;

        yield return new WaitForSeconds(OFFSET_TIMING * angle);

        GameObject go = Instantiate(_fXDrop, parent);

        var rotation = go.transform.rotation;
        go.transform.DOMoveY(go.transform.position.y + _paddingFXDrop, 0);
        go.transform.DORotate(new Vector3(-60, angle, rotation.z), 0);

        // _colorOtherSwap = SetupUIGround.Instance.GetGroundUIData((int)MapManager.Instance.GetLastStateSelected()).ColorIcon;
        _colorOtherSwap = color;
        // print(_colorOtherSwap + " : " + _coords);

        var main = go.GetComponent<ParticleSystem>().main;
        main.startColor = _colorOtherSwap;
        // print("=aller lens : " + main.startColor.color + " : " + _coords);
        // print("=aller kyky : " + _colorOtherSwap);

        yield return new WaitForSeconds(1);

        ChangeState(newState);
    }

    public void UpdateGroundsAroundPreview(AllStates otherState)
    {
        Vector2Int[] hexDirections = new Vector2Int[6];
        // Important for the offset with hex coords
        hexDirections = _coords.x % 2 == 0 ? _hexPeerDirections : _hexOddDirections;

        foreach (var hexPos in hexDirections)
        {
            Vector2Int newPos = new Vector2Int(_coords.x + hexPos.x, _coords.y + hexPos.y);
            var mapGrid = MapManager.Instance.GetMapGrid();

            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1)) continue;

            // Check if something exist
            if (mapGrid[newPos.x, newPos.y] == null) continue;

            // Check if has GroundManager
            if (!mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()) continue;

            // Check if not a Mountain
            if (mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>()
                    .GetCurrentStateEnum() == AllStates.__Pyreneos__) continue;


            var grnd = mapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>();
            var newState = ConditionManager.Instance.GetState(otherState, grnd.StockStatePreview);

            grnd.ChangeStatePrevisu(newState);
            grnd.StockStatePreview = newState;

            _stockGroundPrevisu.Add(grnd);
        }

        // foreach (var grnd in _stockGroundPrevisu)
        // {
        //     _saveGrndToUpdate.Add(grnd, 30);
        // }
    }

    public void UpdateNoSwap(bool state)
    {
        UpdateFBReloadEnergy(state);
        JustBeenSwaped = state;
        //
        if (!state)
            _indicator.GetComponent<GroundIndicator>().UpdateTileState(TileState.Normal, true);
    }

    public void UpdateFbNoSwap(bool state)
    {
        _fbPrevisu.UpdateSwap(state);
    }

    public void UpdateFBReloadEnergy(bool state)
    {
        if (!state && IsBored)
        {
            Instantiate(_fxTileFree, transform);
        }

        IsBored = state;
        _fxTileBored.GetComponent<Fx_BoredTile>().UpdateBored(state);

        if (state)
        {
            _indicator.GetComponent<GroundIndicator>().UpdateTileState(TileState.Bored, true);
        }
        else
            _indicator.GetComponent<GroundIndicator>().UpdateTileState(TileState.Normal, false);
    }

    public void UpdateIsSwapping(bool state)
    {
        _indicator.GetComponent<GroundIndicator>().IsSwapping = state;

        if (!state)
            ResetIndicator();
    }

    private void BounceAnim()
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_startYPosMeshParent - _bottomBounceValue, 0).OnComplete(GetBackMeshParentYPos);
    }

    public void OnSelected() // When bloc is Selected by the player
    {
        if (!MapManager.Instance.IsSwapping)
        {
            MapManager.Instance.CheckIfGroundSelected(gameObject, _coords);
        }
    }

    public AllStates GetCurrentStateEnum()
    {
        return _currentState;
    }

    public AllStates GetCurrentTempStateEnum()
    {
        return _tempCurrentState;
    }

    private void GetBackMeshParentYPos()
    {
        _meshParent.transform.DOKill();
        _meshParent.transform.DOMoveY(_startYPosMeshParent, _timeBounceValue).SetEase(Ease.OutElastic);
    }

    public Sprite GetGroundPrevisu(int index)
    {
        return _fbPrevisu.GetIconTile(index);
    }

    public GameObject GetIndicator()
    {
        return _indicator;
    }

    public Vector2Int GetCoords()
    {
        return _coords;
    }

    public FB_Arrow GetFbArrow()
    {
        return _fbArrow;
    }

    public void ResetIndicator() // Bridge to the indicator and Map_Manager
    {
        _indicator.GetComponent<GroundIndicator>().ResetIndicator();
    }

    public void ResetCountTileChain()
    {
        foreach (var grn in _stockTileChain)
        {
            grn.IsTreated = false;
        }

        _stockTileChain.Clear();
        _countTileChain = 0;
        IsTreated = false;
    }

    public void ResetStockPrevisu()
    {
        ResetPrevisu();

        if (_stockGroundPrevisu.Count == 0) return;

        foreach (var grnd in _stockGroundPrevisu)
        {
            grnd.ResetPrevisu();
        }

        _stockGroundPrevisu.Clear();
    }

    private void ResetPrevisu()
    {
        _fbPrevisu.DeactivateIcon();
        StockStatePreview = _currentState;
    }

    private void OnDisable()
    {
        // MapManager.Instance.CheckBiome -= LaunchCheckForBiome;
        MapManager.Instance.ResetSelection -= ResetIndicator;
    }
}