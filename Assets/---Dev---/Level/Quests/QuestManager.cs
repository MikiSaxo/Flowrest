using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private bool _isFullFloor;
    private bool _isFlower;
    private bool _isNoSpecificTiles;
    private bool _isTileChain;
    private bool _isTileCount;

    private AllStates _fullFloorState;
    private AllStates[] _flowerState;
    private AllStates[] _noSpecificTileState;
    private AllStates _tileChainState;
    private AllStates _tileCountState;

    private int _tileChainNumber;
    private int _tileCountNumber;
    private int _tileCount;

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

    public void InitQuestTileChain(AllStates whichState, int numberToReach)
    {
        _isTileChain = true;
        _tileChainState = whichState;
        _tileChainNumber = numberToReach;
        _countQuestNumber++;

        WarnFullFloorQuest();
    }

    public void InitQuestTileCount(AllStates whichState, int numberToReach)
    {
        _isTileCount = true;
        _tileCountState = whichState;
        _tileCountNumber = numberToReach;
        _tileCount = 0;
        _countQuestNumber++;

        WarnFullFloorQuest();
    }

    public void CheckQuest()
    {
        if (_isFullFloor && CheckFullFloorQuest()
            || _isFlower && CheckFlowerQuest()
            || _isNoSpecificTiles && CheckNoSpecificTileQuest()
            || _isTileChain && CheckTileChain()
            || _isTileCount && CheckTileCountQuest())
            _countQuestDone++;

        if (_countQuestDone == _countQuestNumber && _countQuestNumber > 0)
            ScreensManager.Instance.VictoryScreen();
        else
            _countQuestDone = 0;
        // print("flower completed : " + CheckFlowerQuest());
        // print("full floor completed? : " + CheckFullFloorQuest());
        // print("no specific completed : " + CheckNoSpecificTileQuest());
    }

    public void ResetQuestNumbers()
    {
        _countQuestNumber = 0;
        _countQuestDone = 0;

        _isFullFloor = false;
        _isFlower = false;
        _isNoSpecificTiles = false;
        _isTileChain = false;
        _isTileCount = false;
    }

    private bool CheckFullFloorQuest()
    {
        GameObject[,] map = MapManager.Instance.GetMapGrid();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                var grnd = map[x, y].GetComponent<GroundStateManager>();

                if (grnd == null)
                    continue;

                if (grnd.GetCurrentStateEnum() == AllStates.None)
                    continue;

                if (grnd.GetCurrentStateEnum() == AllStates.Mountain)
                    continue;

                if (grnd.GetCurrentStateEnum() != _fullFloorState)
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

    private bool CheckTileChain()
    {
        GameObject[,] map = MapManager.Instance.GetMapGrid();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                var grnd = map[x, y].GetComponent<GroundStateManager>();
                int getNb = grnd.CountSameTileConnected(map, grnd.GetCoords(), _tileChainState);
                grnd.ResetCountTileChain();

                if (getNb >= _tileChainNumber)
                    return true;
            }
        }

        return false;
    }

    private bool CheckTileCountQuest()
    {
        _tileCount = 0;
        print("launch count quest");

        GameObject[,] map = MapManager.Instance.GetMapGrid();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                var grnd = map[x, y].GetComponent<GroundStateManager>();

                if (grnd == null)
                    continue;

                if (grnd.GetCurrentStateEnum() != _tileCountState)
                    continue;

                _tileCount++;
            }
        }

        print(_tileCount + " / " + _tileCountNumber);
        return _tileCount >= _tileCountNumber;
    }

    private void WarnFullFloorQuest()
    {
        if (_isFullFloor)
            Debug.LogWarning("Be careful, Quest full floor is on and it's combine with another");
    }
}