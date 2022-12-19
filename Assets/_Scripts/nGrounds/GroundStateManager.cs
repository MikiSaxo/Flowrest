using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStateManager : MonoBehaviour
{
    public GroundBaseState currentState;
    public GroundPlainsState PlainsState = new GroundPlainsState();
    public GroundDesertState DesertState = new GroundDesertState();
    public GroundWaterState WaterState = new GroundWaterState();

    [SerializeField] private MeshRenderer _groundMaterial;
    [SerializeField] protected Material[] _materials;

    public float Temperature;
    [Range(0, 100)] public float Humidity;

    private Vector2Int _coords;
    private float _temperatureAround;
    private float _humidityAround;
    private float _countBlocAround;

    private void Start()
    {
        GetValuesAround();
    }

    private IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(.01f);
        ChangeValues(_humidityAround / _countBlocAround, _temperatureAround / _countBlocAround);
    }

    public void ChangeMaterials(int materialNb)
    {
        _groundMaterial.material = _materials[materialNb];
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

    public void SwitchState(GroundBaseState state)
    {
        currentState = state;
        state.EnterState(this);
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
    
    
}