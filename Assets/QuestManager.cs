using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private bool _isFullFloor;
    private AllStates _fullFloorState;
    // private bool _isFullFloorCompleted;

    private bool _isFlower;
    private AllStates[] _flowerState;

    private bool _isNoSpecificTiles;
    private AllStates[] _noSpecificTileState;

    public void InitQuestFullFloor(AllStates whichState)
    {
        _isFullFloor = true;
        _fullFloorState = whichState;
    }

    public void InitQuestFlower(AllStates[] whichState)
    {
        _isFlower = true;
        _flowerState = whichState;
    }

    public void InitQuestNoSpecificTiles(AllStates[] whichState)
    {
        _isNoSpecificTiles = true;
        _noSpecificTileState = whichState;
    }

    public void CheckQuest()
    {
        if (_isFullFloor)
            print("full floor completed? : " + CheckFullFloorQuest());
        else if (_isFlower)
            print("flower completed : " + CheckFlowerQuest());
        else if(_isNoSpecificTiles)
            print("no specific completed : " + CheckNoSpecificTileQuest());
    }

    private bool CheckFullFloorQuest()
    {
        GameObject[,] map = MapManager.Instance.MapGrid;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.None)
                    continue;

                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() != _fullFloorState)
                    return false;
            }
        }

        return true;
    }

    private bool CheckFlowerQuest()
    {
        GameObject[,] map = MapManager.Instance.MapGrid;
        int count = 0;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                foreach (var state in _flowerState)
                {
                    if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == state &&
                        map[x, y].GetComponent<GroundStateManager>().CheckIfFlower())
                    {
                        count++;
                        break;
                    }
                }
            }
        }

        return count >= _flowerState.Length;
    }

    private bool CheckNoSpecificTileQuest()
    {
        GameObject[,] map = MapManager.Instance.MapGrid;
        
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                foreach (var specificState in _noSpecificTileState)
                {
                    if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == specificState)
                        return false;
                }
            }
        }

        return true;
    }
}