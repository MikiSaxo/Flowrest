using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using DG.Tweening;

public class LastMoveManager : MonoBehaviour
{
    public static LastMoveManager Instance;


    private AllStates[,] _currentStateMap;
    private List<AllStates[,]> _stockStateMap = new List<AllStates[,]>();
    private List<bool[,]> _stockCrystals = new List<bool[,]>();
    private List<int> _stockEnergy = new List<int>();
    private List<GroundStateManager[]> _stockLastGroundSwaped = new List<GroundStateManager[]>();
    private List<List<int>> _stockTileButtonTest = new List<List<int>>();
    private List<int> _stockNbRecycle = new List<int>();

    private GroundStateManager[] _lastGroundSwapped = new GroundStateManager[2];


    private GameObject[,] _mapGrid;
    private Vector2Int _mapSize;

    private void Awake()
    {
        Instance = this;
    }

    public void InitMapGrid(GameObject[,] mapGrid)
    {
        _mapGrid = mapGrid;
    }

    public void InitCurrentStateMap(Vector2Int mapSize)
    {
        _mapSize = mapSize;

        _currentStateMap = new AllStates[_mapSize.x, _mapSize.y];
    }

    public void UpdateLastGroundSwapped(GroundStateManager firstSwapped, GroundStateManager secondSwapped)
    {
        _lastGroundSwapped[0] = firstSwapped;
        _lastGroundSwapped[1] = secondSwapped;
    }

    public void UpdateCurrentStateMap(Vector2Int coords, AllStates newState)
    {
        UpdateCurrentStateMap(coords.x, coords.y, newState);
    }

    public void UpdateCurrentStateMap(int x, int y, AllStates newState)
    {
        // print($"x : {x} - y : {y} - state :{newState}");
        _currentStateMap[x, y] = newState;
    }

