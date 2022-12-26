using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class GroundStateManager : MonoBehaviour
{
    private GroundBaseState currentState;
    private GroundPlainState plainState = new GroundPlainState();
    private GroundDesertState desertState = new GroundDesertState();
    private GroundWaterState waterState = new GroundWaterState();
    public int IdOfBloc { get; set; }
    
    
    [Header("Setup")]
    [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject[] _meshes;
    
    [Header("Characteristics")]
    public float Temperature;
    [Range(0, 100)] public float Humidity;
    
    private readonly List<GroundBaseState> _allState = new List<GroundBaseState>();
    private GameObject _meshCurrent;
    private Vector2Int _coords;
    private float _temperatureAround;
    private float _humidityAround;
    private float _countBlocAround;
    private float _countSameBlocAround;


    private void Awake()
    {
        _allState.Add(plainState);
        _allState.Add(desertState);
        _allState.Add(waterState);
    }

    private void Start()
    {
        n_MapManager.Instance.UpdateGround += GetValuesAround;
        
        CheckIfBiome();
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
    }

    private IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(.01f);
        ChangeValues(_humidityAround / _countBlocAround, _temperatureAround / _countBlocAround);
        CheckIfNeedUpdate();
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
    }

    public GroundBaseState GetActualState()
    {
        return currentState;
    }
    
    private void CheckIfBiome()
    {
        _countSameBlocAround = 0;
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
                if(n_MapManager.Instance.MapGrid[newPos.x, newPos.y].GetComponent<GroundStateManager>().IdOfBloc != IdOfBloc) continue;
                // It's good
                _countSameBlocAround++;
            }
        }
        print(currentState + " nb : " + _countSameBlocAround);
    }

    private void OnDisable()
    {
        n_MapManager.Instance.UpdateGround -= GetValuesAround;
    }
    
}