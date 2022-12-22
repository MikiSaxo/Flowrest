using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStateManager : MonoBehaviour
{
    public GroundBaseState currentState;
    public GroundPlainState plainState = new GroundPlainState();
    public GroundDesertState desertState = new GroundDesertState();
    public GroundWaterState waterState = new GroundWaterState();
    public int IdOfBloc;
    private readonly List<GroundBaseState> _allState = new List<GroundBaseState>();

    public float Temperature;
    [Range(0, 100)] public float Humidity;

    
    [SerializeField] private GameObject _meshParent;
    [SerializeField] private GameObject[] _meshes;
    [SerializeField] private GameObject _indicator;

    private GameObject _meshCurrent;
    private Vector2Int _coords;
    private float _temperatureAround;
    private float _humidityAround;
    private float _countBlocAround;


    private void Awake()
    {
        _allState.Add(plainState);
        _allState.Add(desertState);
        _allState.Add(waterState);
    }

    private void Start()
    {
        n_MapManager.Instance.UpdateGround += GetValuesAround;
    }

    public void InitState(int stateNb)
    {
        _allState[stateNb].InitState(this);
        ChangeState(stateNb);
    }
    
    public void ChangeState(int whichState)
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

    private void GetValuesAround()
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

    private void CheckIfNeedUpdate()
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

    public void OnSelected()
    {
        n_MapManager.Instance.CheckIfGroundSelected(gameObject, _coords);
    } 

    public void ResetMatIndicator()
    {
        _indicator.GetComponent<GroundIndicator>().ResetMat();
    }

    private void OnDisable()
    {
        n_MapManager.Instance.UpdateGround -= GetValuesAround;
    }
}