    private bool[,] UpdateCrystalMap()
    {
        bool[,] newCrystalMap = new bool[_mapSize.x, _mapSize.y];

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_currentStateMap[x, y] != AllStates.None && _currentStateMap[x, y] != AllStates.__Pyreneos__)
                {
                    bool hasCrystal;
                    if (_mapGrid[x, y].GetComponent<CrystalsGround>() != null)
                        hasCrystal = _mapGrid[x, y].GetComponent<CrystalsGround>().GetIfHasCrystal();
                    else
                        hasCrystal = false;

                    newCrystalMap[x, y] = hasCrystal;
                }
                else
                {
                    newCrystalMap[x, y] = false;
                }
            }
        }

        return newCrystalMap;
    }

    public void SaveNewMap()
    {
        // print("new current map");
        ScreensManager.Instance.UpdateBackwardsButton(_stockStateMap.Count != 0);

        // Stock Floor
        AllStates[,] newMapState = new AllStates[_mapSize.x, _mapSize.y];
        Array.Copy(_currentStateMap, newMapState, _currentStateMap.Length);
        _stockStateMap.Add(newMapState);

        // Stock Energy
        _stockEnergy.Add(EnergyManager.Instance.GetCurrentEnergy());

        // Stock Crystals
        bool[,] newCrystalMap = new bool[_mapSize.x, _mapSize.y];
        Array.Copy(UpdateCrystalMap(), newCrystalMap, UpdateCrystalMap().Length);
        _stockCrystals.Add(UpdateCrystalMap());

        // Stock Last Block Swapped
        GroundStateManager[] newLastBlocked = new GroundStateManager[2];
        Array.Copy(_lastGroundSwapped, newLastBlocked, _lastGroundSwapped.Length);
        _stockLastGroundSwaped.Add(newLastBlocked);

        // Get Inventory
        List<GameObject> inventory = SetupUIGround.Instance.GetStockTileButton();
        int[] test = new int[10];
        foreach (var but in inventory)
        {
            var currentTile = but.GetComponent<UIButton>();

            test[(int)currentTile.GetStateButton()] += currentTile.GetNumberLeft();
        }

        _stockTileButtonTest.Add(test.ToList());

        // Get Nb of Recycle
        _stockNbRecycle.Add(MapManager.Instance.NbOfRecycling);
    }

    public void GoToLastMove()
    {
        if (MapManager.Instance.IsSwapping || MapManager.Instance.IsPosing || MapManager.Instance.IsVictory) return;

        if (_stockStateMap.Count <= 1) return;

        print("go to last move"); //" :  size of _currentStateMapStock before : " + _currentStateMapStock.Count);

        AudioManager.Instance.PlaySFX("GoBackwards");

        // Update floor and crystals
        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                if (_stockStateMap[0][x, y] != AllStates.None)
                {
                    // print($"ole / x : {x} - y : {y} - state :{_currentStateMap[x, y]}");
                    _mapGrid[x, y].GetComponent<GroundStateManager>().ForceChangeState(_stockStateMap.Count == 1
                        ? _stockStateMap[0][x, y]
                        : _stockStateMap[^2][x, y]);

                    if (_stockCrystals.Count == 1)
                    {
                        if (_stockCrystals[0][x, y])
                        {
                            _mapGrid[x, y].GetComponent<CrystalsGround>().InitCrystal();
                        }
                    }
                    else
                    {
                        if (_stockCrystals[^2][x, y])
                        {
                            _mapGrid[x, y].GetComponent<CrystalsGround>().InitCrystal();
                        }
                    }
                }
            }
        }

        // Update Energy
        // print($"_stockEnergy[^2] : {_stockEnergy[^2]} -  Current Energy : {EnergyManager.Instance.GetCurrentEnergy()}");
        int getOldEnergy = 0;

        if (_stockEnergy.Count == 1)
            getOldEnergy = _stockEnergy[0] - EnergyManager.Instance.GetCurrentEnergy();
        else
            getOldEnergy = _stockEnergy[^2] - EnergyManager.Instance.GetCurrentEnergy();

        EnergyManager.Instance.UpdateEnergy(getOldEnergy);


        // Update Last Swapped Block
        if (_stockLastGroundSwaped.Count > 1)
        {
            if (_stockLastGroundSwaped[^2][0] != null && _stockLastGroundSwaped[^2][1] != null)
            {
                _stockLastGroundSwaped[^2][0].UpdateNoSwap(true);
                _stockLastGroundSwaped[^2][1].UpdateNoSwap(true);

                MapManager.Instance.UpdateTwoLastSwapped(_stockLastGroundSwaped[^2][0], _stockLastGroundSwaped[^2][1]);
            }
        }

        if (_stockLastGroundSwaped[^1][0] != null)
            _stockLastGroundSwaped[^1][0].UpdateNoSwap(false);
        if (_stockLastGroundSwaped[^1][1] != null)
            _stockLastGroundSwaped[^1][1].UpdateNoSwap(false);


        // Update Inventory
        if (_stockStateMap.Count > 1)
        {
            SetupUIGround.Instance.ResetAllButtons();

            for (int i = 0; i < _stockTileButtonTest[^2].Count; i++)
            {
                for (int j = 0; j < _stockTileButtonTest[^2][i]; j++)
                {
                    SetupUIGround.Instance.AddNewGround(i, true);
                }
            }
        }

        // Update Recycling
        MapManager.Instance.NbOfRecycling = _stockNbRecycle.Count == 1 ? _stockNbRecycle[0] : _stockNbRecycle[^2];
        RecyclingManager.Instance.UpdateNbRecyclingLeft();

        // Remove Last
        if (_stockStateMap.Count > 1)
        {
            _stockStateMap.RemoveAt(_stockStateMap.Count - 1);
            _stockEnergy.RemoveAt(_stockEnergy.Count - 1);
            _stockLastGroundSwaped.RemoveAt(_stockLastGroundSwaped.Count - 1);
            _stockTileButtonTest.RemoveAt(_stockTileButtonTest.Count - 1);
            _stockNbRecycle.RemoveAt(_stockNbRecycle.Count - 1);
            _stockCrystals.RemoveAt(_stockCrystals.Count - 1);
        }

        MapManager.Instance.QuestsManager.CheckQuest();

        if (_stockStateMap.Count <= 1)
        {
            ScreensManager.Instance.UpdateBackwardsButton(false);
        }
        
        MapManager.Instance.ResetBig();
    }

    public void ResetGoToLastMove()
    {
        _stockStateMap.Clear();
        _stockEnergy.Clear();
        _stockLastGroundSwaped.Clear();
        _stockTileButtonTest.Clear();
        _stockNbRecycle.Clear();
        _stockCrystals.Clear();
    }
}