using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance;

    [HideInInspector] public List<TilesCondition> _tileAllConditions;

    public TilesConditionPlain[] TileConditionsPlain;
    public TilesConditionDesert[] TileConditionsDesert;
    public TilesConditionWater[] TileConditionsWater;
    public TilesConditionTropical[] TileConditionsTropical;
    public TilesConditionSavanna[] TileConditionsSavanna;
    public TilesConditionGeyser[] TileConditionsGeyser;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _tileAllConditions.AddRange(TileConditionsPlain);
        _tileAllConditions.AddRange(TileConditionsDesert);
        _tileAllConditions.AddRange(TileConditionsWater);
        _tileAllConditions.AddRange(TileConditionsSavanna);
        _tileAllConditions.AddRange(TileConditionsTropical);
        _tileAllConditions.AddRange(TileConditionsGeyser);
    }


    public AllStates GetState(AllStates current, AllStates other)
    {
        foreach (var tile in _tileAllConditions)
        {
            if (tile.Current == current && tile.Other == other)
                return tile.Result;
        }

        return AllStates.None;
    }
}