using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class WaterSourceManager : MonoBehaviour
{
    public static event Action ResetTreatedWater;
    
    private Vector2Int _coords;
    private List<GameObject> _watered = new List<GameObject>();

    private Vector2Int[] _directions = new Vector2Int[]
        { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private void Start()
    {
        MapManager.Instance.CheckWaterSource += LaunchWaterCanFlow;
    }
    
    public void ChangeCoords(Vector2Int newCoords) // Change the coords of the ground
    {
        _coords = newCoords;
    }

    private void LaunchWaterCanFlow()
    {
        ResetAllWater();
        CheckIfWaterCanFlow(MapManager.Instance.MapGrid, _coords);
        StartCoroutine(ResetWaterTreated());
    }

    private void CheckIfWaterCanFlow(GameObject[,] mapGrid, Vector2Int coords)
    {
        foreach (var dir in _directions)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 || newPos.y >= mapGrid.GetLength(1)) continue;
            // Check if not null
            if (mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has WaterFlowing
            if (!mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>()) continue;
            // Check if has been already treated
            if (mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().IsTreated) continue;
            
            mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().ActivateWater();
            CheckIfWaterCanFlow(mapGrid, newPos);
            _watered.Add(mapGrid[newPos.x, newPos.y]);
        }
    }

    IEnumerator ResetWaterTreated()
    {
        yield return new WaitForSeconds(.1f);
        ResetTreatedWater?.Invoke();
    }

    private void ResetAllWater()
    {
        foreach (var water in _watered)
        {
            water.GetComponent<WaterFlowing>().DesactivateWater();
        }
        _watered.Clear();
    }

    private void OnDisable()
    {
        MapManager.Instance.CheckWaterSource -= LaunchWaterCanFlow;
    }
}