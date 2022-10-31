using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class WaterSourceManager : MonoBehaviour
{
    
    private Vector2Int _coords;

    private Vector2Int[] _directions = new Vector2Int[]
        { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    private void Start()
    {
        MapManager.Instance.CheckWaterSource += LaunchWaterCanFlow;
        _coords = GetComponent<GroundManager>().GroundCoords;
    }

    public void LaunchWaterCanFlow()
    {
        _coords = GetComponent<GroundManager>().GroundCoords;
        CheckIfWaterCanFlow(MapManager.Instance.MapGrid, _coords);
    }

    public void CheckIfWaterCanFlow(GameObject[,] mapGrid, Vector2Int coords)
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
            if (mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().IsWater) continue;
            
            mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().ActivateWater();
            CheckIfWaterCanFlow(mapGrid, newPos);
        }
    }

    private void OnDisable()
    {
        MapManager.Instance.CheckWaterSource -= LaunchWaterCanFlow;
    }
}