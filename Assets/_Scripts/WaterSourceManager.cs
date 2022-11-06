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

    private readonly Vector2Int[] _directions = new Vector2Int[]
        { new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

    private void Start()
    {
        MapManager.Instance.CheckWaterSource += LaunchWaterCanFlow;
    }

    public void ChangeCoords(Vector2Int newCoords) // Change the coords of the water in the InitLvl
    {
        _coords = newCoords;
    }

    private void LaunchWaterCanFlow()
    {
        // Reset all the water
        ResetAllWater();
        // Start the recursive
        CheckIfWaterCanFlow(MapManager.Instance.MapGrid, _coords, true, true, true, true);
        // Reboot the water for a future test
        StartCoroutine(ResetWaterTreated());
    }

    private void CheckIfWaterCanFlow(GameObject[,] mapGrid, Vector2Int coords, bool north, bool south, bool west,
        bool east)
    {
        foreach (var dir in _directions)
        {
            Vector2Int newPos = new Vector2Int(coords.x + dir.x, coords.y + dir.y);
            // Check if inside of array
            if (newPos.x < 0 || newPos.x >= mapGrid.GetLength(0) || newPos.y < 0 ||
                newPos.y >= mapGrid.GetLength(1)) continue;
            // Check if not null
            if (mapGrid[newPos.x, newPos.y] == null) continue;
            // Check if has WaterFlowing
            if (!mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>()) continue;
            // Get the directions
            bool[] nswe = new bool[4];
            for (int i = 0; i < nswe.Length; i++)
            {
                nswe[i] = mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().waterData.DirectionsNSWE[i];
            }

            // Check if they have a common direction
            //if(nswe[0] != north && nswe[1] != south && nswe[2] != west && nswe[3] != east) continue;
            if (dir == _directions[0] && (!nswe[1] || !north))
                continue;

            if (dir == _directions[1] && (!nswe[0] || !south))
                continue;

            if (dir == _directions[2] && (!nswe[3] || !west))
                continue;

            if (dir == _directions[3] && (!nswe[2] || !east))
                continue;

            //print("north : " + nswe[0]);
            // print("west : " + nswe[2]);
            // print("east : " + nswe[3]);
            // print("south : " + nswe[1]);
            // Check if has been already treated
            if (mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().IsTreated) continue;

            // It's good so, activate the water 
            mapGrid[newPos.x, newPos.y].GetComponent<WaterFlowing>().ActivateWater();
            // Restart the recursive
            CheckIfWaterCanFlow(mapGrid, newPos, nswe[0], nswe[1], nswe[2], nswe[3]);
            // Add it to the list to reboot it for a future test
            _watered.Add(mapGrid[newPos.x, newPos.y]);
        }
    }

    IEnumerator ResetWaterTreated()
    {
        //---Must change---
        // Wait a little time to be sure the recursive is over
        yield return new WaitForSeconds(.1f);
        // Call the event for all the water blocs
        ResetTreatedWater?.Invoke();
    }

    private void ResetAllWater()
    {
        // Transform the water to no water
        foreach (var water in _watered)
        {
            water.GetComponent<WaterFlowing>().DesactivateWater();
        }

        // Clear the list for a future test
        _watered.Clear();
    }

    private void OnDisable()
    {
        MapManager.Instance.CheckWaterSource -= LaunchWaterCanFlow;
    }
}