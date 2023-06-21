using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DialogUpgrades
{
    public Upgrades _Upgrades;
}

public enum Upgrades
{
    Nothing = 0,
    IsBlindFoldGrassias = 1,
    IsBlindFoldCalcid = 2,
    IsStinking = 3,
    IsRambo = 4,
    IsWaterColor = 5,
    IsKohLanta = 6,
    IsGoMuscu = 7,
    IsPoor = 8,
    IsFootball = 9,
    IsGold = 10,
    IsIceberg = 11,
    IsIcePig = 12
}
