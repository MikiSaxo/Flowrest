using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private bool _isFullFloor;
    private bool _isFlower;
    private bool _isNoSpecificTiles;

    private AllStates _fullFloorState;
    private AllStates[] _flowerState;
    private AllStates[] _noSpecificTileState;

    private int _countQuestDone;
    private int _countQuestNumber;

    public void InitQuestFullFloor(AllStates whichState)
    {
        _isFullFloor = true;
        _fullFloorState = whichState;
        _countQuestNumber++;
    }

    public void InitQuestFlower(AllStates[] whichState)
    {
        _isFlower = true;
        _flowerState = whichState;
        _countQuestNumber++;
        
        WarnFullFloorQuest();
    }

    public void InitQuestNoSpecificTiles(AllStates[] whichState)
    {
        _isNoSpecificTiles = true;
        _noSpecificTileState = whichState;
        _countQuestNumber++;
        
        WarnFullFloorQuest();
    }

    public void CheckQuest()
    {
        if (_isFullFloor && CheckFullFloorQuest())
            _countQuestDone++;

        if (_isFlower && CheckFlowerQuest())
            _countQuestDone++;

        if (_isNoSpecificTiles && CheckNoSpecificTileQuest())
            _countQuestDone++;

        
        if (_countQuestDone == _countQuestNumber)
            ScreensManager.Instance.VictoryScreen();
        else
            _countQuestDone = 0;
        // print("flower completed : " + CheckFlowerQuest());
        // print("full floor completed? : " + CheckFullFloorQuest());
        // print("no specific completed : " + CheckNoSpecificTileQuest());
    }

    private bool CheckFullFloorQuest()
    {
        GameObject[,] map = MapManager.Instance.GetMapGrid();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.None)
                    continue;

                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == AllStates.Mountain)
                    continue;

                if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() != _fullFloorState)
                    return false;
            }
        }

        return true;
    }

    private bool CheckFlowerQuest()
    {
        GameObject[,] map = MapManager.Instance.GetMapGrid();
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
        GameObject[,] map = MapManager.Instance.GetMapGrid();

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

    private void WarnFullFloorQuest()
    {
        if(_isFullFloor)
            Debug.LogWarning("Be careful, Quest full floor is on");
    }
}