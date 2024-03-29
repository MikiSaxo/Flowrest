using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public event Action ResetFlower;

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
    private int _maxTileChainCount;
    private int _countFullFloor;

    private int _countQuestDone;
    private int _countQuestNumber;

    private List<AllStates> _flowerStateDone = new List<AllStates>();

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
        Array.Sort(_flowerState);
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
        if (_isFullFloor && CheckFullFloorQuest())
            _countQuestDone++;
        if (_isFlower && CheckFlowerQuest())
            _countQuestDone++;
        if (_isNoSpecificTiles && CheckNoSpecificTileQuest())
            _countQuestDone++;
        if (_isTileChain && CheckTileChain())
            _countQuestDone++;
        if (_isTileCount && CheckTileCountQuest())
            _countQuestDone++;

        if (_countQuestDone >= _countQuestNumber && _countQuestNumber > 0)
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

        _maxTileChainCount = 0;
        _countFullFloor = 0;

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

                if (grnd.GetCurrentStateEnum() == AllStates.__Pyreneos__)
                    continue;

                _maxTileChainCount++;

                if (grnd.GetCurrentStateEnum() == _fullFloorState)
                    _countFullFloor++;
            }
        }

        ScreensManager.Instance.UpdateOrder(_countFullFloor, 1);

        return _countFullFloor >= _maxTileChainCount;
    }

    private bool CheckFlowerQuest()
    {
        GameObject[,] map = MapManager.Instance.GetMapGrid();
        _flowerStateDone.Clear();
        ScreensManager.Instance.ResetMultipleStock();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                foreach (var state in _flowerState)
                {
                    if (map[x, y].GetComponent<GroundStateManager>().GetCurrentStateEnum() == state
                        && map[x, y].GetComponent<GroundStateManager>().CheckIfFlower())
                    {
                        _flowerStateDone.Add(state);
                        break;
                    }
                }
            }
        }
        
        ResetFlower?.Invoke();

        _flowerStateDone.Sort();
        int countDone = 0;

        foreach (var flowerStateDone in _flowerStateDone)
        {
            foreach (var flowerState in _flowerState)
            {
                if (flowerStateDone == flowerState)
                {
                    countDone++;
                    ScreensManager.Instance.AddNewMultipleOrder(flowerStateDone, 1);
                    break;
                }
            }
        }



        if (_flowerStateDone.Count == _flowerState.Length)
        {
            for (int i = 0; i < _flowerStateDone.Count; i++)
            {
                if (_flowerStateDone[i] != _flowerState[i])
                    return false;
            }
            
            return true;
        }

        return false;

        // return countDone >= _flowerState.Length;
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
        _maxTileChainCount = 0;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == null) continue;

                if (map[x, y].GetComponent<GroundStateManager>() == null)
                    continue;

                var grnd = map[x, y].GetComponent<GroundStateManager>();

                if (grnd.GetCurrentStateEnum() != _tileChainState) continue;

                int getNb = grnd.CountSameTileConnected(map, grnd.GetCoords(), _tileChainState);
                grnd.ResetCountTileChain();

                if (_maxTileChainCount < getNb)
                    _maxTileChainCount = getNb;

                if (getNb >= _tileChainNumber)
                {
                    ScreensManager.Instance.UpdateOrder(_maxTileChainCount, 1);
                    return true;
                }
            }
        }

        ScreensManager.Instance.UpdateOrder(_maxTileChainCount, 1);

        return false;
    }

    private bool CheckTileCountQuest()
    {
        _tileCount = 0;

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

        ScreensManager.Instance.UpdateOrder(_tileCount, 1);

        return _tileCount >= _tileCountNumber;
    }

    private void WarnFullFloorQuest()
    {
        if (_isFullFloor)
            Debug.LogWarning("Be careful, Quest full floor is on and it's combine with another");
    }
}