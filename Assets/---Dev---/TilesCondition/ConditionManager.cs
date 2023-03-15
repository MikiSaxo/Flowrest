using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance;

    private List<TilesCondition> _tileAllConditions;

    private void Awake()
    {
        Instance = this;
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