using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionManager : MonoBehaviour
{
    public List<TilesCondition> TileCondition;

    public AllStates GetState(AllStates current, AllStates other)
    {
        foreach (var tile in TileCondition)
        {
            if (tile.Current == current && tile.Other == other)
                return tile.Result;
        }

        return AllStates.None;
    }
}